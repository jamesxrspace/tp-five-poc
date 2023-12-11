import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_unity_widget/flutter_unity_widget.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/utils/logger.dart';
import 'package:uuid/uuid.dart';

class UnityMessageService {
  List<String> unityHolders = [];
  Map<String, Map<UnityMessageType, Iterable<UnityMessageCallback>>>
      unityMessageMap = {};

  void subscribe(
      String key, UnityMessageType type, UnityMessageCallback callback) {
    unityMessageMap[key] ??= {};

    unityMessageMap[key] = {
      ...unityMessageMap[key]!,
      type: unityMessageMap[key]!.containsKey(type)
          ? [...?unityMessageMap[key]![type], callback]
          : [callback]
    };
  }

  void unsubscribe(
      String key, UnityMessageType type, UnityMessageCallback callback) {
    if (unityMessageMap.containsKey(key) &&
        unityMessageMap[key]!.containsKey(type)) {
      unityMessageMap[key] = {
        ...unityMessageMap[key]!,
        type: unityMessageMap[key]![type]!.where((item) => item != callback)
      };

      if (unityMessageMap[key]![type]!.isEmpty) {
        unityMessageMap.remove(key);
      }
    }
  }

  void unsubscribeAll(String key) {
    if (unityMessageMap.containsKey(key)) {
      unityMessageMap.remove(key);
    }
  }

  void callback(String key, UnityMessage unityMessage) {
    if (unityMessageMap.containsKey(key) &&
        unityMessageMap[key]!.containsKey(unityMessage.type)) {
      for (var callback in unityMessageMap[key]![unityMessage.type]!) {
        callback(unityMessage);
      }
    }
  }

  void addUnityHolder(String key) {
    if (unityHolders.contains(key)) {
      throw Exception('Already contains key $key');
    }

    unityHolders.add(key);
  }

  void removeUnityHolder(String key) {
    unityHolders.remove(key);
  }

  bool isTopmostUnityHolder(String key) {
    if (unityHolders.isEmpty) {
      return false;
    }

    return unityHolders.last == key;
  }
}

// UnityPromiseNotifier
typedef Promise = Map<String, Completer>;

class UnityPromiseNotifier extends StateNotifier<Promise> {
  UnityPromiseNotifier() : super(<String, Completer>{});
}

final unityPromiseProvider =
    StateNotifierProvider<UnityPromiseNotifier, Promise>((ref) {
  return UnityPromiseNotifier();
});

// UnityPostMessageNotifier
class UnityPostRequired {
  final WidgetRef ref;
  final UnityWidgetController controller;

  UnityPostRequired(this.ref, this.controller);
}

class UnityAckData {
  late bool success;
  late UnityMessage unityMessage;

  bool isSuccess() {
    return success && unityMessage.errorCode == ErrorCode.SUCCESS;
  }
}

class UnityPostMessageNotifier extends StateNotifier<UnityPostRequired?> {
  UnityPostMessageNotifier() : super(null);

  static const tag = 'UnityPostMessageNotifier';
  static const _unityObjName = 'UnityMessageManager (singleton)';
  static const _unityFunctionName = 'onMessage';

  void setWidgetAndController(WidgetRef ref, UnityWidgetController controller) {
    state = UnityPostRequired(ref, controller);
  }

  void _postMessage(FlutterMessage flutterMessage) {
    var json = flutterMessageToJson(flutterMessage);
    Log.d(tag, 'Send message to unity: $json');
    state!.controller.postMessage(_unityObjName, _unityFunctionName, json);
  }

  void postMessage(FlutterMessageType type, String data) {
    _postMessage(FlutterMessage(
        data: data,
        type: type,
        sessionId: '',
        errorCode: ErrorCode.SUCCESS,
        errorMsg: ''));
  }

  void postMessageAck(
      String data, String sessionId, ErrorCode errorCode, String errorMsg) {
    _postMessage(FlutterMessage(
        data: data,
        errorCode: errorCode,
        errorMsg: errorMsg,
        sessionId: sessionId,
        type: FlutterMessageType.FLUTTER));
  }

  Future<UnityAckData> postMessageWait(FlutterMessageType type, String data,
      {timeout = 5}) async {
    var unityReturnData = UnityAckData();
    var promise = state!.ref.read(unityPromiseProvider);
    var uuid = const Uuid().v1();
    _postMessage(FlutterMessage(
        data: data,
        sessionId: uuid,
        type: type,
        errorCode: ErrorCode.SUCCESS,
        errorMsg: ''));
    promise[uuid] = Completer();
    try {
      unityReturnData.unityMessage =
          await promise[uuid]!.future.timeout(Duration(seconds: timeout));
      unityReturnData.success = true;
    } on Exception catch (e) {
      if (e is TimeoutException) {
        Fluttertoast.showToast(
            msg: 'Wait for unity message timeout',
            toastLength: Toast.LENGTH_LONG);
      }
      Log.e(tag, e.toString());

      promise[uuid]!.complete();
      unityReturnData.success = false;
    }
    promise.remove(uuid);
    return unityReturnData;
  }
}

final unityPostMessageProvider =
    StateNotifierProvider<UnityPostMessageNotifier, UnityPostRequired?>((ref) {
  return UnityPostMessageNotifier();
});
