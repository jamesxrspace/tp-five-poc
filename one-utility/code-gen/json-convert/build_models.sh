#!/bin/bash

set -xe

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
ONE_CONVERT_FOLDER="${REPO_ROOT}/one-utility/code-gen/json-convert/"
ONE_SCHEMA="${REPO_ROOT}/one-schema/json_schema"
ONE_UNITY="${REPO_ROOT}/one-unity/core/development/common/game-flutter-unity-widget/Runtime/Scripts/generate_model.cs"
ONE_MOBILE="${REPO_ROOT}/one-mobile/flutter_project/lib/generated/freezed/generate_model.dart"
"${ONE_CONVERT_FOLDER}"/json_to_model.sh \
    --model dart \
    --schemafolder "${ONE_SCHEMA}" \
    --output "${ONE_MOBILE}"
"${ONE_CONVERT_FOLDER}"/json_to_model.sh \
    --model cs \
    --namespace TPFive.Model \
    --schemafolder "${ONE_SCHEMA}" \
    --output "${ONE_UNITY}"

sh "${REPO_ROOT}/one-mobile/flutter_project/scripts/generate_freezed_model.sh"

echo "File generate successful! Please check your change."
