import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/three_d_reels/models/switch_option.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';
import 'package:tpfive/feature/three_d_reels/presentations/impor_music_modal.dart';
import 'package:tpfive/feature/three_d_reels/presentations/motion_menu_modal.dart';
import 'package:tpfive/utils/permission_helper.dart';

class ReelsFunction extends ConsumerStatefulWidget {
  final bool isCoCreate;
  final bool showAIGCTab;
  final bool motionButtonActive;
  const ReelsFunction(
      {super.key,
      this.isCoCreate = false,
      this.showAIGCTab = true,
      this.motionButtonActive = true});

  @override
  ReelsFunctionState createState() => ReelsFunctionState();
}

class ReelsFunctionState extends ConsumerState<ReelsFunction>
    with TickerProviderStateMixin {
  late ThreeDReelRecordingPageViewModel _threeDReelRecordingPageViewModel;
  late AnimationController controller;

  @override
  void initState() {
    super.initState();
    _threeDReelRecordingPageViewModel = ThreeDReelRecordingPageViewModel(ref);
    initController();
  }

  void initController() {
    controller = BottomSheet.createAnimationController(this);
    controller.duration = const Duration(milliseconds: 100);
    controller.reverseDuration = const Duration(milliseconds: 100);
  }

  @override
  Widget build(BuildContext context) {
    final isMicActivate = ref.watch(micActivate);
    final isImportMusic = ref.watch(hasImportMusic);
    return Column(
      children: [
        Visibility(
          visible: !widget.isCoCreate,
          child: ElevatedButton(
            onPressed: () {
              showModalBottomSheet(
                  transitionAnimationController: controller,
                  isScrollControlled: true,
                  context: context,
                  shape: ThemeStyle.modalBorder,
                  builder: (builder) {
                    return const ImportMusicModal();
                  }).whenComplete(() => initController());
            },
            style: ElevatedButton.styleFrom(
              shape: const CircleBorder(),
              padding: const EdgeInsets.all(10),
              backgroundColor: Colors.black.withOpacity(0.2),
            ),
            child: Icon(isImportMusic ? TPFiveIcon.music : TPFiveIcon.musicAdd,
                color: isImportMusic
                    ? Theme.of(context).primaryColor
                    : Colors.white),
          ),
        ),
        Visibility(
          visible: widget.motionButtonActive,
          child: ElevatedButton(
            onPressed: () {
              showModalBottomSheet(
                  backgroundColor: Colors.black.withOpacity(0.3),
                  barrierColor: Colors.transparent,
                  transitionAnimationController: controller,
                  isScrollControlled: true,
                  context: context,
                  shape: ThemeStyle.modalBorder,
                  builder: (builder) {
                    return MotionMenuModal(showAIGCTab: widget.showAIGCTab);
                  }).whenComplete(() => initController());
            },
            style: ElevatedButton.styleFrom(
              shape: const CircleBorder(),
              padding: const EdgeInsets.all(10),
              backgroundColor: Colors.black.withOpacity(0.2),
            ),
            child: const Icon(TPFiveIcon.motion, color: Colors.white),
          ),
        ),
        ElevatedButton(
          onPressed: () async {
            if (await PermissionHelper.checkPermission(Permission.microphone)) {
              bool isMicActivateState = ref.read(micActivate.notifier).state;
              _threeDReelRecordingPageViewModel
                  .sendSetMicToUnity(SwitchOption(mode: !isMicActivateState));
            }
          },
          style: ElevatedButton.styleFrom(
            shape: const CircleBorder(),
            padding: const EdgeInsets.all(10),
            backgroundColor: Colors.black.withOpacity(0.2),
          ),
          child: Icon(
              isMicActivate ? TPFiveIcon.speaker : TPFiveIcon.mute_speaker,
              color: Colors.white),
        )
      ],
    );
  }
}
