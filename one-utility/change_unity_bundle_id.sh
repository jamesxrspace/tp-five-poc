#! /usr/bin/env bash
#
# Change the Unity bundle identifier.
set -e

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
BASE_BUNDLE_ID="com.xrspace.tpfive"

function main() {
    BUNDLE_ID="${1}"

    PROJ_SETTINGS="${REPO_ROOT}/one-unity/unity-project/development/complete-unity/ProjectSettings/ProjectSettings.asset"
    PUBSPECT="${REPO_ROOT}/one-mobile/flutter_project/pubspec.yaml"
    BUNDLE_SETTINGS="${REPO_ROOT}/one-mobile/flutter_project/android/app/build.gradle"

    case $(uname) in
        Darwin)
            sed -i '' "s/\(Android: ${BASE_BUNDLE_ID}\).*$/\1.${BUNDLE_ID}/g" "${PROJ_SETTINGS}"
            sed -i '' "s/\(applicationId: ${BASE_BUNDLE_ID}\).*$/\1.${BUNDLE_ID}/g" "${PUBSPECT}"
            sed -i '' "s/${BASE_BUNDLE_ID}/${BASE_BUNDLE_ID}.${BUNDLE_ID}/g" "${BUNDLE_SETTINGS}"
            ;;
        *)
            sed -i "s/\(Android: ${BASE_BUNDLE_ID}\).*$/\1.${BUNDLE_ID}/g" "${PROJ_SETTINGS}"
            sed -i "s/\(applicationId: ${BASE_BUNDLE_ID}\).*$/\1.${BUNDLE_ID}/g" "${PUBSPECT}"
            sed -i "s/${BASE_BUNDLE_ID}/${BASE_BUNDLE_ID}.${BUNDLE_ID}/g" "${BUNDLE_SETTINGS}"
            ;;
    esac
}

main "${1:-local}"
