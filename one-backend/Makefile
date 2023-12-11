.PHONY: all clean test run stop build upgrade help test-repo

all: 			# default action

clean:			# clean-up environment
	rm -rf ./data

test:			# run test

test-repo:
	docker compose -f docker-compose.local_test.yaml up mongodb -d
	cd server && IN_MAKEFILE=1 go test -tags=unit -timeout 30s -cover -bench=. -benchmem -v ./.../repo
	docker compose -f docker-compose.local_test.yaml down
	rm -rf .test-data

.PHONY: test-k6
test-k6:
	@./scripts/local-k6-test.sh

run:			# run in the local environment
	docker-compose up -d

stop:			# stop all the services
	docker-compose stop

build:			# build the binary/library
	docker-compose build

upgrade:		# upgrade all the necessary packages

help:			# show this message
	@printf "Usage: make [OPTION]\n"
	@printf "\n"
	@perl -nle 'print $$& if m{^[\w-]+:.*?#.*$$}' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?#"} {printf "    %-18s %s\n", $$1, $$2}'
