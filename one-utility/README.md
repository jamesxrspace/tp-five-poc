# Utility for TP-five

This folder contains several scripts that can be used to run the TP-five
on each scenario.

## Build iOS APP

In case of build and launch the iOS on your local device, you need to
execute the following commands to patch your build environment:

1. `./patch-build-ios-app.sh` __BEFORE__ export the project from Unity.
2. `./link-unity-frameworks.sh` __AFTER__ export the project from Unity.
