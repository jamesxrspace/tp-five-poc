# ROOM System

This module is responsible for managing the rooms and the players in them.
It also handles the authenticated users and the users that are not authenticated.

[Miro board](https://miro.com/app/board/uXjVMzBljzc=/)

## how to run photon fusion custom auth server in local

Go to the room system folder and run the following command:

```sh
cd one-backend/server/modules/room
go run main.go
````

use following command to test the photon fusion custom auth server

```sh
# for success case
curl "localhost:8090/fusion_custom_auth?token=1234"

# for failure case
curl "localhost:8090/fusion_custom_auth"
```

## how to test photon fusion in local

We use ngrok to expose local server to internet. Please follow the steps below
to install ngrok.

* install ngrok

```sh
#window
choco install ngrok

#macos
brew install ngrok
```

* run server

```sh
cd one-backend/server
go run main.go --action serve 
```

* run ngrok, expose port 8090

```sh
ngrok http 8090
```

It will show an url like this: <https://xxxxxx.ngrok-free.app>

Put this url in the photon fusion dashboard.

There is a field called "Authentication URL", put the url in there.

Remember to add `/fusion_custom_auth` at the end of the url.

Uncheck the "Allow anonymous clients to connect" checkbox.

Once you have done this, you can test the photon fusion in the photon dashboard.

## Registry custom photon fusion for local testing

If you want to test photon fusion along and avoid different photon fusion
callback url, you can register a new photon fusion in the photon dashboard.

You can register a new photon fusion here: <https://dashboard.photonengine.com>

## How to enable fusion custom auth

please see the PR for details:
[PR-40](https://github.com/XRSPACE-Inc/tp-five/pull/40#issue-1842828836)

Custom Auth disabled by default now, you can enable it by check "Custom Auth"
in the "Room Settings" in unity.

![img](https://private-user-images.githubusercontent.com/84462403/259941386-540f8f29-04fa-45ac-961c-1383b84240ab.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTEiLCJleHAiOjE2OTE3Mzc1NzgsIm5iZiI6MTY5MTczNzI3OCwicGF0aCI6Ii84NDQ2MjQwMy8yNTk5NDEzODYtNTQwZjhmMjktMDRmYS00NWFjLTk2MWMtMTM4M2I4NDI0MGFiLnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFJV05KWUFYNENTVkVINTNBJTJGMjAyMzA4MTElMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjMwODExVDA3MDExOFomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPTM5MDJjN2YyZTNhNmJiYWZhY2E0MzhjYmI4MjQ1NzkwN2U1MzdjNjgxNWU2MmY5ZTQyZjViOTgyNmFmMjI4ZDQmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0JmFjdG9yX2lkPTAma2V5X2lkPTAmcmVwb19pZD0wIn0.AaVV6GX08PJfOC_80coiz5vuzFPY3JzDaZ2q2PoRfxM)
![img](https://private-user-images.githubusercontent.com/84462403/259941311-9e0b1bf6-781f-4fd2-becf-1233ec7d21b3.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTEiLCJleHAiOjE2OTE3Mzc1NzgsIm5iZiI6MTY5MTczNzI3OCwicGF0aCI6Ii84NDQ2MjQwMy8yNTk5NDEzMTEtOWUwYjFiZjYtNzgxZi00ZmQyLWJlY2YtMTIzM2VjN2QyMWIzLnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFJV05KWUFYNENTVkVINTNBJTJGMjAyMzA4MTElMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjMwODExVDA3MDExOFomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPTg0ODA5YjdkM2Q1NTY4MjhlZmFhODcyNDNjM2M3YjY4ZWZhZWY0ODZlMWVhNGQyNzZlMTc1MjIyNjEyNDBiMDQmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0JmFjdG9yX2lkPTAma2V5X2lkPTAmcmVwb19pZD0wIn0.pQRJUCaMMkcyRq70YGQNdNH4H_TnhXudzJNAWgbbTKA)
