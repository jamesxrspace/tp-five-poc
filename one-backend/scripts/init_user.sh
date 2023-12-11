#!/bin/bash

set -e

REPO_ROOT=$($(which git) rev-parse --show-toplevel)

import() {
    source "${REPO_ROOT}/one-backend/scripts/avatar.config"
    printf "username: %s\nemail: %s\npassword: %s\nnickname: %s\navatar_format: %s\n \
    avatar_asset_file: %s\nhead_file: %s\nbody_file: %s\nfull_body_file: %s\n" \
        "$USERNAME" "$EMAIL" "$PASSWORD" "$NICKNAME" "$AVATAR_FORMAT" "$AVATAR_ASSET" "$HEAD_THUMBNAIL" "$BODY_THUMBNAIL" "$FULL_BODY_THUMBNAIL"
}

confirm() {
    echo "Is the above information correct? (y/n)"
    read -r input

    if [[ $input == 'y' ]]; then
        process
    else
        import
        confirm
    fi
}

process() {
    if [[ ! -e $AVATAR_ASSET || ! -e $HEAD_THUMBNAIL || ! -e $BODY_THUMBNAIL || ! -e $FULL_BODY_THUMBNAIL ]]; then
        echo "file does not exist"
        exit 1
    fi

    createAccount
    loginAccount | loginUser | createAvatar | activateAvatar
}

createAccount() {
    echo "creating account..."
    res=$(curl -s -X POST \
        -H 'Content-Type: application/x-www-form-urlencoded' \
        -d "username=$USERNAME&password=$PASSWORD&email=$EMAIL&nickname=$NICKNAME" \
        http://localhost:9453/sign-up)

    if [[ $res == *"err_code"* ]]; then
        echo "User account already exists. Continue creating avatar? (y/n)"
        read -r input

        if [[ $input == 'y' ]]; then
            return
        else
            exit 0
        fi
    fi
}

loginAccount() {
    token=$(curl -s -X POST \
        -H 'Content-Type: application/x-www-form-urlencoded' \
        -d "grant_type=password&email=$EMAIL&password=$PASSWORD" \
        http://localhost:9453/oidc/token | tr -d '{"}' | tr ',' '\n' |
        grep access_token | awk -F ':' '{print $2}')
    echo "$token"
}

loginUser() {
    token=$(cat -)
    curl -s -X POST \
        -H 'Content-Type: application/json' \
        -H "Authorization: Bearer $token" \
        http://localhost:8090/api/v1/account/login | tr -d '{"}' | tr ',' '\n' |
        grep access_token | awk -F ':' '{print $2}'
    echo "$token"
}

createAvatar() {
    token=$(cat -)
    avatar_id=$(curl -s -X POST \
        -H 'Content-Type: multipart/form-data' \
        -H "Authorization: Bearer $token" \
        -F "type=xr_v2" \
        -F "avatar_format=$AVATAR_FORMAT" \
        -F "avatar_asset=@$AVATAR_ASSET;type=application/zip" -F "avatar_head=@$HEAD_THUMBNAIL" -F "avatar_upper_body=@$BODY_THUMBNAIL" -F "avatar_full_body=@$FULL_BODY_THUMBNAIL" \
        http://localhost:8090/api/v1/avatar/save | tr -d '{"}' | tr ',' '\n' |
        grep avatar_id | awk -F ':' '{print $2}')
    echo "$avatar_id $token"
}

activateAvatar() {
    input=$(cat -)
    avatar_id=$(echo "$input" | awk '{print $1}')
    token=$(echo "$input" | awk '{print $2}')
    echo "activating avatar $avatar_id"
    curl -s -X POST \
        -H "Authorization: Bearer $token" \
        "http://localhost:8090/api/v1/avatar/activate/$avatar_id"
}

import
confirm
