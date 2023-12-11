#! /usr/bin/env bash
#
# This is the workaround script to remove firebase-related since it block the iOS build

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
UNITY_PROJ="${REPO_ROOT}/one-unity/unity-project/development/complete-unity"
REMOVE_PKG=(
    # MixedReality
    "com.microsoft.mixedreality.openxr"
)

main() {
    PACKAGE_TGZ="Packages/MixedReality/com.microsoft.mixedreality.openxr-1.8.1.tgz"
    PACKAGE_LOCK="Packages/packages-lock.json"
    MANIFEST_JSON="Packages/manifest.json"

    pushd "$UNITY_PROJ" || exit 1

    # remove the package file
    rm -f "${PACKAGE_TGZ}" || true

    for pkg in "${REMOVE_PKG[@]}"; do
        # remove the package-lock.json entry for mixedreality.openxr
        jq "del(.dependencies.\"$pkg\")" "${PACKAGE_LOCK}" >"${PACKAGE_LOCK}.tmp"
        mv "${PACKAGE_LOCK}.tmp" "${PACKAGE_LOCK}"

        # remove the manifest.json entry for mixedreality.openxr
        jq "del(.dependencies.\"$pkg\")" "${MANIFEST_JSON}" >"${MANIFEST_JSON}.tmp"
        mv "${MANIFEST_JSON}.tmp" "${MANIFEST_JSON}"
    done
}

main
