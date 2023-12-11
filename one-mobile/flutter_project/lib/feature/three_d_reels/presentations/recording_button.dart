import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';

double size = 70;

class RecordingButton extends ConsumerStatefulWidget {
  final VoidCallback onTap;
  final bool isCoCreate;
  final double originReelVideoSec;
  const RecordingButton(
      {super.key,
      required this.onTap,
      this.isCoCreate = false,
      this.originReelVideoSec = 30});

  @override
  RecordingButtonState createState() => RecordingButtonState();
}

class RecordingButtonState extends ConsumerState<RecordingButton> {
  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return widget.isCoCreate
        ? coCreateRecordingButton()
        : normalRecordingButton();
  }

  Widget normalRecordingButton() {
    final countdownRecordingNum = ref.watch(countdownRecordingNumber);
    final process = ref.read(recordProcessNumber);

    return GestureDetector(
      onTap: widget.onTap,
      child: Column(
        children: [
          Container(
            width: 50,
            height: 25,
            decoration: BoxDecoration(
              color: Theme.of(context).extension<ColorPalette>()?.record,
              borderRadius: BorderRadius.circular(4),
            ),
            child: Center(
              child: Text(
                '00:${countdownRecordingNum.floor().toString().padLeft(2, '0')}',
                style: GoogleFonts.poppins(
                  textStyle: const TextStyle(
                    decoration: TextDecoration.none,
                    color: Colors.white,
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ),
          ),
          const SizedBox(height: 10),
          Container(
            width: size,
            height: size,
            padding: const EdgeInsets.all(2),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(100),
              color: Colors.transparent,
            ),
            child: Stack(
              alignment: Alignment.center,
              children: [
                SizedBox(
                  height: size,
                  width: size,
                  child: CircularProgressIndicator(
                    strokeWidth: 4,
                    backgroundColor: Colors.white,
                    color: ThemeColors.mainColor,
                    value: process,
                  ),
                ),
                const Icon(
                  TPFiveIcon.stop,
                  size: 32,
                  color: Colors.red,
                ),
              ],
            ),
          )
        ],
      ),
    );
  }

  Widget coCreateRecordingButton() {
    final countdownRecordingNum =
        widget.originReelVideoSec - ref.watch(countdownRecordingNumber);
    final process = ref.read(recordProcessNumber);

    return Stack(
      alignment: Alignment.center,
      children: [
        SizedBox(
          height: size,
          width: size,
          child: CircularProgressIndicator(
            strokeWidth: 4,
            backgroundColor: Colors.white,
            color: ThemeColors.mainColor,
            value: process,
          ),
        ),
        Container(
          decoration: BoxDecoration(
            color: Colors.transparent,
            shape: BoxShape.circle,
            border: Border.all(
              color: Colors.transparent,
              width: 2.0,
            ),
          ),
          child: Container(
            width: 60.0,
            height: 60.0,
            decoration: BoxDecoration(
              color: Colors.black.withOpacity(0.2),
              shape: BoxShape.circle,
            ),
            child: Center(
              child: Text(
                '00:${countdownRecordingNum.ceil().toString().padLeft(2, '0')}',
                style: GoogleFonts.poppins(
                  textStyle: const TextStyle(
                    decoration: TextDecoration.none,
                    color: ThemeColors.mainColor,
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}
