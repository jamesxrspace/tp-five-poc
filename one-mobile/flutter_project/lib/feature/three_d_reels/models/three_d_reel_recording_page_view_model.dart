import 'dart:async';
import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:get_it/get_it.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:tpfive/feature/three_d_reels/models/switch_option.dart';
import 'package:tpfive/feature/unity_holder/unity_message_mixin.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive/providers/record_state_provider.dart';
import 'package:tpfive/services/unity_message_service.dart';
import 'package:tpfive/utils/logger.dart';
import 'package:tpfive/utils/permission_helper.dart';

enum CoCreateMode { VIEW, JOIN }

final showCountDownScreen = StateProvider<bool>((ref) => false);
final countdownNumber = StateProvider<double>((ref) => 3.0);
final countdownRecordingNumber = StateProvider<double>((ref) => 0);
final recordProcessNumber = StateProvider<double>((ref) => 0.0);
final previewProcessNumber = StateProvider<double>((ref) => 0);
final micActivate = StateProvider<bool>((ref) => false);
final hasImportMusic = StateProvider<bool>((ref) => false);
const int recordMaximumTime = 30;
const double tick = 0.05;
final coCreateMode = StateProvider<CoCreateMode>((ref) => CoCreateMode.VIEW);
final descriptionExpanded = StateProvider<bool>((ref) => false);
final isLike = StateProvider<bool>((ref) => false);
final selectedUGCItem = StateProvider<int>((ref) => -1);
final selectedCameraIndex = StateProvider<int>((ref) => 0);
final trackCounts = StateProvider<int>((ref) => 0);
final faceTrackingState = StateProvider<bool>((ref) => false);
final bodyTrackingState = StateProvider<bool>((ref) => false);
double defaultReelVideoSec = 30;

void resetStates(WidgetRef ref) {
  ref.read(micActivate.notifier).state = false;
  ref.read(hasImportMusic.notifier).state = false;
  ref.read(bodyTrackingState.notifier).state = false;
  ref.read(faceTrackingState.notifier).state = false;
  ref.read(selectedCameraIndex.notifier).state = 0;
}

void setShowCountDownScreen(bool flag, WidgetRef ref) {
  ref.read(showCountDownScreen.notifier).state = flag;
}

void setCountDownNumber(double num, WidgetRef ref) {
  ref.read(countdownNumber.notifier).state = num;
}

void setCountdownRecordingNumber(double num, WidgetRef ref) {
  ref.read(countdownRecordingNumber.notifier).state = num;
}

void setRecordProcessNumber(double num, WidgetRef ref) {
  ref.read(recordProcessNumber.notifier).state = num;
}

void setPreviewProcessNumber(double num, WidgetRef ref) {
  ref.read(previewProcessNumber.notifier).state = num;
}

void setMicActivate(bool flag, WidgetRef ref) {
  ref.read(micActivate.notifier).state = flag;
}

void setHasImportMusic(bool flag, WidgetRef ref) {
  ref.read(hasImportMusic.notifier).state = flag;
}

void setFaceTrackingState(bool flag, WidgetRef ref) {
  ref.read(faceTrackingState.notifier).state = flag;
}

void setBodyTrackingState(bool flag, WidgetRef ref) {
  ref.read(bodyTrackingState.notifier).state = flag;
}

class ThreeDReelRecordingPageViewModel with UnityMessageMixin {
  final WidgetRef _ref;
  static const tag = 'ThreeDReelRecordingPageViewModel';

  ThreeDReelRecordingPageViewModel(WidgetRef ref) : _ref = ref {
    initUnityMessageProvider(_ref);
  }
  Timer? _countdownTimer;
  Timer? _recordingTimer;
  late ReelFilePath filePaths;
  bool isCoCreate = false;
  double reelVideoSec = defaultReelVideoSec;

  static const oneSec = Duration(seconds: 1);
  Duration timerTick = Duration(milliseconds: (tick * 1000).toInt());

  ReelSceneConfig reelSceneInfo = const ReelSceneConfig(
      motionButtonActive: false,
      initState: RecordStateType.PRESET,
      decorationActive: true,
      decorations: []);
  bool isConfigRequested = false;

  Future<void> requestUnitySceneInfo() async {
    _ref.read(loadingStateProvider.notifier).show();

    UnityAckData result = await postMessageToUnityWait(
        FlutterMessageType.REQUEST_REEL_SCENE_CONFIG, '');

    if (!result.success) {
      Log.e('ThreeDReelRecordingPageViewModel', 'Fail to get ReelSceneConfig');
      return;
    }

    isConfigRequested = true;
    reelSceneInfo = reelSceneConfigFromJson(result.unityMessage.data);
    updateUIByRecordState(reelSceneInfo.initState);
    _ref.read(loadingStateProvider.notifier).hide();
  }

  void startCountdown() {
    setShowCountDownScreen(true, _ref);

    setCountDownNumber(3, _ref);
    if (_countdownTimer?.isActive ?? false) {
      throw Exception('_countdownTimer exist');
    } else {
      _countdownTimer = Timer.periodic(timerTick, (Timer timer) {
        double countdown = _ref.read(countdownNumber);

        if (countdown < tick) {
          timer.cancel();
          sendRecordMessageToUnity(SwitchOption(mode: true));
        } else {
          setCountDownNumber(countdown - tick, _ref);
        }
      });
    }
  }

  void startRecordingCountdown(double reelDuration, bool autoStopRecord) {
    setCountdownRecordingNumber(0, _ref);
    setRecordProcessNumber(0.0, _ref);
    if (_recordingTimer?.isActive ?? false) {
      throw Exception('_recordingTimer exist');
    } else {
      _recordingTimer = Timer.periodic(timerTick, (Timer timer) {
        double countdownRecordingNow = _ref.read(countdownRecordingNumber);

        if (countdownRecordingNow > reelDuration) {
          timer.cancel();
          if (!autoStopRecord) stopRecord();
        } else {
          setCountdownRecordingNumber(countdownRecordingNow + tick, _ref);
          setRecordProcessNumber(
              (countdownRecordingNow + tick) / reelDuration, _ref);
        }
      });
    }
  }

  void setReelVideoSec(double sec) {
    reelVideoSec = sec;
  }

  void startCountdownPreview(bool isCoCreate, {required Function onEnd}) {
    double countdownRecording;
    countdownRecording =
        isCoCreate ? reelVideoSec : _ref.read(countdownRecordingNumber);

    setPreviewProcessNumber(0, _ref);
    if (_recordingTimer?.isActive ?? false) {
      throw Exception('_recordingTimer exist');
    } else {
      _recordingTimer = Timer.periodic(timerTick, (Timer timer) {
        double previewProcessNow = _ref.read(previewProcessNumber);
        if (previewProcessNow > countdownRecording) {
          timer.cancel();
          onEnd();
        } else {
          setPreviewProcessNumber(previewProcessNow + tick, _ref);
        }
      });
    }
  }

  void sendSetCameraMessageToUnity(SwitchOption option) {
    postMessageToUnity(
        FlutterMessageType.SET_CAMERA, json.encode(option.toJson()));
  }

  void sendRecordMessageToUnity(SwitchOption option) {
    postMessageToUnity(
        FlutterMessageType.START_RECORD, json.encode(option.toJson()));
  }

  void receiveRecordingMessageFromUnity({isCoCreate = false}) {
    setShowCountDownScreen(false, _ref);
    try {
      startRecordingCountdown(
          isCoCreate ? reelVideoSec : defaultReelVideoSec, isCoCreate);
    } catch (e) {
      Log.e(tag, 'receive recording message error: ${e.toString()}');
    }
  }

  void sendStopRecordMessageToUnity(SwitchOption option) async {
    _ref.read(loadingStateProvider.notifier).show();
    await postMessageToUnityWait(
            FlutterMessageType.STOP_RECORD, json.encode(option.toJson()),
            timeout: 30)
        .then((value) =>
            filePaths = reelFilePathFromJson(value.unityMessage.data));
    _ref.read(loadingStateProvider.notifier).hide();
  }

  void receivePreviewMessageFromUnity({isCoCreate = false}) {
    startPreview(true, isCoCreate: isCoCreate);
  }

  void sendToSocialLobbyMessageToUnity(SwitchOption option) {
    _ref.read(loadingStateProvider.notifier).show(alpha: 1);

    postMessageToUnity(FlutterMessageType.REQUEST_TO_SOCIAL_LOBBY_PAGE,
        json.encode(option.toJson()));
  }

  void sendSetMicToUnity(SwitchOption option) {
    postMessageToUnityWait(
            FlutterMessageType.SET_MIC, json.encode(option.toJson()))
        .then((value) {
      if (value.unityMessage.data == 'True') {
        _ref.read(micActivate.notifier).state = true;
      } else {
        _ref.read(micActivate.notifier).state = false;
      }
    });
  }

  void setCurrentMusic(MusicData musicData) {
    postMessageToUnityWait(
        FlutterMessageType.SET_MUSIC, musicDataToJson(musicData));
  }

  void unsetCurrentMusic() {
    postMessageToUnityWait(FlutterMessageType.SET_MUSIC, '');
  }

  void playMusic(MusicData musicData) {
    postMessageToUnityWait(
        FlutterMessageType.PLAY_MUSIC, musicDataToJson(musicData));
  }

  void stopMusic() {
    postMessageToUnityWait(FlutterMessageType.PLAY_MUSIC, '');
  }

  Future sendStartAIGCToUnity(SwitchOption option) async {
    _ref.read(loadingStateProvider.notifier).show();
    await postMessageToUnityWait(
        FlutterMessageType.START_AIGC, json.encode(option.toJson()));
    _ref.read(loadingStateProvider.notifier).hide();
  }

  Future sendUpdateTracking(TrackingConfig config) async {
    _ref.read(loadingStateProvider.notifier).show();
    var value = await postMessageToUnityWait(
        FlutterMessageType.TOGGLE_TRACKING, json.encode(config.toJson()));
    if (value.success) {
      _ref.read(faceTrackingState.notifier).state = config.face;
      _ref.read(bodyTrackingState.notifier).state = config.upperBody;
    }
    _ref.read(loadingStateProvider.notifier).hide();
  }

  Future<void> requestToPreviewPage() async {
    await postMessageToUnityWait(FlutterMessageType.REQUEST_TO_PREVIEW, '');
  }

  void stopRecord() {
    _recordingTimer?.cancel();
    sendStopRecordMessageToUnity(SwitchOption(mode: true));
  }

  void setUGCItem(int aigcId) {
    _ref.read(selectedUGCItem.notifier).state = aigcId;
  }

  Future<int> _getTrackCountOfCurrentScene() async {
    try {
      var result =
          await postMessageToUnityWait(FlutterMessageType.GET_TRACK_COUNT, '');
      if (!result.success) {
        throw 'get track count failed';
      }
      return int.parse(result.unityMessage.data);
    } catch (e) {
      Log.e(tag, 'get track count error: ${e.toString()}');
    }
    return 0;
  }

  void setTrackOfCurrentScene(int index) {
    _ref.read(selectedCameraIndex.notifier).state = index;
    postMessageToUnity(
        FlutterMessageType.SELECT_TRACK, json.encode({'track': index}));
  }

  void playTrackOfCurrentScene() {
    postMessageToUnity(FlutterMessageType.SELECT_TRACK,
        json.encode({'track': _ref.read(selectedCameraIndex)}));
  }

  void resetRecord(SwitchOption option) {
    postMessageToUnity(
        FlutterMessageType.RESET_RECORD, json.encode(option.toJson()));
  }

  void disposeTimer() {
    _countdownTimer?.cancel();
    _recordingTimer?.cancel();
  }

  void startPreview(bool flag,
      {isCoCreate = false,
      isLoop = false,
      Function? onStart,
      Function? onEnd}) {
    _recordingTimer?.cancel();
    // TBD: remove bool flag after pr
    postMessageToUnity(
        FlutterMessageType.START_PREVIEW, json.encode({'mode': flag}));
    try {
      onStart?.call();
      startCountdownPreview(isCoCreate, onEnd: () {
        onEnd?.call();
        if (isLoop) {
          startPreview(true,
              onStart: onStart,
              onEnd: onEnd,
              isCoCreate: isCoCreate,
              isLoop: isLoop);
        }
      });
    } catch (e) {
      Log.e(tag, 'preview error: ${e.toString()}');
    }
  }

  Future<void> startFilm(bool flag) async {
    _ref.read(loadingStateProvider.notifier).show();
    playTrackOfCurrentScene();
    var result = await postMessageToUnityWait(
        FlutterMessageType.START_FILM, json.encode({'mode': flag}),
        timeout: recordMaximumTime + 1);
    var tmp = reelFilePathFromJson(result.unityMessage.data);
    filePaths = ReelFilePath(
        thumbnail: tmp.thumbnail, video: tmp.video, xrs: tmp.xrs, audio: '');
    _ref.read(loadingStateProvider.notifier).hide();
  }

  Future<void> setupCameraTracksAndPlay() async {
    _ref.read(trackCounts.notifier).state =
        await _getTrackCountOfCurrentScene();
    startPreview(true,
        onStart: () => {playTrackOfCurrentScene()},
        isCoCreate: isCoCreate,
        isLoop: true);
  }

  void setCoCreateMode(CoCreateMode type) {
    setRecordType(RecordStateType.STANDBY);
    _ref.read(coCreateMode.notifier).state = type;
  }

  void setIsLike(bool flag) {
    _ref.read(isLike.notifier).state = flag;
  }

  void setDescriptionExpanded(bool flag) {
    _ref.read(descriptionExpanded.notifier).state = flag;
  }

  void subscribeUnityEvent(String messageKey) {
    final UnityMessageService unityMessageService =
        GetIt.I<UnityMessageService>();
    unityMessageService.subscribe(messageKey, UnityMessageType.RECORD_STATE,
        (message) {
      Log.d(tag, 'Received message from unity: ${message.toString()}');

      updateUIByRecordState(recordStateTypeFromJson(message.data));
    });
  }

  void updateUIByRecordState(RecordStateType recordState) async {
    setRecordType(recordState);

    if (recordState == RecordStateType.STANDBY) {
      if (await PermissionHelper.checkPermission(Permission.camera)) {
        var config =
            const TrackingConfig(face: true, upperBody: true, fullBody: false);
        await sendUpdateTracking(config);
      }
    } else if (recordState == RecordStateType.RECORDING) {
      receiveRecordingMessageFromUnity(isCoCreate: isCoCreate);
    } else if (recordState == RecordStateType.PREVIEW) {
      receivePreviewMessageFromUnity(isCoCreate: isCoCreate);
    }
  }
}
