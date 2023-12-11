#!/bin/bash

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
ONE_BACKEND_FOLDER="${REPO_ROOT}/one-backend"

# export all variables defined in .env.
set -a
source "${ONE_BACKEND_FOLDER}/.env"
set +a

# Put the root compose file in the last, so that we can overwrite service
# config to connect with 3rd party services.
PWD="${ONE_BACKEND_FOLDER}" docker compose \
    -f "${ONE_BACKEND_FOLDER}"/auth_platform/deployment/docker-compose.yaml \
    -f "${ONE_BACKEND_FOLDER}"/server/deployment/docker-compose.yaml \
    -f "${ONE_BACKEND_FOLDER}"/docker-compose.yaml \
    up --build "$@"
