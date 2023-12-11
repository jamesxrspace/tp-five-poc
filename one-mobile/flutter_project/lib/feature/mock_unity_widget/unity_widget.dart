import 'dart:convert';
import 'dart:async';
import 'package:flutter/material.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';

typedef UnityPostMessage = Future<void>? Function(
    String gameObject, String methodName, String message);

class UnityWidgetController {
  final UnityPostMessage _postMessage;
  UnityWidgetController(this._postMessage);
  Future<void>? postMessage(String gameObject, methodName, message) {
    return _postMessage(gameObject, methodName, message);
  }
}

typedef UnityMessageCallback = void Function(dynamic handler);

class UnityWidget extends StatefulWidget {
  const UnityWidget(
      {required this.onUnityMessage,
      required this.onUnityCreated,
      required this.useAndroidViewSurface,
      required this.hideStatus,
      required this.fullscreen,
      super.key});

  final UnityMessageCallback onUnityMessage;
  final UnityMessageCallback onUnityCreated;
  final bool useAndroidViewSurface;
  final bool hideStatus;
  final bool fullscreen;

  @override
  UnityWidgetState createState() => UnityWidgetState();
}

class UnityWidgetState extends State<UnityWidget> {
  UnityWidgetController? controller;
  static String curPage = '';
  static bool fakeUnityInit = false;

  void switchToPage(String page, String data) {
    curPage = page;
    widget.onUnityMessage('''{
        "type": "$page",
        "data": "$data",
        "sessionId": "",
        "errorCode": "${ErrorCode.SUCCESS.name}",
        "errorMsg": ""
        }''');
  }

  void sendRecordState(RecordStateType type) {
    widget.onUnityMessage('''{
        "type": "${UnityMessageType.RECORD_STATE.name}",
        "data": "{\\"type\\": \\"${type.name}\\"}",
        "sessionId": "",
        "errorCode": "${ErrorCode.SUCCESS.name}",
        "errorMsg": ""
        }''');
  }

  Future<void>? postMessage(
      String gameObject, String methodName, String message) {
    var receiveFlutterMessage = FlutterMessage.fromJson(jsonDecode(message));
    Timer.run(() {
      switch (receiveFlutterMessage.type) {
        case FlutterMessageType.REQUEST_TO_REEL_PAGE:
          switchToPage(UnityMessageType.SWITCHED_TO_REEL_PAGE.name, 'Success');
        case FlutterMessageType.REQUEST_TO_SOCIAL_LOBBY_PAGE:
          switchToPage(
              UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE.name, 'Success');
        case FlutterMessageType.REQUEST_TO_SPACE:
          switchToPage(UnityMessageType.SWITCHED_TO_SPACE.name, 'Success');
        case FlutterMessageType.SET_CAMERA:
          sendRecordState(RecordStateType.STANDBY);
        case FlutterMessageType.START_RECORD:
          sendRecordState(RecordStateType.RECORDING);
        case FlutterMessageType.STOP_RECORD:
          widget.onUnityMessage('''{
              "type": "${UnityMessageType.UNITY.name}",
              "data": "{\\"audio\\": \\"\\",\\"video\\": \\"\\",\\"thumbnail\\": \\"\\",\\"xrs\\":\\"\\"}",
              "sessionId": "${receiveFlutterMessage.sessionId}",
              "errorCode": "${ErrorCode.SUCCESS.name}",
              "errorMsg": ""
              }''');
          sendRecordState(RecordStateType.PREVIEW);
        case FlutterMessageType.START_FILM:
          widget.onUnityMessage('''{
              "type": "${UnityMessageType.UNITY.name}",
              "data": "{\\"audio\\": \\"\\",\\"video\\": \\"\\",\\"thumbnail\\": \\"\\",\\"xrs\\":\\"\\"}",
              "sessionId": "${receiveFlutterMessage.sessionId}",
              "errorCode": "${ErrorCode.SUCCESS.name}",
              "errorMsg": ""
              }''');
        case FlutterMessageType.REQUEST_REEL_SCENE_CONFIG:
          widget.onUnityMessage('''{
                "type": "${UnityMessageType.UNITY.name}",
                "data": "{\\"motionButtonActive\\": true}",
                "sessionId": "${receiveFlutterMessage.sessionId}",
                "errorCode": "${ErrorCode.SUCCESS.name}",
                "errorMsg": ""
                }''');
        default:
      }
    });
    return null;
  }

  @override
  void initState() {
    super.initState();
    controller = UnityWidgetController(postMessage);
    Timer.run(() {
      widget.onUnityCreated(controller);
    });

    // Unity send this event when the app is created
    if (!fakeUnityInit) {
      Timer.run(() {
        switchToPage(UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE.name, '');
      });
      fakeUnityInit = true;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Center(
        child: GridView.count(shrinkWrap: true, crossAxisCount: 2, children: [
      Column(children: [
        const Center(child: Text('Current Page:')),
        Center(child: Text(curPage, style: const TextStyle(fontSize: 9))),
      ]),
    ]));
  }
}
