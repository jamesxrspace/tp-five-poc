#!/bin/bash

while [[ $# -gt 0 ]]; do
    case $1 in
        --schemafolder)
            JSON_SCHEMA_FOLDER=$2 # json schema folder full path
            shift
            shift
            ;;
        --model)
            MODEL=$2 # dart / cs
            shift
            shift
            ;;
        --output)
            OUTPUT_FILE=$2 # output model full path
            shift
            shift
            ;;
        --namespace)
            NAMESPACE=$2 # cs namespace
            shift
            shift
            ;;
        --help | -h)
            echo "arguments$:
				--schemafolder: json schema folder 
				--model: dart|cs 
				--output: output file name with extension
				--namespace: only for cs model"
            exit
            ;;
        *)
            echo -e "Invalid option -${1}" >&2
            exit 1
            ;;
    esac
done

MODELS=("dart" "cs")
[[ ${MODELS[*]} =~ ${MODEL} ]] || {
    echo "error model: ${MODEL}"
    exit 1
}

if [ "$MODEL" = "cs" ] && [ -z "$NAMESPACE" ]; then
    echo "namespace error"
    exit 1
fi

if [ ! -d "$JSON_SCHEMA_FOLDER" ]; then
    echo "$JSON_SCHEMA_FOLDER is not a folder"
    exit 1
fi

OUTPUT_FOLDER=$(dirname "$OUTPUT_FILE")
mkdir -p "$OUTPUT_FOLDER"

OPTIONS=(-s schema -l "${MODEL}" -o "${OUTPUT_FILE}" --src "${JSON_SCHEMA_FOLDER}" --no-combine-classes)
if [ "$MODEL" == "dart" ]; then
    OPTIONS+=(--use-freezed --required-props --part-name generate_model)
else
    OPTIONS+=(--csharp-version 6 --array-type array --namespace "${NAMESPACE}")
fi

npx quicktype "${OPTIONS[@]}"

echo "${MODEL} codes are generated!"
