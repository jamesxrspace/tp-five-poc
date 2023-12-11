######################################################
## Builder Image                                    ##
######################################################
FROM public.ecr.aws/docker/library/golang:1.21-alpine3.18 AS builder

ARG VERSION="local-build"
ENV CGO_ENABLED=0

WORKDIR /app

## copy and download dependencies
COPY go.mod go.sum ./
RUN go mod download


COPY . .

# hadolint ignore=DL3018
RUN <<__EOF__
    tr "\r\n" "\n" < deployment/entrypoint.sh > entrypoint.sh
    chmod +x entrypoint.sh
    go build -ldflags "-w -s -X main.version=${VERSION}" -o server ./cmd/gameserver
__EOF__


######################################################
## Final Image                                      ##
######################################################
FROM public.ecr.aws/docker/library/alpine:3.18

EXPOSE 8090

RUN <<__EOF__
    apk add --no-cache envsubst
    # -s SHELL Login shell
    # -u UID   User id
    # -D       Don't assign a password
    adduser -u 1024 -s /bin/sh -D xrspace
__EOF__

WORKDIR /app

COPY --from=builder /app/server /app
COPY --from=builder /app/config.template.yaml /app
COPY --from=builder /app/entrypoint.sh /app

## [TFI-95] downgrade the permission
USER xrspace
WORKDIR /home/xrspace

ENTRYPOINT ["/app/entrypoint.sh"]
