import torch

from .scaler import MinMaxScaler


class Normalizer:
    def __init__(self, data):
        flat = data.reshape(-1, data.shape[-1])
        self.scaler = MinMaxScaler((-1, 1), clip=True)
        self.scaler.fit(flat)

    def normalize(self, x):
        batch, seq, ch = x.shape
        x = x.reshape(-1, ch)
        return self.scaler.transform(x).reshape((batch, seq, ch))

    def unnormalize(self, x):
        batch, seq, ch = x.shape
        x = x.reshape(-1, ch)
        x = torch.clip(x, -1, 1)  # clip to force compatibility
        return self.scaler.inverse_transform(x).reshape((batch, seq, ch))
