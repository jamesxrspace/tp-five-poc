#!/bin/bash
set -o pipefail

BUCKET_NAME="one-app-develop-s3-cms"
PREFIX="artifact/"

DATE=${1:-$(date +'%Y/%m/%d')}

APK_FILE=$(aws s3 ls "s3://$BUCKET_NAME/$PREFIX$DATE" --recursive | grep "apk/dev/app-dev-release" | sort | tail -n 1 | awk '{print $4}')

APK_FILE_NAME=$(basename "$APK_FILE")

if [ -n "$APK_FILE" ]; then
    aws s3 cp "s3://$BUCKET_NAME/$APK_FILE" "$APK_FILE_NAME"
    adb shell pm list packages | grep com\.xrspace\. | cut -d ":" -f 2 | xargs -n 1 adb uninstall
    echo "Start installing APK file: $APK_FILE_NAME"
    if adb install "$APK_FILE_NAME"; then
        echo "APK installed successfully."
    else
        echo "Failed to install the APK using adb."
        exit 1
    fi
else
    echo "No APK file found for the specified date: $DATE"
    exit 1
fi
