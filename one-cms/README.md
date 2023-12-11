# One CMS

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app)

## Prerequisites

Please set up Node.js environment first

- [Node.js](https://nodejs.org/en/download/)
- [pnpm](https://pnpm.io/zh-TW/)

## How to run this app

1. Run `one-utility/decrypt_env.sh` to create `.env` from `.local.env`

    ```sh
    sh one-utility/decrypt_env.sh
    ```

1. Enter the project folder

    ```sh
    # Open a new terminal window and enter the folder
    cd <project folder>
    ```

2. Install packages via pnpm

    ```sh
    pnpm install
    ```

3. Runs the app in the development mode

    ```sh
    pnpm start
    ```

## Available Scripts

In the project directory, you can run:

### `pnpm start`

Runs the app in the development mode.

Open[http://localhost:3000](http://localhost:3000) to view it in the browser.

The page will reload if you make edits.

You will also see any lint errors in the console.

### `pnpm test`

Launches the test runner in the interactive watch mode.

See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `pnpm ci:test`

Run tests in CI-mode, and they will only run once instead of launching the watcher.

See the section about [running tests on CI servers](https://create-react-app.dev/docs/running-tests/#on-ci-servers) for more information.

### `pnpm run build`

Builds the app for production to the `build` folder.

It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `pnpm lint`

Format the code by eslint and prettier

## How to use OpenAPI generator

1. Install OpenAPI Generator CLI first

    You can simply install it by homebrew or install in [another way](https://openapi-generator.tech/docs/installation)

    ```sh
    brew install openapi-generator
    ```

1. Update the config file `src/scripts/openapi-merge.json` to include the API files which you need

    See more details in the [openapi-merge-cli documents](https://github.com/robertmassaioli/openapi-merge#the-openapi-merge-repository)

1. Generate code by running `one-cms/src/scripts/generate_openapi.sh`

    ```sh
    sh one-cms/src/scripts/generate_openapi.sh
    ```
