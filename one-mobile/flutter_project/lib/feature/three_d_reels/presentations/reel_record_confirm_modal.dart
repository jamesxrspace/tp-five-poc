import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/style.dart';

class ReelRecordConfirmModal extends ConsumerStatefulWidget {
  final VoidCallback onStartOver, onExit;
  const ReelRecordConfirmModal(
      {super.key, required this.onStartOver, required this.onExit});

  @override
  MotionMenuModalState createState() => MotionMenuModalState();
}

class MotionMenuModalState extends ConsumerState<ReelRecordConfirmModal>
    with TickerProviderStateMixin {
  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Consumer(
      builder: (BuildContext context, WidgetRef ref, Widget? child) {
        return Padding(
          padding: const EdgeInsets.symmetric(horizontal: 30),
          child: SizedBox(
            height: 380,
            child: Column(
              children: [
                const SizedBox(height: 10),
                Expanded(
                  child: Stack(
                    alignment: AlignmentDirectional.topCenter,
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const SizedBox(height: 30),
                          Text(
                            'Quit 3D reel recording now?',
                            style: GoogleFonts.poppins(
                              textStyle: const TextStyle(
                                color: Colors.black,
                                fontSize: 18,
                                fontWeight: FontWeight.w700,
                              ),
                            ),
                          ),
                          const SizedBox(height: 16),
                          Text(
                            'You haven’t finished this 3D reel creation yet. This means that you will discard this recording.',
                            style: GoogleFonts.poppins(
                              textStyle: const TextStyle(
                                color: Color.fromRGBO(102, 102, 102, 1),
                                fontSize: 15,
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                          ),
                          const SizedBox(height: 25),
                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton(
                              onPressed: () {
                                Navigator.pop(context);
                              },
                              style: ElevatedButton.styleFrom(
                                backgroundColor: ThemeColors.mainColor,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(20.0),
                                ),
                                shadowColor: Colors.transparent,
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 12, vertical: 10),
                              ),
                              child: Text(
                                'Keep Editing',
                                style: GoogleFonts.poppins(
                                  textStyle: const TextStyle(
                                    color: Colors.black,
                                    fontSize: 16,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                              ),
                            ),
                          ),
                          const SizedBox(height: 5),
                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton(
                              onPressed: widget.onStartOver,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Theme.of(context)
                                    .extension<ColorPalette>()
                                    ?.grey
                                    ?.shade50,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(20.0),
                                ),
                                shadowColor: Colors.transparent,
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 12, vertical: 10),
                              ),
                              child: Text(
                                'Start Over',
                                style: GoogleFonts.poppins(
                                  textStyle: const TextStyle(
                                    color: Colors.black,
                                    fontSize: 16,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                              ),
                            ),
                          ),
                          const SizedBox(height: 5),
                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton(
                              onPressed: widget.onExit,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Theme.of(context)
                                    .extension<ColorPalette>()
                                    ?.grey
                                    ?.shade50,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(20.0),
                                ),
                                shadowColor: Colors.transparent,
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 12, vertical: 10),
                              ),
                              child: Text(
                                'Exit',
                                style: GoogleFonts.poppins(
                                  textStyle: const TextStyle(
                                    color: Colors.black,
                                    fontSize: 16,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                              ),
                            ),
                          ),
                        ],
                      ),
                      Positioned(
                        bottom: 9,
                        child: Align(
                          alignment: Alignment.center,
                          child: Container(
                            width: 134,
                            height: 5,
                            decoration: BoxDecoration(
                              color: Colors.black,
                              borderRadius: BorderRadius.circular(20.0),
                            ),
                          ),
                        ),
                      )
                    ],
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
