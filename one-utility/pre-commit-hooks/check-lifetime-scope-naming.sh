#!/bin/bash

# Get a list of Unity files that were staged for commit
UNITY_FILES=$(git diff --cached --name-only -- '*.unity')

# If there are no Unity files, exit successfully
if [ -z "$UNITY_FILES" ]; then
    exit 0
fi

# Ensure following for loop would break by newline, instead of space.
IFS=$'\n'
ORIGINAL_IFS=$IFS
trap '$(IFS=$ORIGINAL_IFS)' EXIT

for FILE in $UNITY_FILES; do
    if grep -E "m_Name: Lifetime\s?Scope$" "$FILE"; then
        echo "Error: Unity file '$FILE' contains the forbidden string 'LifetimeScope' or 'Lifetime Scope'."
        exit 1
    fi
done

exit 0
