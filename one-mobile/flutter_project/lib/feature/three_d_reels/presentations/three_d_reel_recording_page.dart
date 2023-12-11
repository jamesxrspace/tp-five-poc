import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/social_lobby/presentations/social_lobby.dart';
import 'package:tpfive/feature/three_d_reels/models/switch_option.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';
import 'package:tpfive/feature/three_d_reels/presentations/auto_preview_widget.dart';
import 'package:tpfive/feature/three_d_reels/presentations/change_camera_shot_widget.dart';
import 'package:tpfive/feature/three_d_reels/presentations/common.dart';
import 'package:tpfive/feature/three_d_reels/presentations/countdown_screen.dart';
import 'package:tpfive/feature/three_d_reels/presentations/preset_camera_button.dart';
import 'package:tpfive/feature/three_d_reels/presentations/preset_screen.dart';
import 'package:tpfive/feature/three_d_reels/presentations/record_button.dart';
import 'package:tpfive/feature/three_d_reels/presentations/recording_button.dart';
import 'package:tpfive/feature/three_d_reels/presentations/reel_record_confirm_modal.dart';
import 'package:tpfive/feature/three_d_reels/presentations/reels_function.dart';
import 'package:tpfive/feature/three_d_reels/presentations/share_reels.dart';
import 'package:tpfive/feature/unity_holder/unity_message_widget.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive/providers/record_state_provider.dart';
import 'package:tpfive/utils/logger.dart';
import 'package:tpfive/utils/permission_helper.dart';

class ThreeDReelRecordingPage extends UnityMessageWidget {
  static const routeName = '/threeDReelRecordingPage';
  static const tag = 'ThreeDReelRecordingPage';

  const ThreeDReelRecordingPage({super.key, required super.messageKey});

  @override
  ThreeDReelRecordingPageState createState() => ThreeDReelRecordingPageState();
}

class ThreeDReelRecordingPageState
    extends UnityMessageWidgetState<ThreeDReelRecordingPage>
    with TickerProviderStateMixin {
  late ThreeDReelRecordingPageViewModel _threeDReelRecordingPageViewModel;
  late AnimationController controller;
  //TODO: TF3R-390 [Unity][Flutter] face/body tracking  todo item
  var faceTrackingProgress = TrackingStateType.INITIATING;
  var bodyTrackingProgress = TrackingStateType.INITIATING;

  @override
  void initState() {
    super.initState();
    _threeDReelRecordingPageViewModel = ThreeDReelRecordingPageViewModel(ref);
    _threeDReelRecordingPageViewModel.subscribeUnityEvent(widget.messageKey);
    initController();
  }

  @override
  void initSubscribe() {
    subscribe(
        UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE, toSocialLobbyPage);
    subscribe(UnityMessageType.TRACKING_STATE, updateTrackingState);
  }

  @override
  void dispose() {
    super.dispose();
    _threeDReelRecordingPageViewModel.disposeTimer();
  }

  @override
  void onUnityWidgetCreated() {
    _threeDReelRecordingPageViewModel.requestUnitySceneInfo();
  }

  void initController() {
    controller = BottomSheet.createAnimationController(this);
    controller.duration = const Duration(milliseconds: 100);
    controller.reverseDuration = const Duration(milliseconds: 100);
  }

  @override
  Widget buildChildBody(BuildContext context) {
    if (!_threeDReelRecordingPageViewModel.isConfigRequested) {
      return Container();
    }

    return normalScreen();
  }

  Widget normalScreen() {
    final recordState = ref.watch(recordStateType);
    final showCountDown = ref.watch(showCountDownScreen);
    final faceTracking = ref.watch(faceTrackingState);
    final bodyTracking = ref.watch(bodyTrackingState);

    Widget button;

    switch (recordState) {
      case RecordStateType.PRESET:
        button = PresetCameraButton(onTap: () {
          _threeDReelRecordingPageViewModel
              .sendSetCameraMessageToUnity(SwitchOption(mode: true));
        });
        break;
      case RecordStateType.STANDBY:
        button = RecordButton(onTap: () {
          try {
            _threeDReelRecordingPageViewModel.startCountdown();
          } catch (e) {
            debugPrint(e.toString());
          }
        });
        break;
      case RecordStateType.RECORDING:
        button = RecordingButton(
            onTap: _threeDReelRecordingPageViewModel.stopRecord);
        break;
      case RecordStateType.PREVIEW:
        button = const AutoPreviewWidget();
        break;
      case RecordStateType.DONE:
        button = const ChangeCameraShotWidget();
        break;
      default:
        button = const SizedBox();
        break;
    }

    return Stack(
      children: [
        SafeArea(
          child: Stack(
            children: [
              Positioned(
                left: 20,
                top: 30,
                child: Visibility(
                  visible: recordState != RecordStateType.DONE &&
                      recordState != RecordStateType.PREVIEW,
                  child: ElevatedButton(
                    onPressed: () {
                      ref.read(loadingStateProvider.notifier).show(alpha: 1);
                      _threeDReelRecordingPageViewModel
                          .sendToSocialLobbyMessageToUnity(
                              SwitchOption(mode: true));
                    },
                    style: ElevatedButton.styleFrom(
                      shape: const CircleBorder(),
                      padding: const EdgeInsets.all(5),
                      backgroundColor: Colors.transparent,
                      foregroundColor: Colors.transparent,
                      shadowColor: Colors.transparent,
                    ),
                    child: const Icon(
                      TPFiveIcon.arrowLeft,
                      color: Colors.white,
                      size: 20,
                    ),
                  ),
                ),
              ),
              Positioned(
                left: 0,
                right: 0,
                top: 30,
                child: Visibility(
                  visible: recordState == RecordStateType.PREVIEW,
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      ElevatedButton(
                        onPressed: () {
                          showModalBottomSheet(
                              transitionAnimationController: controller,
                              isScrollControlled: true,
                              context: context,
                              shape: ThemeStyle.modalBorder,
                              builder: (builder) {
                                return ReelRecordConfirmModal(
                                  onStartOver: () {
                                    _threeDReelRecordingPageViewModel
                                        .resetRecord(SwitchOption(mode: true));
                                    setRecordType(RecordStateType.STANDBY);
                                    Navigator.pop(context);
                                  },
                                  onExit: () {
                                    _threeDReelRecordingPageViewModel
                                        .sendToSocialLobbyMessageToUnity(
                                            SwitchOption(mode: true));
                                    Navigator.pop(context);
                                  },
                                );
                              }).whenComplete(() => initController());
                        },
                        style: ElevatedButton.styleFrom(
                          shape: const CircleBorder(),
                          padding: const EdgeInsets.all(5),
                          backgroundColor: Colors.transparent,
                          foregroundColor: Colors.transparent,
                          shadowColor: Colors.transparent,
                        ),
                        child: const Icon(
                          TPFiveIcon.arrowLeft,
                          color: Colors.white,
                          size: 20,
                        ),
                      ),
                      Expanded(
                        child: Center(
                          child: Text(
                            'Preview',
                            style: GoogleFonts.poppins(
                              textStyle: const TextStyle(
                                fontSize: 23,
                                fontWeight: FontWeight.w800,
                                color: Colors.white,
                              ),
                            ),
                          ),
                        ),
                      ),
                      ElevatedButton(
                        onPressed: () async {
                          await _threeDReelRecordingPageViewModel
                              .setupCameraTracksAndPlay();
                        },
                        style: ElevatedButton.styleFrom(
                          shape: const CircleBorder(),
                          padding: const EdgeInsets.all(5),
                          backgroundColor: ThemeColors.mainColor,
                          foregroundColor: Colors.transparent,
                          shadowColor: Colors.transparent,
                        ),
                        child: const Icon(
                          TPFiveIcon.tick,
                          color: Colors.black,
                          size: 20,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              Positioned(
                left: 0,
                right: 0,
                top: 30,
                child: Visibility(
                  visible: recordState == RecordStateType.DONE,
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      ElevatedButton(
                        onPressed: () {
                          _threeDReelRecordingPageViewModel
                              .requestToPreviewPage();
                        },
                        style: ElevatedButton.styleFrom(
                          shape: const CircleBorder(),
                          padding: const EdgeInsets.all(5),
                          backgroundColor: Colors.transparent,
                          foregroundColor: Colors.transparent,
                          shadowColor: Colors.transparent,
                        ),
                        child: const Icon(
                          TPFiveIcon.arrowLeft,
                          color: Colors.white,
                          size: 20,
                        ),
                      ),
                      Expanded(
                        child: Center(
                          child: Text(
                            'Camera shot',
                            style: GoogleFonts.poppins(
                              textStyle: const TextStyle(
                                fontSize: 23,
                                fontWeight: FontWeight.w800,
                                color: Colors.white,
                              ),
                            ),
                          ),
                        ),
                      ),
                      ElevatedButton(
                        onPressed: () async {
                          await _threeDReelRecordingPageViewModel
                              .startFilm(true);
                          if (!mounted) return;
                          setRecordType(RecordStateType.UPLOAD);
                          Navigator.pushNamed(context, ShareReel.routeName,
                              arguments: {
                                'filePaths':
                                    _threeDReelRecordingPageViewModel.filePaths
                              }).then(
                              (value) => setRecordType(RecordStateType.DONE));
                        },
                        style: ElevatedButton.styleFrom(
                          shape: const CircleBorder(),
                          padding: const EdgeInsets.all(5),
                          backgroundColor: ThemeColors.mainColor,
                          foregroundColor: Colors.transparent,
                          shadowColor: Colors.transparent,
                        ),
                        child: const Icon(
                          TPFiveIcon.tick,
                          color: Colors.black,
                          size: 20,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              Positioned(
                right: 20,
                top: 30,
                child: Visibility(
                  visible: recordState == RecordStateType.STANDBY,
                  child: Center(
                    child: Container(
                      decoration: BoxDecoration(
                        color: const Color.fromRGBO(0, 0, 0, 0.2),
                        borderRadius: BorderRadius.circular(20.0),
                      ),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                        children: <Widget>[
                          GestureDetector(
                            child: Padding(
                              padding: const EdgeInsets.symmetric(
                                  vertical: 10, horizontal: 15),
                              child: Icon(TPFiveIcon.trackingFace,
                                  color: computeTrackingIconColor(faceTracking,
                                      faceTrackingProgress)), // Icon
                            ),
                            onTap: () async {
                              if (await PermissionHelper.checkPermission(
                                  Permission.camera)) {
                                if (!faceTracking) {
                                  faceTrackingProgress =
                                      TrackingStateType.INITIATING;
                                }
                                var config = TrackingConfig(
                                    face: !faceTracking,
                                    upperBody: bodyTracking,
                                    fullBody: false);
                                _threeDReelRecordingPageViewModel
                                    .sendUpdateTracking(config);
                              }
                            },
                          ),
                          GestureDetector(
                            child: Padding(
                              padding: const EdgeInsets.symmetric(
                                  vertical: 10, horizontal: 15),
                              child: Icon(TPFiveIcon.trackingUpbody,
                                  color: computeTrackingIconColor(bodyTracking,
                                      bodyTrackingProgress)), // Icon
                            ),
                            onTap: () async {
                              if (await PermissionHelper.checkPermission(
                                  Permission.camera)) {
                                if (!bodyTracking) {
                                  bodyTrackingProgress =
                                      TrackingStateType.INITIATING;
                                }
                                var config = TrackingConfig(
                                    face: faceTracking,
                                    upperBody: !bodyTracking,
                                    fullBody: false);
                                _threeDReelRecordingPageViewModel
                                    .sendUpdateTracking(config);
                              }
                            },
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ),
              Positioned(
                right: 20,
                top: 200,
                child: Visibility(
                    visible: recordState != RecordStateType.PREVIEW &&
                        recordState != RecordStateType.PRESET &&
                        recordState != RecordStateType.DONE,
                    child: ReelsFunction(
                        motionButtonActive: _threeDReelRecordingPageViewModel
                            .reelSceneInfo.motionButtonActive)),
              ),
              Positioned(
                bottom: 50,
                left: 0,
                right: 0,
                child: button,
              ),
              Positioned.fill(
                child: Visibility(
                    visible: recordState == RecordStateType.PRESET,
                    child: const PresetScreen()),
              ),
            ],
          ),
        ),
        Positioned.fill(
          child: Visibility(
              visible: showCountDown, child: const CountdownScreen()),
        ),
      ],
    );
  }

  void toSocialLobbyPage(message) {
    var currentStateType = ref.read(recordStateType);
    Log.d(ThreeDReelRecordingPage.tag,
        'Received message from unity: ${message.toString()}, ${currentStateType.toString()}');

    if (message.errorCode == ErrorCode.SUCCESS) {
      setRecordType(RecordStateType.PRESET);
      resetStates(ref);
      ref.read(loadingStateProvider.notifier).hide();
      switch (currentStateType) {
        case RecordStateType.UPLOAD:
          Navigator.popUntil(
              context, ModalRoute.withName(SocialLobby.routeName));
          break;
        default:
          Navigator.pop(context);
          break;
      }
    }
  }

  void updateTrackingState(dynamic message) {
    var trackingState = TrackingState.fromJson(json.decode(message.data));
    switch (trackingState.type) {
      case TrackingFlag.FACE:
        setState(() {
          faceTrackingProgress = trackingState.state;
        });
        break;
      case TrackingFlag.UPPER_BODY:
        setState(() {
          bodyTrackingProgress = trackingState.state;
        });
        break;
      default:
        break;
    }
  }
}
