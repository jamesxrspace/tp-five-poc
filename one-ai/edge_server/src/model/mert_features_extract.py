import librosa as lr
import numpy as np
import torch
import torchaudio.transforms as T
from transformers import AutoModel, Wav2Vec2FeatureExtractor

MODEL = "m-a-p/MERT-v1-95M"
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
model = AutoModel.from_pretrained(MODEL, trust_remote_code=True).to(device)
processor = Wav2Vec2FeatureExtractor.from_pretrained(MODEL, trust_remote_code=True)

def downsample(representation):
    resampled_reps = lr.resample(
        np.asfortranarray(representation.T),
        orig_sr=75,
        target_sr=30,
        res_type="fft",
    ).T

    return resampled_reps


def extract_stream(audio, sampling_rate):
    resample_rate = processor.sampling_rate

    # make sure the sample_rate aligned
    resampler = T.Resample(
        sampling_rate, resample_rate
    ) if resample_rate != sampling_rate else lambda x: x

    # audio file is decoded on the fly
    input_audio = resampler(torch.from_numpy(audio).float())

    inputs = processor(input_audio, sampling_rate=resample_rate, return_tensors="pt")
    inputs.to(device)
    with torch.no_grad():
        outputs = model(**inputs, output_hidden_states=True)

    all_layer_hidden_states = torch.stack(outputs.hidden_states).squeeze()
    reps = all_layer_hidden_states[-1].cpu().numpy()

    # downsample 75 to 30
    downsample_reps = downsample(reps)

    return downsample_reps
