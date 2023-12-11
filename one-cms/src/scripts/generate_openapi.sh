#!/bin/bash

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
CMS_ROOT="$REPO_ROOT/one-cms"
SCHEMA_DOC_FOLDER="$REPO_ROOT/one-schema/openapi-doc/game-server/documents"

LOCAL_SCHEMA_DOC_FOLDER="${CMS_ROOT}/src/scripts/doc"
mkdir "${LOCAL_SCHEMA_DOC_FOLDER}"
cp -r "${SCHEMA_DOC_FOLDER}" "${LOCAL_SCHEMA_DOC_FOLDER}"

# build _merge_api.yaml
cd "${CMS_ROOT}/src/scripts/" && npx openapi-merge-cli
# build typescript models from _merge_api.yaml
openapi-generator generate -i "${LOCAL_SCHEMA_DOC_FOLDER}/documents/_merge_api.yaml" -g "typescript-fetch" -o "${CMS_ROOT}/src/api/openapi"

rm -rf "${LOCAL_SCHEMA_DOC_FOLDER}"
