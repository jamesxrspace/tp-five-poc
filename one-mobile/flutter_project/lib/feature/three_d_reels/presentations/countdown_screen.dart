import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';

class CountdownScreen extends ConsumerStatefulWidget {
  const CountdownScreen({super.key});

  @override
  CountdownScreenState createState() => CountdownScreenState();
}

class CountdownScreenState extends ConsumerState<CountdownScreen> {
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
    final countdownNum = ref.watch(countdownNumber);
    return Container(
      color: Colors.black.withOpacity(0.5),
      child: Center(
        child: Stack(
          alignment: Alignment.center,
          children: [
            SizedBox(
              height: 104,
              width: 104,
              child: CircularProgressIndicator(
                strokeWidth: 4,
                backgroundColor: Colors.white,
                color: ThemeColors.mainColor,
                value: (3 - countdownNum) / 3,
              ),
            ),
            Center(
              child: Text(
                '${countdownNum.ceil()}',
                style: GoogleFonts.poppins(
                  textStyle: const TextStyle(
                    decoration: TextDecoration.none,
                    fontSize: 48,
                    fontWeight: FontWeight.w600,
                    color: Colors.white,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
