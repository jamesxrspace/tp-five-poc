import 'dart:convert';

import 'package:expandable_text/expandable_text.dart';
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
import 'package:tpfive_game_server_api_client/api.dart';
import 'package:video_player/video_player.dart';

class CoCreatePage extends UnityMessageWidget {
  static const routeName = '/coCreatePage';
  static const tag = 'CoCreatePage';

  final Reel reelData;

  const CoCreatePage(
      {super.key, required super.messageKey, required this.reelData});

  @override
  CoCreatePageState createState() => CoCreatePageState();
}

class CoCreatePageState extends UnityMessageWidgetState<CoCreatePage>
    with TickerProviderStateMixin {
  late AnimationController controller;
  late ThreeDReelRecordingPageViewModel _threeDReelRecordingPageViewModel;
  late VideoPlayerController _videoController;
  //TODO: TF3R-390 [Unity][Flutter] face/body tracking  todo item
  var faceTrackingProgress = TrackingStateType.INITIATING;
  var bodyTrackingProgress = TrackingStateType.INITIATING;

  @override
  void initState() {
    super.initState();
    _threeDReelRecordingPageViewModel = ThreeDReelRecordingPageViewModel(ref);
    _threeDReelRecordingPageViewModel.isCoCreate = true;
    initUnityMessageProvider(ref);
    initController();
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      ref.watch(recordStateType);
    });
    _videoController =
        VideoPlayerController.contentUri(Uri.parse(widget.reelData.video!))
          ..initialize().then((value) {
            setState(() {});
            _threeDReelRecordingPageViewModel.setReelVideoSec(
                _videoController.value.duration.inSeconds.toDouble());
          });

    _threeDReelRecordingPageViewModel.subscribeUnityEvent(widget.messageKey);
  }

  @override
  void initSubscribe() {
    subscribe(
        UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE, toSocialLobbyPage);
    subscribe(UnityMessageType.TRACKING_STATE, updateTrackingState);
  }

  @override
  void onUnityWidgetCreated() {
    _threeDReelRecordingPageViewModel.requestUnitySceneInfo();
  }

  @override
  void dispose() {
    super.dispose();
    controller.dispose();
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

    final coCreateModeState = ref.watch(coCreateMode);
    return coCreateModeState == CoCreateMode.JOIN
        ? buildCoCreateScreen()
        : buildViewScreen();
  }

  Widget buildViewScreen() {
    final descriptionExpandedState = ref.watch(descriptionExpanded);
    final isLikeState = ref.watch(isLike);

    return SafeArea(
      top: false,
      child: Stack(
        children: [
          Positioned.fill(
            child: Visibility(
              visible: descriptionExpandedState,
              child: Container(
                color: Colors.black.withOpacity(0.4),
                child: Center(
                  child: Container(),
                ),
              ),
            ),
          ),
          Positioned(
            left: 20,
            top: 40,
            child: ElevatedButton(
              onPressed: () {
                _threeDReelRecordingPageViewModel
                    .sendToSocialLobbyMessageToUnity(SwitchOption(mode: true));
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
                color: Color.fromRGBO(217, 217, 217, 1),
                size: 20,
              ),
            ),
          ),
          Positioned(
            right: 20,
            top: 50,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: <Widget>[
                GestureDetector(
                  child: Container(
                    width: 87,
                    height: 32,
                    padding: const EdgeInsets.symmetric(
                        horizontal: 12, vertical: 10),
                    decoration: ShapeDecoration(
                      color: Colors.black.withOpacity(0.2),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(99),
                      ),
                    ),
                    child: const Row(
                      mainAxisSize: MainAxisSize.min,
                      mainAxisAlignment: MainAxisAlignment.center,
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        Icon(
                          TPFiveIcon.three_d_view,
                          color: Colors.white,
                          size: 15,
                        ),
                        Text(
                          '3D View',
                          textAlign: TextAlign.right,
                          style: TextStyle(
                            color: Colors.white,
                            fontSize: 11,
                            fontFamily: 'Poppins',
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                  ),
                  onTap: () {},
                ),
                const SizedBox(width: 10),
                GestureDetector(
                  child: Container(
                    width: 70,
                    height: 32,
                    padding: const EdgeInsets.symmetric(
                        horizontal: 12, vertical: 10),
                    decoration: ShapeDecoration(
                      color: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(99),
                      ),
                    ),
                    child: const Row(
                      mainAxisSize: MainAxisSize.min,
                      mainAxisAlignment: MainAxisAlignment.center,
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        Icon(
                          TPFiveIcon.join,
                          size: 15,
                        ),
                        Text(
                          'Join',
                          textAlign: TextAlign.right,
                          style: TextStyle(
                            color: Colors.black,
                            fontSize: 11,
                            fontFamily: 'Poppins',
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                  ),
                  onTap: () {
                    postMessageToUnityWait(FlutterMessageType.COCREATE_JOIN, '',
                            timeout: 30)
                        .then((data) {
                      if (data.success) {
                        _threeDReelRecordingPageViewModel
                            .setCoCreateMode(CoCreateMode.JOIN);
                      }
                    });
                  },
                ),
                const SizedBox(width: 10),
                GestureDetector(
                  child: Container(
                    width: 32,
                    height: 32,
                    padding: const EdgeInsets.all(8),
                    decoration: ShapeDecoration(
                      color: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(99),
                      ),
                    ),
                    child: const Icon(
                      TPFiveIcon.reels,
                      size: 15,
                    ),
                  ),
                  onTap: () {},
                ),
              ],
            ),
          ),
          Positioned(
            right: 0,
            top: 350,
            child: Column(
              children: [
                ElevatedButton(
                  onPressed: () {
                    bool isLikeNow = ref.read(isLike);
                    _threeDReelRecordingPageViewModel.setIsLike(!isLikeNow);
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.transparent,
                    shadowColor: Colors.transparent,
                  ),
                  child: Column(
                    children: [
                      Icon(
                        isLikeState ? TPFiveIcon.heart_on : TPFiveIcon.heart,
                        color: isLikeState
                            ? const Color.fromRGBO(249, 55, 125, 1)
                            : Colors.white,
                      ),
                      const SizedBox(height: 3),
                      const Text('Like')
                    ],
                  ),
                ),
                const SizedBox(height: 16),
                ElevatedButton(
                  onPressed: () {},
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.transparent,
                    shadowColor: Colors.transparent,
                  ),
                  child: const Column(
                    children: [
                      Icon(TPFiveIcon.share),
                      SizedBox(height: 3),
                      Text('Share')
                    ],
                  ),
                ),
                const SizedBox(height: 16),
                ElevatedButton(
                  onPressed: () {},
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.transparent,
                    shadowColor: Colors.transparent,
                  ),
                  child: Column(
                    children: [
                      Container(
                        width: 42,
                        height: 42,
                        clipBehavior: Clip.antiAlias,
                        decoration: ShapeDecoration(
                          shape: RoundedRectangleBorder(
                            side: const BorderSide(
                                width: 1.43, color: ThemeColors.mainColor),
                            borderRadius: BorderRadius.circular(18),
                          ),
                        ),
                        child: Image.asset(
                            'assets/image/social_lobby_to_real.png'),
                      ),
                      const SizedBox(height: 3),
                      const Text('From')
                    ],
                  ),
                ),
                const SizedBox(height: 16),
                ElevatedButton(
                  onPressed: () {},
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.transparent,
                    shadowColor: Colors.transparent,
                  ),
                  child: const Icon(TPFiveIcon.mute),
                ),
                ElevatedButton(
                  onPressed: () {},
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.transparent,
                    shadowColor: Colors.transparent,
                  ),
                  child: const Icon(TPFiveIcon.more_verticle),
                ),
              ],
            ),
          ),
          Positioned(
            bottom: 50,
            left: 0,
            right: 0,
            child: Container(
              padding: const EdgeInsets.all(20),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Title text',
                    style: TextStyle(
                      decoration: TextDecoration.none,
                      color: Colors.white,
                      fontSize: 12,
                      fontFamily: 'Poppins',
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  const Text(
                    'description sadfsadfadasdas',
                    style: TextStyle(
                      decoration: TextDecoration.none,
                      color: Colors.white,
                      fontSize: 12,
                      fontFamily: 'Poppins',
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  SizedBox(
                    width: 270,
                    child: ExpandableText(
                      'Title text Description here The maximum number of characters that can be entered in the input field. Default is 200 characters. Thank you! ',
                      expandText: '',
                      style: const TextStyle(
                        decoration: TextDecoration.none,
                        color: Colors.white,
                        fontSize: 12,
                        fontFamily: 'Poppins',
                        fontWeight: FontWeight.w500,
                      ),
                      maxLines: 1,
                      linkColor: Colors.white,
                      animation: true,
                      collapseOnTextTap: true,
                      expandOnTextTap: true,
                      onExpandedChanged: (value) {
                        _threeDReelRecordingPageViewModel
                            .setDescriptionExpanded(value);
                      },
                    ),
                  ),
                  const SizedBox(height: 10),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Container(
                        width: 42,
                        height: 42,
                        clipBehavior: Clip.antiAlias,
                        decoration: ShapeDecoration(
                          shape: RoundedRectangleBorder(
                            side: const BorderSide(
                                width: 1.43, color: ThemeColors.mainColor),
                            borderRadius: BorderRadius.circular(18),
                          ),
                        ),
                        child: Image.asset(
                            'assets/image/social_lobby_to_real.png'),
                      ),
                      const SizedBox(width: 10),
                      const Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            Text(
                              'Creator name',
                              style: TextStyle(
                                decoration: TextDecoration.none,
                                color: Colors.white,
                                fontSize: 18,
                                fontFamily: 'Poppins',
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                            SizedBox(height: 5),
                            Row(
                              mainAxisSize: MainAxisSize.min,
                              mainAxisAlignment: MainAxisAlignment.start,
                              crossAxisAlignment: CrossAxisAlignment.center,
                              children: [
                                Icon(
                                  TPFiveIcon.mute_speaker,
                                  color: ThemeColors.mainColor,
                                  size: 15,
                                ),
                                SizedBox(width: 2),
                                Expanded(
                                  child: SizedBox(
                                    child: Text(
                                      'Song name here - Singer name',
                                      style: TextStyle(
                                        decoration: TextDecoration.none,
                                        color: Colors.white,
                                        fontSize: 10,
                                        fontFamily: 'Poppins',
                                        fontWeight: FontWeight.w400,
                                      ),
                                    ),
                                  ),
                                ),
                              ],
                            )
                          ],
                        ),
                      ),
                      Container(
                        width: 70,
                        height: 32,
                        decoration: ShapeDecoration(
                          color: ThemeColors.mainColor,
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(99),
                          ),
                        ),
                        child: const Center(
                          child: Text(
                            'Follow',
                            style: TextStyle(
                              decoration: TextDecoration.none,
                              color: Colors.black,
                              fontSize: 11,
                              fontFamily: 'Poppins',
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                      )
                    ],
                  )
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget buildCoCreateScreen() {
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
            Log.e(CoCreatePage.tag, 'count down error: $e');
          }
        });
        break;
      case RecordStateType.RECORDING:
        button = RecordingButton(
            onTap: () {},
            isCoCreate: true,
            originReelVideoSec: _threeDReelRecordingPageViewModel.reelVideoSec);
        break;
      case RecordStateType.PREVIEW:
        button = AutoPreviewWidget(
            isCoCreate: true,
            originReelVideoSec: _threeDReelRecordingPageViewModel.reelVideoSec);
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
                          await Navigator.pushNamed(
                              context, ShareReel.routeName, arguments: {
                            'filePaths':
                                _threeDReelRecordingPageViewModel.filePaths
                          });
                          setRecordType(RecordStateType.DONE);
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
                                  color: computeTrackingIconColor(
                                      faceTracking, faceTrackingProgress)),
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
                                    color: computeTrackingIconColor(
                                        bodyTracking, bodyTrackingProgress))),
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
                    isCoCreate: true,
                    showAIGCTab: checkToShowAIGCTab(),
                    motionButtonActive: _threeDReelRecordingPageViewModel
                        .reelSceneInfo.motionButtonActive,
                  ),
                ),
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
    Log.d(CoCreatePage.tag,
        'Received message from unity: ${message.toString()}, ${currentStateType.toString()}');

    if (message.errorCode == ErrorCode.SUCCESS) {
      setRecordType(RecordStateType.PRESET);
      _threeDReelRecordingPageViewModel.setCoCreateMode(CoCreateMode.VIEW);
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

  bool checkToShowAIGCTab() {
    if (widget.reelData.musicToMotionUrl == null) {
      return true;
    } else {
      if (widget.reelData.musicToMotionUrl!.isEmpty) {
        return true;
      } else {
        return false;
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
