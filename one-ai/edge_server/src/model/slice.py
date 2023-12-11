import os

import librosa as lr
import numpy as np
import soundfile as sf

SAMPLE_RATE = 44100


def slice_audio(audio_file, stride, length, out_dir):
    # stride, length in seconds
    audio, sr = lr.load(audio_file, sr=SAMPLE_RATE)
    file_name = os.path.splitext(os.path.basename(audio_file))[0]
    start_idx = 0
    idx = 0
    window = int(length * sr)
    stride_step = int(stride * sr)
    while start_idx <= len(audio) - window:
        audio_slice = audio[start_idx : start_idx + window]
        sf.write(f"{out_dir}/{file_name}_slice{idx}.wav", audio_slice, sr)
        start_idx += stride_step
        idx += 1
    return idx


def slice_audio_stream(audio_file, stride, length):
    # stride, length in seconds
    audio, sr = lr.load(audio_file, sr=SAMPLE_RATE)

    audio_slcie_list = []
    start_idx = 0
    idx = 0
    window = int(length * sr)
    stride_step = int(stride * sr)

    while start_idx <= len(audio) - window:
        audio_slice = audio[start_idx : start_idx + window]
        audio_slcie_list.append(audio_slice)
        start_idx += stride_step
        idx += 1

    return np.array(audio_slcie_list), idx, sr
