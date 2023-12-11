#!/bin/bash

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
ONE_BACKEND_FOLDER="${REPO_ROOT}/one-backend"

# Put the root compose file in the last, so that we can overwrite service
# config to connect with 3rd party services.
PWD="${ONE_BACKEND_FOLDER}" docker compose \
    -f "${ONE_BACKEND_FOLDER}"/docker-compose.tempo.yaml \
    up "$@"
