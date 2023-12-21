# Overview

New app for vision pro.

## Setup

Run `make setup` to install all dependencies.

### Environment

#### backend

Install golang 1.21

```bash
brew install go@1.21
```

#### pre-commit

Pre-commit is a framework for managing and maintaining multi-language pre-commit hooks. We use it to setup lint and test hooks

- [Have the pre-commit package manager installed](https://pre-commit.com/#installation).

  ```bash
  # On mac, use homebrew
  $ brew update && brew install pre-commit install hadolint
  # On Windows WSL, use pip
  $ pip install pre-commit
  $ docker pull hadolint/hadolint
  $ docker run hadolint/hadolint
  ```

- Install [pre-commit](https://pre-commit.com/#quick-start) in the current project
- Build dll

  ```bash
  pre-commit install
  ```

### Infra

Confidential Data Management
<https://xrspace.atlassian.net/wiki/spaces/TF/pages/2254897163/Confidential+Data+Management>

# tp-five-poc
