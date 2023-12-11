UNITY_ENV := dev

.PHONY: all clean test run build upgrade setup help

all: 			# default action

clean:			# clean-up environment

test:			# run test

run:			# run in the local environment

build:			# build the binary/library

upgrade:		# upgrade all the necessary packages
	pre-commit autoupdate

setup:			# setup your local DEV environment
	@[ -f .git/hooks/pre-commit ] || pre-commit install --install-hooks
	@git config commit.template .gitmessage.txt
	@git config blame.ignoreRevsFile .git-blame-ignore-revs

help:			# show this message
	@printf "Usage: make [OPTION]\n"
	@printf "\n"
	@perl -nle 'print $$& if m{^[\w-]+:.*?#.*$$}' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?#"} {printf "    %-18s %s\n", $$1, $$2}'

setup: setup-bootstrap setup-env

setup-bootstrap:				# setup your local DEV env
	bash -i ./one-utility/bootstrap

setup-env: setup-bootstrap		# decrypt and setup your .env files
	@printf "Decrypt local environment files\n"
	@git checkout one-utility/decrypt_env.sh
	@./one-utility/decrypt_env.sh $(UNITY_ENV)
