######################################################
## Builder Image                                    ##
######################################################
FROM public.ecr.aws/docker/library/golang:1.21-alpine3.18 AS builder

ARG VERSION="local-build"
ENV CGO_ENABLED=0

# hadolint ignore=DL3018
RUN <<__EOF__
    apk update
    apk add --no-cache openssh-keygen
__EOF__

# generate jwt secret
WORKDIR /app/.ssh
RUN ssh-keygen -t rsa -b 2048 -m PEM -f private_key.pem

## copy and download dependencies
WORKDIR /app
COPY go.mod go.sum ./
RUN go mod download
COPY . .

RUN go build -ldflags "-w -s -X main.version=${VERSION}" -o server

######################################################
## Final Image                                      ##
######################################################
FROM public.ecr.aws/docker/library/alpine:3.18

EXPOSE 9453

HEALTHCHECK --interval=10s --timeout=3s --start-period=1s --retries=10 \
    CMD wget --no-verbose --tries=1 http://localhost:9453/health || exit 1

WORKDIR /app
COPY --from=builder ./app/server /app
COPY --from=builder ./app/.ssh /app/.ssh

ENTRYPOINT ["./server"]
