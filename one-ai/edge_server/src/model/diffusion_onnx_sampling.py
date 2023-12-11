import numpy as np
import torch


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

        cosine_s = 8e-3
        timesteps = np.arange(n_timestep + 1) / n_timestep + cosine_s
        alphas = timesteps / (1 + cosine_s) * np.pi / 2
        alphas = np.cos(alphas) ** 2
        alphas = alphas / alphas[0]
        betas = 1 - alphas[1:] / alphas[:-1]
        betas = np.clip(betas, a_min=0, a_max=0.999)

        alphas = 1.0 - betas
        self.alphas_cumprod = np.cumprod(alphas, axis=0)

        # make a SMPL instance for FK module
        self.smpl = smpl

        self.n_timestep = int(n_timestep)
        self.predict_epsilon = predict_epsilon

        self.guidance_weight = guidance_weight

    @torch.no_grad()
    def long_ddim_sample(self, shape, cond, rand_data=None, slice_sampling=False):
        batch, total_timesteps, eta = shape[0], self.n_timestep, 1

        # [-1, 0, 1, 2, ..., T-1] when sampling_timesteps == total_timesteps
        times = np.linspace(-1, total_timesteps - 1, self.sampling_timesteps + 1)
        times = list(reversed(times.astype(int).tolist()))

        weights = np.clip(
            np.linspace(0, self.guidance_weight * 2, self.sampling_timesteps),
            None,
            self.guidance_weight,
        )
        time_pairs = list(
            zip(times[:-1], times[1:], weights)
        )  # [(T-1, T-2), (T-2, T-3), ..., (1, 0), (0, -1)]

        if rand_data is None:
            x = np.random.randn(shape[0], shape[1], shape[2])
            rand_noises = np.random.randn(
                self.sampling_timesteps, shape[0], shape[1], shape[2]
            )
        else:
            x = np.array(rand_data[0])
            rand_noises = np.array(rand_data[1:])

        half = x.shape[1] // 2

        if slice_sampling:
            slice_unit = 5
            split_slice_size = batch // slice_unit
        else:
            slice_unit = batch
            split_slice_size = 1
        split_x_slice = x.reshape(split_slice_size, slice_unit, shape[1], shape[2])
        split_rand_noises = rand_noises.reshape(
            self.sampling_timesteps - 1,
            split_slice_size,
            slice_unit,
            shape[1],
            shape[2],
        )
        split_cond = cond.reshape(
            split_slice_size, slice_unit, cond.shape[1], cond.shape[2]
        )
        split_x_slice_last_record = np.zeros(
            (split_slice_size - 1, len(times[:-1]), half, shape[2])
        )

        for s in range(split_slice_size):
            sub_x = split_x_slice[s]
            time_loop_count = 0
            rand_index = 0

            for time, time_next, weight in time_pairs:
                time_cond = np.repeat(time, slice_unit).astype(np.int64)
                ort_inputs = {
                    self.model.get_inputs()[0].name: np.array(sub_x),
                    self.model.get_inputs()[1].name: np.array(split_cond[s]),
                    self.model.get_inputs()[2].name: time_cond,
                    self.model.get_inputs()[3]
                    .name: np.array(weight)
                    .astype(np.float32),
                }

                ort_outs = self.model.run(None, ort_inputs)
                pred_noise = np.array(ort_outs[0])
                x_start = np.array(ort_outs[1])

                if time_next < 0:
                    sub_x = x_start
                    continue

                alpha = self.alphas_cumprod[time]
                alpha_next = self.alphas_cumprod[time_next]

                sigma = eta * np.sqrt(
                    (1 - alpha / alpha_next) * (1 - alpha_next) / (1 - alpha)
                )
                c = np.sqrt(1 - alpha_next - sigma**2)

                noise = split_rand_noises[rand_index, s]
                rand_index += 1

                sub_x = x_start * np.sqrt(alpha_next) + c * pred_noise + sigma * noise

                if time > 0:
                    sub_x[1:, :half] = sub_x[:-1, half:]

                    if s > 0:
                        sub_x[0, :half, :] = split_x_slice_last_record[
                            s - 1, time_loop_count, :, :
                        ]

                    # record sub_x
                    if s < split_slice_size - 1:
                        split_x_slice_last_record[s, time_loop_count] = sub_x[
                            -1, half:, :
                        ]
                    time_loop_count += 1

            split_x_slice[s] = sub_x

        x = split_x_slice.reshape(shape[0], shape[1], shape[2])
        return x
