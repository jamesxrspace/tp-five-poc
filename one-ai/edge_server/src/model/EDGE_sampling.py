import math

import torch
import torch.nn as nn
import torch.nn.functional as F

from .diffusion_sampling import GaussianDiffusionSampling
from .model import DanceDecoder
from .smpl_skeleton import SMPLSkeleton


def wrap(x):
    return {f"module.{key}": value for key, value in x.items()}


def maybe_wrap(x, num):
    return x if num == 1 else wrap(x)


class GeluFromSource(nn.Module):
    def __init__(
        self,
    ):
        super().__init__()

    def forward(self, x):
        return (
            0.5 * x * (1 + torch.tanh(math.sqrt(math.pi / 2) * (x + 0.044715 * x**3)))
        )


class EDGESampling(nn.Module):
    def __init__(
        self,
        feature_type,
        checkpoint_path="",
        EMA=True,
        sampling_timesteps=50,
        device="cpu",
    ):
        super(EDGESampling, self).__init__()
        use_baseline_feats = feature_type == "baseline"

        pos_dim = 3
        rot_dim = 24 * 6  # 24 joints, 6dof
        self.repr_dim = repr_dim = pos_dim + rot_dim + 4
        self.device = device

        if feature_type == "MERT" or feature_type == "MERT_resample":
            feature_dim = 768
        elif use_baseline_feats:
            feature_dim = 35
        else:
            feature_dim = 4800

        horizon_seconds = 5
        FPS = 30
        self.horizon = horizon = horizon_seconds * FPS
        self.feature_dim = feature_dim

        checkpoint = None
        if checkpoint_path != "":
            checkpoint = torch.load(checkpoint_path, map_location=self.device)
            self.normalizer = checkpoint["normalizer"]

        self.model = DanceDecoder(
            nfeats=repr_dim,
            seq_len=horizon,
            latent_dim=512,
            ff_size=1024,
            num_layers=8,
            num_heads=8,
            dropout=0.1,
            cond_feature_dim=feature_dim,
            activation=F.gelu,
        )

        smpl = SMPLSkeleton(self.device)
        self.diffusion = GaussianDiffusionSampling(
            self.model,
            horizon,
            repr_dim,
            smpl,
            schedule="cosine",
            n_timestep=1000,
            predict_epsilon=False,
            loss_type="l2",
            use_p2=False,
            cond_drop_prob=0.25,
            guidance_weight=2,
            sampling_timesteps=sampling_timesteps,
        )

        self.model = self.model.to(device)
        self.diffusion = self.diffusion.to(device)

        if checkpoint_path != "":
            self.model.load_state_dict(
                maybe_wrap(
                    checkpoint["ema_state_dict" if EMA else "model_state_dict"],
                    1,
                )
            )

    def eval(self):
        self.diffusion.eval()

    def forward(self, x, cond, time_cond, weight):
        pred_noise, x_start = self.diffusion(x, cond, time_cond, weight)
        return pred_noise, x_start

    def sampling(self, cond):
        render_count = len(cond)
        shape = (render_count, self.horizon, self.repr_dim)
        samples = self.diffusion.long_ddim_sample(shape, cond, return_init_x=True)
        return samples

    def slice_sampling(
        self,
        cond,
        initx,
        rand_data,
        record,
    ):
        render_count = len(cond)
        shape = (render_count, self.horizon, self.repr_dim)
        outputs = self.diffusion.slice_sampling(
            shape,
            cond,
            initx,
            rand_data,
            record,
        )
        return outputs

    def slice_process(self, samples):
        return self.diffusion.slice_post_process(samples, self.normalizer)

    def slice_render(
        self, full_pose, epoch, render_out, name, sound, stitch, sound_folder, render
    ):
        self.diffusion.slice_render(
            full_pose, epoch, render_out, name, sound, stitch, sound_folder, render
        )
