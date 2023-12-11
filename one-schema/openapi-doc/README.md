
# OpenAPI Overview

An [OpenAPI](https://spec.openapis.org/oas/latest.html)
definition can then be used by documentation generation tools to display the API,
code generation tools to generate servers and clients in various programming languages

## Mock server

Use [prism cli](https://github.com/stoplightio/prism) to start http mock server quickly

```bash
npm install -g @stoplight/prism-cli
prism mock <openapi.yaml>
```

## Unity package

[openapi-unity](https://github.com/XRSPACE-Inc/openapi-unity)
