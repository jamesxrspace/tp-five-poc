# Mock Auth Platform

port: 9453

## Generate private key

```sh
cd one-backend/auth_platform

mkdir .ssh && cd .ssh

ssh-keygen -t rsa -b 2048 -m PEM -f private_key.pem -N ''
```

## Run mock auth server only(in one-backend directory)

```sh
docker compose -f ./auth_platform/deployment/docker-compose.yaml up --env-file .env -d
```

## API url

<http://localhost:9453>

## Create new user

<http://localhost:9453/sign-in>

## Mock user repository file

.data/user.json will be created at the first time for persisting user repository
