#!/bin/bash

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
ONE_BACKEND_FOLDER="${REPO_ROOT}/one-backend"

# export all variables defined in .env.
set -a
source "${ONE_BACKEND_FOLDER}/.env"
set +a

# Put the root compose file in the last, so that we can overwrite service
# config to connect with 3rd party services.
docker_run() {
    PWD="${ONE_BACKEND_FOLDER}" docker compose \
        -f "${ONE_BACKEND_FOLDER}"/auth_platform/deployment/docker-compose.yaml \
        -f "${ONE_BACKEND_FOLDER}"/server/deployment/docker-compose.yaml \
        -f "${ONE_BACKEND_FOLDER}"/docker-compose.local_test.yaml \
        -f "${ONE_BACKEND_FOLDER}"/docker-compose.yaml \
        "$@"
}

# Define a function to run the tests
run_tests() {
    cd e2e/k6-ts-test && pnpm run test
}

# Define a function to clean up after the tests
cleanup() {
    docker_run down
    docker_run rm
    cd ../../ && rm -rf .test-data/
}

startup() {

    # Start the server
    docker_run up -d --build

    echo "Waiting for server to be ready..."
    timeout=30
    while [ $timeout -gt 0 ]; do
        # Check if the server is listening on the expected port
        if nc -z localhost 8090; then
            echo "Server is ready!"
            break
        fi

        echo -ne "\rServer not ready, $timeout seconds left..."
        sleep 1
        timeout=$((timeout - 1))
    done

    if [ $timeout -eq 0 ]; then
        echo "Timeout waiting for server to be ready"
        exit 1
    fi
}

startup

# Set up a trap to ensure that the cleanup function is executed even if an error occurs
trap cleanup EXIT

run_tests
