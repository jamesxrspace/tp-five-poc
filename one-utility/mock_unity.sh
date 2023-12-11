#! /usr/bin/env bash
set -o pipefail

REPO_ROOT=$($(which git) rev-parse --show-toplevel)

function welcome() {
    echo "This is the help script to mock unity and easy to implement flutter"
    echo "Contact to morris or cmj if you have any question"
    echo ""
    echo "Enjoy it!"
}

function replace {
    PATTERN="$1"
    OLD="$2"
    NEW="$3"

    find . -name "$PATTERN" | while read -r line; do
        case $(uname) in
            Darwin)
                sed -i '' "s#$OLD#$NEW#g" "$line"
                ;;
            *)
                sed -i "s#$OLD#$NEW#g" "$line"
                ;;
        esac
    done
}

function remove {
    PATTERN="$1"
    OLD="$2"

    find . -name "$PATTERN" | while read -r line; do
        case $(uname) in
            Darwin)
                sed -i '' "/$OLD/d" "$line"
                ;;
            *)
                sed -i "/$OLD/d" "$line"
                ;;
        esac
    done
}

function patch_pubspec_yaml {
    case $(uname) in
        Darwin)
            sed -i '' '/flutter_unity_widget:/,+4d' "$REPO_ROOT/one-mobile/flutter_project/pubspec.yaml"
            ;;
        *)
            sed -i '/flutter_unity_widget:/,+3d' "$REPO_ROOT/one-mobile/flutter_project/pubspec.yaml"
            ;;
    esac
}

function main() {
    welcome

    patch_pubspec_yaml

    pushd "$REPO_ROOT/one-mobile/flutter_project/" || exit 1
    replace '*.dart' "flutter_unity_widget/flutter_unity_widget.dart" "tpfive/feature/mock_unity_widget/unity_widget.dart"
    replace '*.kt' "com.xraph.plugin.flutter_unity_widget.FlutterUnityActivity" "io.flutter.embedding.android.FlutterActivity"
    replace '*.kt' "MainActivity: FlutterUnityActivity" "MainActivity: FlutterActivity"

    remove '*.swift' 'import flutter_unity_widget'
    remove '*.swift' 'InitUnityIntegrationWithOptions.*$'
    remove 'build.gradle' "implementation project(':unityLibrary')"
    remove 'project.pbxproj' "UnityFramework.framework"
    popd || exit 1

    echo "Congratulation! You have mock the unity successfully"
    echo "Now you can run 'flutter run --flavor dev --target lib/main_dev.dart' to build the Android/iOS without unity"
}

main
