SRC := $(shell find . -name '*.go')
BIN := bootstrap

.PHONY: all clean test run build remove help

all: 			# default action

clean:			# clean-up environment
	rm -f $(BIN)

test:			# run test
	gofmt -s -w $(SRC)
	awslocal cloudformation list-stacks || (echo "run localstack first" && exit 1)
	go test ./...

run:			# run binary on local
	go run main.go

build: $(BIN)  	# build the binary/library

help:			# show this message
	@printf "Usage: make [OPTION]\n"
	@printf "\n"
	@perl -nle 'print $$& if m{^[\w-]+:.*?#.*$$}' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?#"} {printf "    %-18s %s\n", $$1, $$2}'

$(BIN): $(SRC)
	GOOS=linux GOARCH=amd64 go build -tags lambda.norpc -o $(BIN) *.go


# AWS Lambda related
# ==================

AWS      := aws
ZIPFILE  := $(BIN).zip
FUNCNAME := license-server
IAMROLE  := arn:aws:iam::123456789012:role/lambda-ex

.PHONY: lambda-deploy lambda-update lambda-remove

$(ZIPFILE): $(BIN)
	zip $@ $<

deploy: $(ZIPFILE)		# deploy lambda to AWS/Localstack
	$(AWS) lambda create-function --function-name $(FUNCNAME) \
		--runtime provided.al2 --handler $(BIN) \
		--architectures x86_64 \
		--zip-file fileb://$(ZIPFILE) \
		--role $(IAMROLE)

redeploy: $(ZIPFILE)	# update lambda to AWS/Localstack
	$(AWS) lambda update-function-code --function-name $(FUNCNAME) \
		--zip-file fileb://$(ZIPFILE)

undeploy:				# remove the lambda from AWS/Localstack
	$(AWS) lambda delete-function --function-name $(FUNCNAME)


# License related
# ===============

.PHONY: acquire revoke wait
acquire: wait		# acquire a new license
	$(AWS) lambda invoke --function-name $(FUNCNAME) \
		--cli-binary-format raw-in-base64-out --payload '{"action":"acquire"}' output.txt
	@cat output.txt
	@rm -f output.txt

revoke: wait		# revoke the license
	$(AWS) lambda invoke --function-name $(FUNCNAME) \
		--cli-binary-format raw-in-base64-out --payload '{"action":"revoke","id":"$(ID)"}' output.txt
	@cat output.txt
	@rm -f output.txt

wait:				# wait for the lambda to be active
	@$(AWS) lambda wait function-active-v2 --function-name $(FUNCNAME)
