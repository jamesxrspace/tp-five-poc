#!/bin/bash
# Build XrAuth porting aar file and put it to one-unity

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
ANDROID_PORTING_PROJECT=$REPO_ROOT/one-native/xrauth-android
printf "Repo root: %s\n" "${REPO_ROOT}"
printf "Source android folder: %s\n" "${ANDROID_PORTING_PROJECT}"

function publish_aar() {
    cd "${ANDROID_PORTING_PROJECT}" || exit 1
    ./gradlew :xrauth:cleanBuildPublish --max-workers=1

    # shellcheck disable=SC2181
    if [[ $? -ne 0 ]]; then
        echo "Can not build aar file"
        exit 1
    fi
}

{
    publish_aar
} || {
    echo "Can not build aar file correctly"
}
