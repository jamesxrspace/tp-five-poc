# Server

port 8090

## Environment

```sh
cp one-backend/server/config.template.yaml one-backend/server/config.local.yaml  
```

## Create DB Index

```sh
cd one-backend/server

go run main.go --action create-index
```

## Run Server

```sh
cd one-backend/server

go run cmd/testapp/main.go
```

## End to End

1. Install the extension `rest-client` in VSCode. <https://marketplace.visualstudio.com/items?itemName=humao.rest-client>

2. Open any file in `one-backend/server/e2e`

3. Click `Send Request` at the top of each API request
section and see the result on the right side of the screen.
