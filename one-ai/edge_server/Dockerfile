# syntax=docker/dockerfile:1.3-labs
FROM pytorch/pytorch:1.13.0-cuda11.6-cudnn8-devel

RUN <<__EOF__
apt-get update
apt-get upgrade -y
apt-get install git=1:2.17.1-1ubuntu0.18 libsndfile1=1.0.28-4ubuntu0.18.04.2 nginx=1.14.0-0ubuntu1.11 curl=7.58.0-2ubuntu3.24 -y --no-install-recommends
apt-get clean
rm -rf /var/lib/apt/lists/*
__EOF__

RUN <<__EOF__
curl -L -o  pytorch3D.tar.bz2 https://anaconda.org/pytorch3d/pytorch3d/0.7.4/download/linux-64/pytorch3d-0.7.4-py39_cu116_pyt1130.tar.bz2
conda install pytorch3D.tar.bz2
__EOF__

WORKDIR /opt/program

COPY requirements.txt ./requirements.txt
RUN pip install --no-cache-dir -r ./requirements.txt --extra-index-url https://download.pytorch.org/whl/cu116

ENV PATH="/opt/program:${PATH}"

COPY src .
