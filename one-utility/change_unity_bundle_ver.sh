#! /usr/bin/env bash
#
# Change the Unity bundle identifier.
set -e

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
PUBSPECT="${REPO_ROOT}/one-mobile/flutter_project/pubspec.yaml"
BUILD_NUMBER=$(grep 'version: ' "$PUBSPECT" | cut -d ' ' -f 2)

function main() {
    BUILD_NUMBER="$1"

    PROJ_SETTINGS="${REPO_ROOT}/one-unity/unity-project/development/complete-unity/ProjectSettings/ProjectSettings.asset"

    case $(uname) in
        Darwin)
            sed -i '' "s/bundleVersion: .*/bundleVersion: ${BUILD_NUMBER}/g" "${PROJ_SETTINGS}"
            ;;
        *)
            sed -i "s/bundleVersion: .*/bundleVersion: ${BUILD_NUMBER}/g" "${PROJ_SETTINGS}"
            ;;
    esac
}

main "${1:-$BUILD_NUMBER}"
