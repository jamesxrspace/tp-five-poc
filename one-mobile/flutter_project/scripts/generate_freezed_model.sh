#!/bin/bash

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
FLUTTER_ROOT="$REPO_ROOT/one-mobile/flutter_project"
cd "${FLUTTER_ROOT}" || exit
dart run build_runner build --delete-conflicting-outputs --build-filter="lib/generated/freezed/*.dart"
