# Overview

This package is served as bridge, which is used to reducing dependecies among unity projects.
The technique relies on using delegates.

In planned usage, the service implementation will provide the binding to delegates. Since
those services in regular flow will be setup before content is loaded, this ensures at the
time these bindings are used while content loaded, the actual behavior is provided.

As these delegates are used, the actual behavior can be bound any time to fit the purpose.
