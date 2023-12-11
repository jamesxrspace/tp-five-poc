# Overview

## Layout

Package Layout follows [Unity document](https://docs.unity3d.com/Manual/cus-layout.html).

## Folder Structure

- core
- creator
- unity-project
- font-tool
- openapi-doc

### core

All the packages mainly used inside complete unity project or other projects
reside here.
There are also folders ordered by specific alphabetical order.

The cross package is used as bridge by different unity projects.
The usage is for reducing the dependency between projects.

More detail can be found in the package readme.

### creator

The creator related packages reside here.

### unity-project

Complete unity project mainly resides here.

By complete unity project, it means this is the actual build which will be
installed on devices.

### font-tool

See [Pull Request](https://github.com/XRSPACE-Inc/tp-five/pull/26)

### openapi-doc

See [OpenAPI Document](https://xrspace.atlassian.net/wiki/spaces/TF/pages/2255585303/Quick+guide+how+to+work+with+OpenAPI)

## Environment

### Login environment config

- Please follow the [instructions](https://xrspace.atlassian.net/l/cp/7mGL31Aw)
here to setup the system configuration correctly for local development.

## Notes

- Choose a meaningful name for the new LifetimeScope GameObject
(For VContainer Diagnostics purpose).

    For example, new LifetimeScope GameObject named "```LifetimeScope.\<SCENE NAME\>```".
    for more info see <https://github.com/XRSPACE-Inc/tp-five/pull/39>

## Tests

### How to write tests

- Basically, follow [this instructions](https://docs.unity3d.com/Packages/com.unity.test-framework@2.0/manual/workflow-create-test.html)
- If you write tests in the package folder
(e.g. under `core/development/common` or `core/development/frontend` folder),
you need to add package name to the `testables` section
in the `unity-project/development/complete-unity/Packages/manifest.json` file.
