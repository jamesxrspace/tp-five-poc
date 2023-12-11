# One Backend

## Prerequest

* [Docker](https://docs.docker.com/get-docker/)

## Start

It's easy to start a local server with docker-compose.

```bash
./scripts/local.sh
```

If you want to run specific service, you can use the following command.

```bash
./scripts/local.sh <gameserver|mongodb|mockauthservice>
```

## Game server development

* Start a local server with docker-compose.

```bash
./scripts/server_developing.sh
```

* If you are not able to connect to the local server,  update `./server/config.local.yaml`.

## All-in-one local DEV services

you can start all-in-one backend services with single command `make run` and stop
by `make stop`. The all-in-one backend service includes two parts: backend service
and other third-party services.

The all-in-one local DEV environment based on docker-compose. The follwoing
are the support third-party services:

| Service        | version  | Description                              |
| -------------- | -------- | ---------------------------------------- |
| MogoDB         | 6.0.8    | document-oriented database               |
| mogodb-express | 0.54.0   | web-based MongoDB admin interface        |
| localstack     | 2.2.0    | A fully functional local AWS cloud stack |
| redis          | 7.0.12   | the in-memory key-value storage          |

**NOTE** The version of services should align with the production environment.

## Local development

How to setup your local development environment (with mock data)

* setup your docker environment with `brew install docker`.
* run `make build` to build the docker images (by docker-compose).
* run `make run` to start the docker containers (by docker-compose).

### MinIO

You can run the S3 compatible storage service on your local environment based on
MinIO, which is the (almost) fully function simulation of AWS S3.

In case you want to access the MinIO service directly, you should create a new
access key and secret key for your local environment.

1. access to <http://localhost:9090> by browser with the USERNAME and PASSWORD.
2. create access key on <http://localhost:9090/access-keys>.
3. click _Create_ and _Download for import_ to get the credentials.
4. setup the credential on your application.
5. To enable one-backend/server access minio, modify config.example.yaml

```yaml
aws:
  access_key: ${copy accessKey you get from step 3 here}
  secret_key: ${copy secretKey you get from step 3 here}
  endpoint: http://localhost:9000
  region: ap-southeast-1
```

You can set up your AWS cli by `aws configure` and type the credential you created,
and test by the command `aws --endpoint-url http://localhost:9000 s3 ls`.

**NOTE** The persistent storage of MinIO is located at `./data/minio`.

### (DEPRECATED) AWS Localstack

> **NOTE** Localstack does not provides the persistent storage for free-tier users.
Use MinIO instead.

You can run the AWS service on your local environment based on localstack, which
is the (almost) fully function simulation of AWS cloud stack.

For example, you can create S3 bucket and upload file by aws command line tools

```bash
#! /usr/bin/env sh


## create a new s3 bucket
aws --endpoint http://localhost:4566 s3 mb s3://test-bucket

## upload a file to the bucke
aws --endpoint http://localhost:4566 s3 cp ./test.txt s3://test-bucket
```

In Go project with AWS [SDK][0] you can access localstack with specified the _Endpoint_:

```go
    package main

    import (
        "fmt"
        "log"

        "github.com/aws/aws-sdk-go/aws"
        "github.com/aws/aws-sdk-go/aws/session"
        "github.com/aws/aws-sdk-go/service/s3"
    )

    func main() {
        endpoint := "http://127.0.0.1:4566"
        region := "us-east-1"

        sess, err := session.NewSession(&aws.Config{
            Endpoint: &endpoint,
            Region:   &region,
        })

        if err != nil {
            log.Fatalf("failed to create AWS session %v", err)
            return
        }

        svc := s3.New(sess)
        input := &s3.ListBucketsInput{}
        result, err := svc.ListBuckets(input)
        if err != nil {
            log.Fatalf("failed to list buckets, %v", err)
            return
        }

        fmt.Println("Buckets:")
        for _, bucket := range result.Buckets {
            fmt.Printf("* %s created on %s\n", aws.StringValue(bucket.Name), aws.TimeValue(bucket.CreationDate))
        }
    }
```

### ELK stack

In case you need to setup your ELK stack on your local environment, you can use
the docker-compose file `docker-compose.elk.yml` to start the ELK stack. It provides
the following services:

| Service       | version | Description                             |
| ------------- | ------- | --------------------------------------- |
| elasticsearch | 8.9.0   | distributed search and analytics engine |
| kibana        | 8.9.0   | data visualization                      |
| fluent-bit    | 2.1.8   | log data collector                      |

When first time to start the ELK stack, you should connect to the Kibana service
and set up the elasticsearch certificate by:

1. access <http://localhost:5601> (it may take seconds).
2. get the passcode on docker log `XXX-XXX`.

### Alternative

If you want to access and control your local environment directly, you have the
following options.

1. execute the docker-compose commands directly, e.g. `docker-compose up -d` to
   start the containers in the background.
2. install the mongo and other services locally and start them manually.

[0]: https://docs.aws.amazon.com/sdk-for-go/api/aws/
