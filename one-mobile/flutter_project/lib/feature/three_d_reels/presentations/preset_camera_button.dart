import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';

class PresetCameraButton extends ConsumerStatefulWidget {
  final VoidCallback onTap;
  const PresetCameraButton({super.key, required this.onTap});

  @override
  PresetCameraButtonState createState() => PresetCameraButtonState();
}

class PresetCameraButtonState extends ConsumerState<PresetCameraButton> {
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
    return Center(
      child: GestureDetector(
        onTap: widget.onTap,
        child: Container(
          decoration: BoxDecoration(
            color: Colors.transparent,
            shape: BoxShape.circle,
            border: Border.all(
              color: ThemeColors.mainColor,
              width: 3.0,
            ),
          ),
          child: Container(
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
              decoration: const BoxDecoration(
                color: Colors.transparent,
                shape: BoxShape.circle,
              ),
              child: const Icon(
                TPFiveIcon.camera,
                color: Colors.white,
                size: 30,
              ),
            ),
          ),
        ),
      ),
    );
  }
}
