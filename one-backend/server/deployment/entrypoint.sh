#! /bin/sh
set -e

ACTION="${ACTION:-serve}"
CONFIG_FILE="config.yaml"

if [ ! -f "$CONFIG_FILE" ]; then
    # substitute environment variables in config file
    # TODO maybe pass the config file as an argument to the container
    envsubst </app/config.template.yaml >"${CONFIG_FILE}"
fi

exec /app/server --action "${ACTION}" --config "${CONFIG_FILE}"
