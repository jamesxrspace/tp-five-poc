#! /usr/bin/env bash
#
# This is the wrapper script to setup your local python environment
# and install the extra frameworks from flutter-unity-widget into local build environment.

set -e

REPO_ROOT=$($(which git) rev-parse --show-toplevel)
VENV="${REPO_ROOT}/one-utility/.venv"

function install_necessary_package() {
    echo "Installing necessary packages..."

    if ! command -v python3 &>/dev/null; then
        brew install --quiet python@3.11
    fi
}

function setup_venv() {
    echo "Setting up virtual environment..."

    if [ ! -d ".venv" ]; then
        echo "Create new virtual environment..."
        python3 -m venv "${VENV}"
    fi

    "${VENV}/bin/pip" install pbxproj
}

function install_unity_frameworks() {
    echo "Installing unity frameworks..."
    "${VENV}/bin/python" link-unity-frameworks.py
}

function main() {
    pushd "${REPO_ROOT}/one-utility" || exit 1

    install_necessary_package
    setup_venv
    install_unity_frameworks

    popd || exit 1
}

main
