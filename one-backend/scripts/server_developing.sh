#!/bin/bash
export IMAGE_VERSION
# IMAGE_VERSION=$(git rev-parse --short HEAD)
IMAGE_VERSION=latest
REPO_ROOT=$($(which git) rev-parse --show-toplevel)
ONE_BACKEND_FOLDER="${REPO_ROOT}/one-backend"

# export all variables defined in .env.
set -a
source "${ONE_BACKEND_FOLDER}/.env"
set +a

PWD="${ONE_BACKEND_FOLDER}" docker compose \
    -f "${ONE_BACKEND_FOLDER}"/auth_platform/deployment/docker-compose.yaml \
    -f "${ONE_BACKEND_FOLDER}"/docker-compose.yaml \
    up --build "$@"
