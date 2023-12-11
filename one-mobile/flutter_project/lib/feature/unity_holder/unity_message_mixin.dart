import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/services/unity_message_service.dart';

mixin UnityMessageMixin {
  late UnityPostMessageNotifier _unityPostMessageNotifier;

  void initUnityMessageProvider(WidgetRef ref) {
    _unityPostMessageNotifier = ref.read(unityPostMessageProvider.notifier);
  }

  void postMessageToUnity(FlutterMessageType type, String payload) {
    _unityPostMessageNotifier.postMessage(type, payload);
  }

  void postMessageToUnityAck(
      String data, String sessionId, ErrorCode errorCode, String errorMessage) {
    _unityPostMessageNotifier.postMessageAck(
        data, sessionId, errorCode, errorMessage);
  }

  Future<UnityAckData> postMessageToUnityWait(
      FlutterMessageType type, String payload,
      {timeout = 5}) {
    return _unityPostMessageNotifier.postMessageWait(type, payload,
        timeout: timeout);
  }
}
