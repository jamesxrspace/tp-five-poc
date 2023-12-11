import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';

class PresetScreen extends ConsumerStatefulWidget {
  const PresetScreen({super.key});

  @override
  PresetScreenState createState() => PresetScreenState();
}

class PresetScreenState extends ConsumerState<PresetScreen> {
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
    return SafeArea(
      child: Stack(
        children: [
          Positioned(
            left: 20,
            top: 20,
            child: Image.asset('assets/image/asset_frame_left_top.png'),
          ),
          Positioned(
            right: 20,
            top: 20,
            child: Image.asset('assets/image/asset_frame_right_top.png'),
          ),
          Positioned(
            left: 20,
            bottom: 77,
            child: Image.asset('assets/image/asset_frame_left_bottom.png'),
          ),
          Positioned(
            right: 20,
            bottom: 77,
            child: Image.asset('assets/image/asset_frame_right_bottom.png'),
          ),
          Positioned(
            bottom: 140,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                'Tap button to set the fixed camera',
                style: GoogleFonts.poppins(
                  textStyle: const TextStyle(
                    decoration: TextDecoration.none,
                    fontSize: 11,
                    fontWeight: FontWeight.w500,
                    color: Colors.white,
                  ),
                ),
              ),
            ),
          )
        ],
      ),
    );
  }
}
