import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';

class AutoPreviewWidget extends ConsumerStatefulWidget {
  final bool isCoCreate;
  final double originReelVideoSec;
  const AutoPreviewWidget(
      {super.key, this.isCoCreate = false, this.originReelVideoSec = 30});

  @override
  AutoPreviewWidgetState createState() => AutoPreviewWidgetState();
}

class AutoPreviewWidgetState extends ConsumerState<AutoPreviewWidget> {
  late ThreeDReelRecordingPageViewModel _threeDReelRecordingPageViewModel;

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
    final double countdownRecordingNum;

    countdownRecordingNum = widget.isCoCreate
        ? widget.originReelVideoSec
        : ref.read(countdownRecordingNumber);

    final process = ref.watch(previewProcessNumber);

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 20),
      height: 100,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          ElevatedButton(
            onPressed: () {
              _threeDReelRecordingPageViewModel.startPreview(true,
                  isCoCreate: widget.isCoCreate);
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.transparent,
              foregroundColor: Colors.transparent,
              shadowColor: Colors.transparent,
            ),
            child: Icon(
              TPFiveIcon.pause,
              color:
                  Theme.of(context).extension<ColorPalette>()?.grey?.shade100,
              size: 20,
            ),
          ),
          Padding(
            padding: const EdgeInsets.only(bottom: 10),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                SizedBox(
                  width: 250,
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        '00:${process.floor().toString().padLeft(2, '0')}',
                        style: const TextStyle(color: Colors.white),
                      ),
                      Text(
                        '00:${countdownRecordingNum.floor().toString().padLeft(2, '0')}',
                        style: const TextStyle(color: Colors.white),
                      ),
                    ],
                  ),
                ),
                SizedBox(
                  width: 250,
                  child: LinearProgressIndicator(
                    value: process / countdownRecordingNum,
                    backgroundColor: Colors.transparent,
                    valueColor: AlwaysStoppedAnimation<Color>(
                        Theme.of(context).primaryColor),
                  ),
                )
              ],
            ),
          )
        ],
      ),
    );
  }
}
