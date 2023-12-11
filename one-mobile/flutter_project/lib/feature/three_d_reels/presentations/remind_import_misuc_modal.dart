import 'package:flutter/material.dart';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/feature/three_d_reels/presentations/impor_music_modal.dart';

class RemindImportMusicModal extends ConsumerStatefulWidget {
  const RemindImportMusicModal({super.key});

  @override
  RemindImportMusicModalState createState() => RemindImportMusicModalState();
}

class RemindImportMusicModalState extends ConsumerState<RemindImportMusicModal>
    with TickerProviderStateMixin {
  late AnimationController controller;
  final RoundedRectangleBorder modalBorder = const RoundedRectangleBorder(
    borderRadius: BorderRadius.only(
      topLeft: Radius.circular(30.0),
      topRight: Radius.circular(30.0),
    ),
  );

  @override
  void initState() {
    super.initState();
    initController();
  }

  void initController() {
    controller = BottomSheet.createAnimationController(this);
    controller.duration = const Duration(milliseconds: 100);
    controller.reverseDuration = const Duration(milliseconds: 100);
  }

  @override
  Widget build(BuildContext context) {
    return Consumer(
      builder: (BuildContext context, WidgetRef ref, Widget? child) {
        return Padding(
          padding: const EdgeInsets.symmetric(horizontal: 30),
          child: SizedBox(
            height: 300,
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
                            'No music detected.',
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
                            'Please choose an audio first to create the AIGC motions.',
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
                                showModalBottomSheet(
                                    transitionAnimationController: controller,
                                    isScrollControlled: true,
                                    context: context,
                                    shape: modalBorder,
                                    builder: (builder) {
                                      return const ImportMusicModal();
                                    }).whenComplete(() => initController());
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
                                'Import Audio',
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
                              onPressed: () {
                                Navigator.pop(context);
                              },
                              style: ElevatedButton.styleFrom(
                                backgroundColor:
                                    const Color.fromRGBO(238, 238, 238, 1),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(20.0),
                                ),
                                shadowColor: Colors.transparent,
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 12, vertical: 10),
                              ),
                              child: Text(
                                'Cancel',
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
