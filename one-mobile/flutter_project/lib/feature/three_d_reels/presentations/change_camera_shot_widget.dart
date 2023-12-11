import 'dart:math';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';

class ChangeCameraShotWidget extends ConsumerStatefulWidget {
  const ChangeCameraShotWidget({super.key});

  @override
  ChangeCameraShotWidgetState createState() => ChangeCameraShotWidgetState();
}

class ChangeCameraShotWidgetState
    extends ConsumerState<ChangeCameraShotWidget> {
  late ThreeDReelRecordingPageViewModel _threeDReelRecordingPageViewModel;
  static const trackIcons = [
    TPFiveIcon.camera_track_1,
    TPFiveIcon.camera_track_2,
    TPFiveIcon.camera_track_3,
    TPFiveIcon.camera_track_4,
    TPFiveIcon.camera_track_5
  ];
  @override
  void initState() {
    super.initState();
    _threeDReelRecordingPageViewModel = ThreeDReelRecordingPageViewModel(ref);
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final selectCameraIndex = ref.watch(selectedCameraIndex);
    final int tracks = ref.watch(trackCounts);

    const buttonSize = 64.0;
    return SizedBox(
      height: 120,
      child: Column(
        children: [
          Text(
            'Change camera shot ',
            style: GoogleFonts.poppins(
              textStyle: const TextStyle(
                color: Colors.white,
                fontSize: 14,
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
          const SizedBox(
            height: 20,
          ),
          SizedBox(
              width: tracks * buttonSize,
              height: 60,
              child: Row(
                mainAxisSize: MainAxisSize.min,
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: List.generate(
                  min(tracks, trackIcons.length),
                  (i) => Expanded(
                    child: ElevatedButton(
                      onPressed: () {
                        _threeDReelRecordingPageViewModel.startPreview(true,
                            onStart: () => {
                                  _threeDReelRecordingPageViewModel
                                      .setTrackOfCurrentScene(i)
                                },
                            isLoop: true);
                      },
                      style: ElevatedButton.styleFrom(
                        fixedSize: const Size(buttonSize, buttonSize),
                        shape: const RoundedRectangleBorder(
                          borderRadius: BorderRadius.all(Radius.circular(28.0)),
                        ),
                        padding: const EdgeInsets.all(0),
                        backgroundColor: selectCameraIndex == i
                            ? ThemeColors.mainColor
                            : Colors.transparent,
                        foregroundColor: Colors.transparent,
                        shadowColor: Colors.transparent,
                      ),
                      child: Icon(
                        trackIcons[i],
                        color: selectCameraIndex == i
                            ? Colors.black
                            : Colors.white.withOpacity(0.4),
                        size: 20,
                      ),
                    ),
                  ),
                ),
              ))
        ],
      ),
    );
  }
}
