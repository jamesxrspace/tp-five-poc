#!/bin/bash

mkdir -p "auth_platform/fake_secret"
cd auth_platform/fake_secret || exit 1

ssh-keygen -t rsa -b 2048 -m PEM -f private_key.pem
openssl rsa -in private_key.pem -pubout -outform PEM -out public_key.pem
