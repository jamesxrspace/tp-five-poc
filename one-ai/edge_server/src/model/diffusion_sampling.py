import copy
from functools import partial

import numpy as np
import torch
import torch.nn.functional as F
from einops import reduce
from model.quaternion import ax_from_6v, quat_slerp
from pytorch3d.transforms import (axis_angle_to_quaternion,
                                  quaternion_to_axis_angle)

from .utils import extract, extract_no_grad, make_beta_schedule


def identity(t, *args, **kwargs):
    return t


class EMA:
    def __init__(self, beta):
        super().__init__()
        self.beta = beta

    def update_model_average(self, ma_model, current_model):
        for current_params, ma_params in zip(
            current_model.parameters(), ma_model.parameters()
        ):
            old_weight, up_weight = ma_params.data, current_params.data
            ma_params.data = self.update_average(old_weight, up_weight)

    def update_average(self, old, new):
        if old is None:
            return new
        return old * self.beta + (1 - self.beta) * new


class GaussianDiffusionSampling(torch.nn.Module):
    def __init__(
        self,
        model,
        horizon,
        repr_dim,
        smpl,
        n_timestep=1000,
        schedule="linear",
        loss_type="l1",
        clip_denoised=True,
        predict_epsilon=True,
        guidance_weight=3,
        use_p2=False,
        cond_drop_prob=0.2,
        sampling_timesteps=50,
    ):
        super().__init__()
        self.horizon = horizon
        self.transition_dim = repr_dim
        self.model = model
        self.sampling_timesteps = sampling_timesteps
        self.ema = EMA(0.9999)
        self.master_model = copy.deepcopy(self.model)

        self.cond_drop_prob = cond_drop_prob

        # make a SMPL instance for FK module
        self.smpl = smpl

        betas = torch.Tensor(
            make_beta_schedule(schedule=schedule, n_timestep=n_timestep)
        )
        alphas = 1.0 - betas
        alphas_cumprod = torch.cumprod(alphas, axis=0)
        alphas_cumprod_prev = torch.cat([torch.ones(1), alphas_cumprod[:-1]])

        self.n_timestep = int(n_timestep)
        self.clip_denoised = clip_denoised
        self.predict_epsilon = predict_epsilon

        self.register_buffer("betas", betas)
        self.register_buffer("alphas_cumprod", alphas_cumprod)
        self.register_buffer("alphas_cumprod_prev", alphas_cumprod_prev)

        self.guidance_weight = guidance_weight

        # calculations for diffusion q(x_t | x_{t-1}) and others
        self.register_buffer("sqrt_alphas_cumprod", torch.sqrt(alphas_cumprod))
        self.register_buffer(
            "sqrt_one_minus_alphas_cumprod", torch.sqrt(1.0 - alphas_cumprod)
        )
        self.register_buffer(
            "log_one_minus_alphas_cumprod", torch.log(1.0 - alphas_cumprod)
        )
        self.register_buffer(
            "sqrt_recip_alphas_cumprod", torch.sqrt(1.0 / alphas_cumprod)
        )
        self.register_buffer(
            "sqrt_recipm1_alphas_cumprod", torch.sqrt(1.0 / alphas_cumprod - 1)
        )

        # calculations for posterior q(x_{t-1} | x_t, x_0)
        posterior_variance = (
            betas * (1.0 - alphas_cumprod_prev) / (1.0 - alphas_cumprod)
        )
        self.register_buffer("posterior_variance", posterior_variance)

        ## log calculation clipped because the posterior variance
        ## is 0 at the beginning of the diffusion chain
        self.register_buffer(
            "posterior_log_variance_clipped",
            torch.log(torch.clamp(posterior_variance, min=1e-20)),
        )
        self.register_buffer(
            "posterior_mean_coef1",
            betas * np.sqrt(alphas_cumprod_prev) / (1.0 - alphas_cumprod),
        )
        self.register_buffer(
            "posterior_mean_coef2",
            (1.0 - alphas_cumprod_prev) * np.sqrt(alphas) / (1.0 - alphas_cumprod),
        )

        # p2 weighting
        self.p2_loss_weight_k = 1
        self.p2_loss_weight_gamma = 0.5 if use_p2 else 0
        self.register_buffer(
            "p2_loss_weight",
            (self.p2_loss_weight_k + alphas_cumprod / (1 - alphas_cumprod))
            ** -self.p2_loss_weight_gamma,
        )

        ## get loss coefficients and initialize objective
        self.loss_fn = F.mse_loss if loss_type == "l2" else F.l1_loss

    def predict_start_from_noise(self, x_t, t, noise):
        """
        if self.predict_epsilon, model output is (scaled) noise;
        otherwise, model predicts x0 directly
        """
        if self.predict_epsilon:
            return (
                extract(self.sqrt_recip_alphas_cumprod, t, x_t.shape) * x_t
                - extract(self.sqrt_recipm1_alphas_cumprod, t, x_t.shape) * noise
            )
        else:
            return noise

    def predict_noise_from_start(self, x_t, t, x0):
        # replace gather to indices for onnx transform
        return (
            extract_no_grad(self.sqrt_recip_alphas_cumprod, t, x_t.shape) * x_t - x0
        ) / extract_no_grad(self.sqrt_recipm1_alphas_cumprod, t, x_t.shape)

    def model_predictions(self, x, cond, t, weight=None, clip_x_start=False):
        weight = weight if weight is not None else self.guidance_weight
        model_output = self.model.guided_forward(x, cond, t, weight)
        maybe_clip = (
            partial(torch.clamp, min=-1.0, max=1.0) if clip_x_start else identity
        )

        x_start = model_output
        x_start = maybe_clip(x_start)
        pred_noise = self.predict_noise_from_start(x, t, x_start)

        return pred_noise, x_start

    def q_posterior(self, x_start, x_t, t):
        posterior_mean = (
            extract(self.posterior_mean_coef1, t, x_t.shape) * x_start
            + extract(self.posterior_mean_coef2, t, x_t.shape) * x_t
        )
        posterior_variance = extract(self.posterior_variance, t, x_t.shape)
        posterior_log_variance_clipped = extract(
            self.posterior_log_variance_clipped, t, x_t.shape
        )
        return posterior_mean, posterior_variance, posterior_log_variance_clipped

    def p_mean_variance(self, x, cond, t):
        # guidance clipping
        if t[0] > 1.0 * self.n_timestep:
            weight = min(self.guidance_weight, 0)
        elif t[0] < 0.1 * self.n_timestep:
            weight = min(self.guidance_weight, 1)
        else:
            weight = self.guidance_weight

        x_recon = self.predict_start_from_noise(
            x, t=t, noise=self.model.guided_forward(x, cond, t, weight)
        )

        if self.clip_denoised:
            x_recon.clamp_(-1.0, 1.0)
        else:
            assert RuntimeError()

        model_mean, posterior_variance, posterior_log_variance = self.q_posterior(
            x_start=x_recon, x_t=x, t=t
        )
        return model_mean, posterior_variance, posterior_log_variance, x_recon

    @torch.no_grad()
    def p_sample(self, x, cond, t):
        b = x.shape[0]
        model_mean, _, model_log_variance, x_start = self.p_mean_variance(
            x=x, cond=cond, t=t
        )
        noise = torch.randn_like(model_mean)
        # no noise when t == 0
        nonzero_mask = (1 - (t == 0).float()).reshape(
            b, *((1,) * (len(noise.shape) - 1))
        )
        x_out = model_mean + nonzero_mask * (0.5 * model_log_variance).exp() * noise
        return x_out, x_start

    @torch.no_grad()
    def p_sample_loop(
        self,
        shape,
        cond,
        noise=None,
        constraint=None,
        return_diffusion=False,
        start_point=None,
    ):
        device = self.betas.device

        # default to diffusion over whole timescale
        start_point = self.n_timestep if start_point is None else start_point
        batch_size = shape[0]
        x = torch.randn(shape, device=device) if noise is None else noise.to(device)
        cond = cond.to(device)

        if return_diffusion:
            diffusion = [x]

        for i in reversed(range(0, start_point)):
            # fill with i
            timesteps = torch.full((batch_size,), i, device=device, dtype=torch.long)
            x, _ = self.p_sample(x, cond, timesteps)

            if return_diffusion:
                diffusion.append(x)

        if return_diffusion:
            return x, diffusion
        else:
            return x

    @torch.no_grad()
    def ddim_sample(self, shape, cond, **kwargs):
        batch, device, total_timesteps, sampling_timesteps, eta = (
            shape[0],
            self.betas.device,
            self.n_timestep,
            50,
            1,
        )

        times = torch.linspace(
            -1, total_timesteps - 1, steps=sampling_timesteps + 1
        )  # [-1, 0, 1, 2, ..., T-1] when sampling_timesteps == total_timesteps
        times = list(reversed(times.int().tolist()))
        time_pairs = list(
            zip(times[:-1], times[1:])
        )  # [(T-1, T-2), (T-2, T-3), ..., (1, 0), (0, -1)]

        x = torch.randn(shape, device=device)
        cond = cond.to(device)

        x_start = None

        for time, time_next in time_pairs:
            time_cond = torch.full((batch,), time, device=device, dtype=torch.long)
            pred_noise, x_start, *_ = self.model_predictions(
                x, cond, time_cond, clip_x_start=self.clip_denoised
            )

            if time_next < 0:
                x = x_start
                continue

            alpha = self.alphas_cumprod[time]
            alpha_next = self.alphas_cumprod[time_next]

            sigma = (
                eta * ((1 - alpha / alpha_next) * (1 - alpha_next) / (1 - alpha)).sqrt()
            )
            c = (1 - alpha_next - sigma**2).sqrt()

            noise = torch.randn_like(x)

            x = x_start * alpha_next.sqrt() + c * pred_noise + sigma * noise
        return x

    @torch.no_grad()
    def long_ddim_sample(self, shape, cond, **kwargs):
        batch, device, total_timesteps, eta = (
            shape[0],
            self.betas.device,
            self.n_timestep,
            1,
        )

        if batch == 1:
            return self.ddim_sample(shape, cond)

        times = torch.linspace(
            -1, total_timesteps - 1, steps=self.sampling_timesteps + 1
        )  # [-1, 0, 1, 2, ..., T-1] when sampling_timesteps == total_timesteps

        times = list(reversed(times.int().tolist()))

        weights = np.clip(
            np.linspace(0, self.guidance_weight * 2, self.sampling_timesteps),
            None,
            self.guidance_weight,
        )
        time_pairs = list(
            zip(times[:-1], times[1:], weights)
        )  # [(T-1, T-2), (T-2, T-3), ..., (1, 0), (0, -1)]

        x = torch.randn(shape, device=device)
        init_x = x
        cond = cond.to(device)

        half = x.shape[1] // 2

        x_start = None
        rand_data = [x.cpu().numpy()]

        for time, time_next, weight in time_pairs:
            time_cond = torch.full((batch,), time, device=device, dtype=torch.long)
            pred_noise, x_start, *_ = self.model_predictions(
                x, cond, time_cond, weight=weight, clip_x_start=self.clip_denoised
            )

            if time_next < 0:
                x = x_start
                continue

            alpha = self.alphas_cumprod[time]
            alpha_next = self.alphas_cumprod[time_next]

            sigma = (
                eta * ((1 - alpha / alpha_next) * (1 - alpha_next) / (1 - alpha)).sqrt()
            )
            c = (1 - alpha_next - sigma**2).sqrt()

            noise = torch.randn_like(x)
            rand_data.append(noise.cpu().numpy())

            x = x_start * alpha_next.sqrt() + c * pred_noise + sigma * noise

            if time > 0:
                # the first half of each sequence is the second half of the previous one
                x[1:, :half] = x[:-1, half:]

        if "return_rand_data" in kwargs and kwargs["return_rand_data"]:
            return rand_data, x
        elif "return_init_x" in kwargs and kwargs["return_init_x"]:
            return init_x, x
        else:
            return x

    @torch.no_grad()
    def long_ddim_sample_slice(self, shape, cond, initx, rand_data, record=None):
        batch, device, total_timesteps, eta = (
            shape[0],
            self.betas.device,
            self.n_timestep,
            0,
        )

        times = torch.linspace(
            -1, total_timesteps - 1, steps=self.sampling_timesteps + 1
        )  # [-1, 0, 1, 2, ..., T-1] when sampling_timesteps == total_timesteps

        times = list(reversed(times.int().tolist()))

        weights = np.clip(
            np.linspace(0, self.guidance_weight * 2, self.sampling_timesteps),
            None,
            self.guidance_weight,
        )
        time_pairs = list(
            zip(times[:-1], times[1:], weights)
        )  # [(T-1, T-2), (T-2, T-3), ..., (1, 0), (0, -1)]

        x = initx.to(device)
        cond = cond.to(device)
        rand_data = rand_data.to(device)
        half = x.shape[1] // 2
        time_idx = 0
        rand_index = 0
        last_record = torch.zeros((len(times[:-1]), half, shape[2]), device=device)

        for time, time_next, weight in time_pairs:
            time_cond = torch.full((batch,), time, device=device, dtype=torch.long)
            pred_noise, x_start, *_ = self.model_predictions(
                x, cond, time_cond, weight=weight, clip_x_start=self.clip_denoised
            )

            if time_next < 0:
                x = x_start
                continue

            alpha = self.alphas_cumprod[time]
            alpha_next = self.alphas_cumprod[time_next]

            sigma = (
                eta * ((1 - alpha / alpha_next) * (1 - alpha_next) / (1 - alpha)).sqrt()
            )
            c = (1 - alpha_next - sigma**2).sqrt()

            noise = rand_data[rand_index]
            rand_index += 1

            x = x_start * alpha_next.sqrt() + c * pred_noise + sigma * noise

            if time > 0:
                # the first half of each sequence is the second half of the previous one
                x[1:, :half] = x[:-1, half:]
                if record is not None:
                    x[0, :half, :] = record[time_idx]
                last_record[time_idx] = x[-1, half:, :]
                time_idx += 1

        return x, last_record

    @torch.no_grad()
    def inpaint_loop(
        self,
        shape,
        cond,
        noise=None,
        constraint=None,
        return_diffusion=False,
        start_point=None,
        **kwargs,
    ):
        device = self.betas.device

        batch_size = shape[0]
        x = torch.randn(shape, device=device) if noise is None else noise.to(device)
        cond = cond.to(device)
        if return_diffusion:
            diffusion = [x]

        mask = constraint["mask"].to(device)  # batch x horizon x channels
        value = constraint["value"].to(device)  # batch x horizon x channels

        start_point = self.n_timestep if start_point is None else start_point
        for i in reversed(range(0, start_point)):
            # fill with i
            timesteps = torch.full((batch_size,), i, device=device, dtype=torch.long)

            # sample x from step i to step i-1
            x, _ = self.p_sample(x, cond, timesteps)
            # enforce constraint between each denoising step
            value_ = self.q_sample(value, timesteps - 1) if (i > 0) else x
            x = value_ * mask + (1.0 - mask) * x

            if return_diffusion:
                diffusion.append(x)

        if return_diffusion:
            return x, diffusion
        else:
            return x

    @torch.no_grad()
    def long_inpaint_loop(
        self,
        shape,
        cond,
        noise=None,
        constraint=None,
        return_diffusion=False,
        start_point=None,
        **kwargs,
    ):
        # sampling_timesteps = self.sampling_timesteps
        sampling_timesteps = 700
        batch, device, total_timesteps = (
            shape[0],
            self.betas.device,
            self.n_timestep,
        )

        times = torch.linspace(
            -1, total_timesteps - 1, steps=sampling_timesteps + 1
        )  # [-1, 0, 1, 2, ..., T-1] when sampling_timesteps == total_timesteps

        times = list(reversed(times.int().tolist()))

        weights = np.clip(
            np.linspace(0, self.guidance_weight * 2, sampling_timesteps),
            None,
            self.guidance_weight,
        )
        time_pairs = list(
            zip(times[:-1], times[1:], weights)
        )  # [(T-1, T-2), (T-2, T-3), ..., (1, 0), (0, -1)]

        batch_size = shape[0]
        x = torch.randn(shape, device=device) if noise is None else noise.to(device)
        cond = cond.to(device)
        if return_diffusion:
            diffusion = [x]

        assert x.shape[1] % 2 == 0
        if batch_size == 1:
            # there's no continuation to do, just do normal
            return self.p_sample_loop(
                shape,
                cond,
                noise=noise,
                constraint=constraint,
                return_diffusion=return_diffusion,
                start_point=start_point,
            )
        assert batch_size > 1
        half = x.shape[1] // 2

        mask = constraint["mask"].to(device)  # batch x horizon x channels
        value = constraint["value"].to(device)  # batch x horizon x channels

        resampling_steps = 1  # was 50 before

        for time, time_next, _ in time_pairs:
            for u in range(resampling_steps):
                # fill with i
                timesteps = torch.full((batch,), time, device=device, dtype=torch.long)
                timesteps_next = torch.full(
                    (batch,), time_next, device=device, dtype=torch.long
                )

                # sample x from step i to step i-1
                x, _ = self.p_sample(x, cond, timesteps)
                # enforce constraint between each denoising step
                if time_next > 0:
                    value_ = self.q_sample(value, timesteps_next)
                    x = value_ * mask + (1.0 - mask) * x

                # resample
                if u < resampling_steps - 1 and time_next > 0:
                    x = self.q_sample(value, timesteps_next)

                if time > 0:
                    x[1:, :half] = x[:-1, half:]

                if return_diffusion:
                    diffusion.append(x)

        if return_diffusion:
            return x, diffusion
        else:
            return x

    @torch.no_grad()
    def conditional_sample(
        self, shape, cond, constraint=None, *args, horizon=None, **kwargs
    ):
        return self.p_sample_loop(shape, cond, *args, **kwargs)

    def q_sample(self, x_start, t, noise=None):
        if noise is None:
            noise = torch.randn_like(x_start)

        sample = (
            extract(self.sqrt_alphas_cumprod, t, x_start.shape) * x_start
            + extract(self.sqrt_one_minus_alphas_cumprod, t, x_start.shape) * noise
        )

        return sample

    def p_losses(self, x_start, cond, t):
        noise = torch.randn_like(x_start)
        x_noisy = self.q_sample(x_start=x_start, t=t, noise=noise)

        # reconstruct
        x_recon = self.model(x_noisy, cond, t, cond_drop_prob=self.cond_drop_prob)
        assert noise.shape == x_recon.shape

        model_out = x_recon
        if self.predict_epsilon:
            target = noise
        else:
            target = x_start

        # full reconstruction loss
        loss = self.loss_fn(model_out, target, reduction="none")
        loss = reduce(loss, "b ... -> b (...)", "mean")
        loss = loss * extract(self.p2_loss_weight, t, loss.shape)

        # split off contact from the rest
        model_contact, model_out = torch.split(
            model_out, (4, model_out.shape[2] - 4), dim=2
        )
        _, target = torch.split(target, (4, target.shape[2] - 4), dim=2)

        # velocity loss
        target_v = target[:, 1:] - target[:, :-1]
        model_out_v = model_out[:, 1:] - model_out[:, :-1]
        v_loss = self.loss_fn(model_out_v, target_v, reduction="none")
        v_loss = reduce(v_loss, "b ... -> b (...)", "mean")
        v_loss = v_loss * extract(self.p2_loss_weight, t, v_loss.shape)

        # FK loss
        b, s, c = model_out.shape
        # unnormalize
        # model_out = self.normalizer.unnormalize(model_out)
        # target = self.normalizer.unnormalize(target)
        # X, Q
        model_x = model_out[:, :, :3]
        model_q = ax_from_6v(model_out[:, :, 3:].reshape(b, s, -1, 6))
        target_x = target[:, :, :3]
        target_q = ax_from_6v(target[:, :, 3:].reshape(b, s, -1, 6))

        # perform FK
        model_xp = self.smpl.forward(model_q, model_x)
        target_xp = self.smpl.forward(target_q, target_x)

        fk_loss = self.loss_fn(model_xp, target_xp, reduction="none")
        fk_loss = reduce(fk_loss, "b ... -> b (...)", "mean")
        fk_loss = fk_loss * extract(self.p2_loss_weight, t, fk_loss.shape)

        # foot skate loss
        foot_idx = [7, 8, 10, 11]

        # find static indices consistent with model's own predictions
        static_idx = model_contact > 0.95  # N x S x 4
        model_feet = model_xp[:, :, foot_idx]  # foot positions (N, S, 4, 3)
        model_foot_v = torch.zeros_like(model_feet)
        model_foot_v[:, :-1] = (
            model_feet[:, 1:, :, :] - model_feet[:, :-1, :, :]
        )  # (N, S-1, 4, 3)
        model_foot_v[~static_idx] = 0
        foot_loss = self.loss_fn(
            model_foot_v, torch.zeros_like(model_foot_v), reduction="none"
        )
        foot_loss = reduce(foot_loss, "b ... -> b (...)", "mean")

        losses = (
            0.636 * loss.mean(),
            2.964 * v_loss.mean(),
            0.646 * fk_loss.mean(),
            10.942 * foot_loss.mean(),
        )
        return sum(losses), losses

    def loss(self, x, cond, t_override=None):
        batch_size = len(x)
        if t_override is None:
            t = torch.randint(0, self.n_timestep, (batch_size,), device=x.device).long()
        else:
            t = torch.full((batch_size,), t_override, device=x.device).long()
        return self.p_losses(x, cond, t)

    def forward(self, x, cond, time_cond, weight):
        pred_noise, x_start, *_ = self.model_predictions(
            x, cond, time_cond, weight=weight, clip_x_start=self.clip_denoised
        )

        return pred_noise, x_start

    def partial_denoise(self, x, cond, t):
        x_noisy = self.noise_to_t(x, t)
        return self.p_sample_loop(x.shape, cond, noise=x_noisy, start_point=t)

    def noise_to_t(self, x, timestep):
        batch_size = len(x)
        t = torch.full((batch_size,), timestep, device=x.device).long()
        return self.q_sample(x, t) if timestep > 0 else x

    def slice_sampling(
        self,
        shape,
        cond,
        initx,
        rand_data,
        record,
    ):
        ddpm_outputs = self.long_ddim_sample_slice(
            shape,
            cond,
            initx,
            rand_data,
            record,
        )
        return ddpm_outputs

    def slice_post_process(self, samples, normalizer):
        device = self.betas.device
        samples = samples.detach().cpu()
        samples = normalizer.unnormalize(samples)

        sample_contact, samples = torch.split(samples, (4, samples.shape[2] - 4), dim=2)

        # do the FK all at once
        b, s, _ = samples.shape
        pos = samples[:, :, :3].to(device)  # np.zeros((sample.shape[0], 3))
        q = samples[:, :, 3:].reshape(b, s, 24, 6)
        # go 6d to ax
        q = ax_from_6v(q).to(device)
        foot_contact = sample_contact.clone().to(device)

        b, s, c1, c2 = q.shape
        assert s % 2 == 0
        half = s // 2
        if b > 1:
            # if long mode, stitch position using linear interp
            fade_out = torch.ones((1, s, 1)).to(pos.device)
            fade_in = torch.ones((1, s, 1)).to(pos.device)
            fade_out[:, half:, :] = torch.linspace(1, 0, half)[None, :, None].to(
                pos.device
            )
            fade_in[:, :half, :] = torch.linspace(0, 1, half)[None, :, None].to(
                pos.device
            )

            pos[:-1] *= fade_out
            pos[1:] *= fade_in

            full_pos = torch.zeros((s + half * (b - 1), 3)).to(pos.device)
            idx = 0
            for pos_slice in pos:
                full_pos[idx : idx + s] += pos_slice
                idx += half

            foot_contact[:-1] *= fade_out
            foot_contact[1:] *= fade_in

            full_foot_contact = torch.zeros((s + half * (b - 1), 4)).to(pos.device)
            idx = 0
            for foot_slice in foot_contact:
                full_foot_contact[idx : idx + s] += foot_slice
                idx += half

            # stitch joint angles with slerp
            slerp_weight = torch.linspace(0, 1, half)[None, :, None].to(pos.device)

            left, right = q[:-1, half:], q[1:, :half]
            # convert to quaternion
            left, right = (
                axis_angle_to_quaternion(left),
                axis_angle_to_quaternion(right),
            )
            merged = quat_slerp(left, right, slerp_weight)  # (b-1) x half x ...
            # convert back
            merged = quaternion_to_axis_angle(merged)

            full_q = torch.zeros((s + half * (b - 1), c1, c2)).to(pos.device)
            full_q[:half] += q[0, :half]
            idx = half
            for q_slice in merged:
                full_q[idx : idx + half] += q_slice
                idx += half
            full_q[idx : idx + half] += q[-1, half:]

            # unsqueeze for fk
            full_pos = full_pos.unsqueeze(0)
            full_q = full_q.unsqueeze(0)
        else:
            full_pos = pos
            full_q = q
            full_foot_contact = foot_contact[0]

        full_pose = self.smpl.forward(full_q, full_pos).detach().cpu().numpy()

        return (
            full_pose[0],
            full_pos[0].detach().cpu().numpy(),
            full_q[0].detach().cpu().numpy(),
        )