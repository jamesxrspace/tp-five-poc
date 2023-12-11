#!/bin/bash

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
FLUTTER_ROOT="$REPO_ROOT/one-mobile/flutter_project"
SCHEMA_DOC_FOLDER="$REPO_ROOT/one-schema/openapi-doc/game-server/documents"

LOCAL_SCHEMA_DOC_FOLDER="${FLUTTER_ROOT}/scripts/doc"
mkdir "${LOCAL_SCHEMA_DOC_FOLDER}"
cp -r "${SCHEMA_DOC_FOLDER}" "${LOCAL_SCHEMA_DOC_FOLDER}"

# build _merge_api.yaml
cd "${FLUTTER_ROOT}/scripts/" && npx openapi-merge-cli
rm -rf "${FLUTTER_ROOT}/packages/tpfive_game_server_api_client/lib"
# build dart modals from _merge_api.yaml
cd "${FLUTTER_ROOT}" &&
    flutter clean &&
    flutter pub get &&
    dart run build_runner build --delete-conflicting-outputs

rm -rf "${LOCAL_SCHEMA_DOC_FOLDER}"
