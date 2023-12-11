# License Server

This is the self-hosted Unity license server.
The project dispatch and revoke Unity Editor and Unity Pro licenses on CI/CD flows.

## Architecture

The license server is composed of the following components:

- Lambda/Go   the main component that dispatches and revokes licenses.
- DynamoDB    the database that stores the license information.
