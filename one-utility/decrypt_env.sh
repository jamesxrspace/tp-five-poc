#!/bin/bash
# Usage example:
# $ decrypt_env.sh local # to decrypt local env files
# $ decrypt_env.sh dev # to decrypt dev env files

REPO_ROOT=$($(which git) rev-parse --show-toplevel)

validate_env() {
    envs=(local dev qa prod)
    if [[ ! "${envs[*]}" =~ ${1} ]]; then
        echo "local"
        exit 0
    fi
    echo "${1}"
}

env=$(validate_env "${1}")
echo "env=${env}"

function decrypt_env_file() {
    SOURCE_ENCRYPTED_FILE="${1}/.${env}.env"
    TARGET_ENCRYPTED_FILE="${2}/.env"

    echo The encryped file:
    echo "${SOURCE_ENCRYPTED_FILE}"
    echo The decrypted file:
    echo "${TARGET_ENCRYPTED_FILE}"

    git checkout "${SOURCE_ENCRYPTED_FILE}"
    if ! sops -d "${SOURCE_ENCRYPTED_FILE}" >"${TARGET_ENCRYPTED_FILE}"; then
        echo "Can not decrypt env file"
        exit 1
    fi
}

function decrypt_env_files() {
    # decrypt auth env file to unity project
    AUTH_ENV_SOURCE_FOLDER=$REPO_ROOT/one-native
    AUTH_ENV_UNITY_TARGET_FOLDER=$REPO_ROOT/one-unity/unity-project/development/complete-unity
    decrypt_env_file "${AUTH_ENV_SOURCE_FOLDER}" "${AUTH_ENV_UNITY_TARGET_FOLDER}"

    # decrypt auth env file for flutter project
    AUTH_ENV_FLUTTER_TARGET_FOLDER=$REPO_ROOT/one-mobile/flutter_project
    decrypt_env_file "${AUTH_ENV_SOURCE_FOLDER}" "${AUTH_ENV_FLUTTER_TARGET_FOLDER}"

    # decrypt cms env file
    CMS_FOLDER=$REPO_ROOT/one-cms
    decrypt_env_file "${CMS_FOLDER}" "${CMS_FOLDER}"

    # decrypt backend env file
    BACKEND_FOLDER=$REPO_ROOT/one-backend
    if ! decrypt_env_file "${BACKEND_FOLDER}" "${BACKEND_FOLDER}"; then
        echo "Can not decrypt env files correctly, please refer to:"
        echo "https://xrspace.atlassian.net/wiki/spaces/TF/pages/2254897163/Confidential+Data+Management"

        return
    fi

    echo "Decrypt correctly"
}

decrypt_env_files
