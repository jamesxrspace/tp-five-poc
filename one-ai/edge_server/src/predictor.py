import io
import os

import flask
import numpy as np
import torch
from model.EDGE_sampling import EDGESampling
from model.mert_features_extract import extract_stream as mert_extract
from model.slice import slice_audio_stream

FEATURE_TYPE = os.environ.get("FEATURE_TYPE", "MERT")
CHECKPOINT = os.environ.get("CHECKPOINT", "train-2000.pt")
SAMPLING_TIMESTEPS = int(os.environ.get("SAMPLING_TIMESTEPS", "50"))
SLICE_SEC = int(os.environ.get("SLICE_SEC", "5"))
OVERLAP_SEC = float(os.environ.get("OVERLAP_SEC", "2.5"))
MODEL_PATH = "/opt/ml/model"


class MotionService(object):
    _model = None

    @staticmethod
    def warm_up(model):
        # warnup at first time
        x = np.random.randn(5, model.horizon, model.repr_dim).astype(np.float32)
        rand_noises = np.random.randn(
            model.diffusion.sampling_timesteps - 1, 5, model.horizon, model.repr_dim
        ).astype(np.float32)
        cond = np.random.randn(5, model.horizon, model.feature_dim).astype(np.float32)
        model.slice_sampling(
            torch.from_numpy(cond),
            torch.from_numpy(x),
            torch.from_numpy(rand_noises),
            None,
        )

    @classmethod
    def get_model(cls):
        # Get the model object for this instance, loading it if it's not already loaded.
        if cls._model is None:
            device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
            checkpoint = os.path.join(MODEL_PATH, CHECKPOINT)
            model = EDGESampling(
                FEATURE_TYPE,
                checkpoint,
                sampling_timesteps=SAMPLING_TIMESTEPS,
                device=device,
            ).to(device)
            # Set model to eval mode
            model.eval()
            cls.warm_up(model)
            cls._model = model
        return cls._model

    @staticmethod
    def generate_motion(model, group_size, cond_list):
        x = np.random.randn(group_size, model.horizon, model.repr_dim).astype(
            np.float32
        )
        rand_noises = np.random.randn(
            model.diffusion.sampling_timesteps - 1,
            group_size,
            model.horizon,
            model.repr_dim,
        ).astype(np.float32)

        samples, _ = model.slice_sampling(
            torch.from_numpy(np.array(cond_list)),
            torch.from_numpy(x),
            torch.from_numpy(rand_noises),
            None,
        )
        slice_samples = samples.cpu().detach().numpy().reshape(-1)
        return slice_samples

    @staticmethod
    def prepare_input(wav_file):
        slice_list, _, slice_sr = slice_audio_stream(wav_file, OVERLAP_SEC, SLICE_SEC)
        return [mert_extract(audio_slice, slice_sr) for audio_slice in slice_list]

    @classmethod
    def predict(cls, input):
        cond_list = cls.prepare_input(input)
        return cls.generate_motion(cls.get_model(), len(cond_list), cond_list)


app = flask.Flask(__name__)


@app.route("/ping", methods=["GET"])
def ping():
    health = MotionService.get_model() is not None

    status = 200 if health else 404
    return flask.Response(response="OK", status=status, mimetype="application/json")


@app.route("/invocations", methods=["POST"])
def invocations():
    data = flask.request.data
    filelike = io.BytesIO(data)
    motions = MotionService.predict(filelike)
    return flask.Response(
        response=motions.tobytes(), status=200, mimetype="application/octet-stream"
    )
