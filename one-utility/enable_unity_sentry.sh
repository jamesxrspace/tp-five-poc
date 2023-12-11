#! /usr/bin/env bash
#
# This script enables Sentry for Unity projects.

REPO_ROOT=$($(which git) rev-parse --show-toplevel)

function enable_unity_sentry() {
    FILE="${REPO_ROOT}/one-unity/unity-project/development/complete-unity/Assets/Resources/Sentry/SentryOptions.asset"

    case $(uname) in
        Darwin)
            sed -i '' 's/<Enabled>k__BackingField: 0/<Enabled>k__BackingField: 1/g' "$FILE"
            ;;
        *)
            sed -i 's/<Enabled>k__BackingField: 0/<Enabled>k__BackingField: 1/g' "$FILE"
            ;;
    esac
}

function enable_android_sentry() {
    pushd "${REPO_ROOT}/one-mobile/flutter_project/android" || exit 1

    find . -name AndroidManifest.xml -type f | while read -r line; do
        case $(uname) in
            Darwin)
                sed -i '' '/io.sentry.auto-init/d' "$line"
                ;;
            *)
                sed -i '/io.sentry.auto-init/d' "$line"
                ;;
        esac
    done

    popd || exit 1
}

function main() {
    enable_unity_sentry
    enable_android_sentry
}

main
