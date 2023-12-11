import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/three_d_reels/models/switch_option.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';
import 'package:tpfive/feature/three_d_reels/presentations/remind_import_misuc_modal.dart';

class MotionMenuModal extends ConsumerStatefulWidget {
  final bool showAIGCTab;
  const MotionMenuModal({super.key, this.showAIGCTab = true});

  @override
  MotionMenuModalState createState() => MotionMenuModalState();
}

class MotionMenuModalState extends ConsumerState<MotionMenuModal>
    with TickerProviderStateMixin {
  late ThreeDReelRecordingPageViewModel _threeDReelRecordingPageViewModel;
  late TabController _tabController;
  late AnimationController controller;

  final RoundedRectangleBorder unSelectedBorder = RoundedRectangleBorder(
    side: const BorderSide(color: Colors.transparent, width: 1.0),
    borderRadius: BorderRadius.circular(24.0),
  );
  final RoundedRectangleBorder selectedBorder = RoundedRectangleBorder(
    side: const BorderSide(color: ThemeColors.mainColor, width: 3.0),
    borderRadius: BorderRadius.circular(28.0),
  );
  final RoundedRectangleBorder modalBorder = const RoundedRectangleBorder(
    borderRadius: BorderRadius.only(
      topLeft: Radius.circular(30.0),
      topRight: Radius.circular(30.0),
    ),
  );

  @override
  void initState() {
    super.initState();
    _threeDReelRecordingPageViewModel = ThreeDReelRecordingPageViewModel(ref);
    _tabController =
        TabController(length: widget.showAIGCTab ? 3 : 2, vsync: this);
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
          padding: const EdgeInsets.symmetric(horizontal: 20),
          child: SizedBox(
            height: 300,
            child: Column(
              children: [
                const SizedBox(height: 10),
                Align(
                  alignment: Alignment.center,
                  child: Container(
                    width: 32,
                    height: 5,
                    decoration: BoxDecoration(
                      color: const Color.fromRGBO(170, 170, 170, 1),
                      borderRadius: BorderRadius.circular(20.0),
                    ),
                  ),
                ),
                const SizedBox(height: 10),
                TabBar(
                  indicatorSize: TabBarIndicatorSize.tab,
                  indicatorColor: ThemeColors.mainColor,
                  labelColor: ThemeColors.mainColor,
                  labelStyle: const TextStyle(color: Colors.red),
                  unselectedLabelColor: Colors.white,
                  unselectedLabelStyle: const TextStyle(color: Colors.black),
                  controller: _tabController,
                  tabs: [
                    if (widget.showAIGCTab)
                      const Tab(
                        height: 34,
                        text: 'AIGC',
                      ),
                    const Tab(height: 34, text: 'Motion'),
                    const Tab(height: 34, text: 'My UGC'),
                  ],
                ),
                Expanded(
                  child: Stack(
                    alignment: AlignmentDirectional.topCenter,
                    children: [
                      TabBarView(
                        controller: _tabController,
                        children: <Widget>[
                          if (widget.showAIGCTab) aigcChild(ref),
                          motionChild(ref),
                          myUGCChild(ref),
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
                              color: Colors.white,
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

  Widget aigcChild(WidgetRef ref) {
    final isImportMusic = ref.watch(hasImportMusic);
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        ElevatedButton(
          onPressed: () async {
            if (!isImportMusic) {
              showModalBottomSheet(
                  transitionAnimationController: controller,
                  isScrollControlled: true,
                  context: context,
                  shape: modalBorder,
                  builder: (builder) {
                    return const RemindImportMusicModal();
                  }).whenComplete(() => initController());
            } else {
              await _threeDReelRecordingPageViewModel
                  .sendStartAIGCToUnity(SwitchOption(mode: true));

              if (!context.mounted) return;

              Navigator.pop(context);
            }
          },
          style: ElevatedButton.styleFrom(
            backgroundColor: ThemeColors.mainColor,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(20.0),
            ),
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              const SizedBox(width: 4),
              Text(
                'Start AIGC',
                style: GoogleFonts.poppins(
                  textStyle: const TextStyle(
                    color: Colors.black,
                    fontSize: 11,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
              const Icon(
                TPFiveIcon.motionAIGC,
                color: Colors.black,
                size: 16,
              ),
              const SizedBox(width: 4),
            ],
          ),
        ),
      ],
    );
  }

  Widget motionChild(WidgetRef ref) {
    return CustomScrollView(
      slivers: [
        const SliverToBoxAdapter(
          child: SizedBox(height: 10),
        ),
        SliverGrid(
          gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 3,
            mainAxisSpacing: 10.0,
            crossAxisSpacing: 10.0,
            childAspectRatio: 1.0,
          ),
          delegate: SliverChildBuilderDelegate(
            childCount: 20,
            (BuildContext context, int index) {
              return Card(
                elevation: 2.0,
                child: Center(
                  child: Text('Item $index'),
                ),
              );
            },
          ),
        ),
      ],
    );
  }

  Widget myUGCChild(WidgetRef ref) {
    final selectedugcItem = ref.watch(selectedUGCItem);

    return CustomScrollView(
      slivers: [
        const SliverToBoxAdapter(
          child: SizedBox(height: 10),
        ),
        SliverGrid(
          gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 3,
            mainAxisSpacing: 10.0,
            crossAxisSpacing: 10.0,
            childAspectRatio: 1.0,
          ),
          delegate: SliverChildBuilderDelegate(
            childCount: 20,
            (BuildContext context, int index) {
              if (index == 0) {
                return InkWell(
                  onTap: () {},
                  child: Stack(
                    fit: StackFit.expand,
                    children: [
                      Card(
                        color: const Color.fromRGBO(255, 255, 255, 0.7),
                        shape: unSelectedBorder,
                        child: const Center(
                          child: Icon(
                            TPFiveIcon.add,
                            color: Color.fromRGBO(0, 0, 0, 0.2),
                            size: 24,
                          ),
                        ),
                      ),
                      Padding(
                        padding: const EdgeInsets.all(1),
                        child: Card(
                          margin: const EdgeInsets.all(0),
                          elevation: 0,
                          color: Colors.transparent,
                          shape: unSelectedBorder,
                        ),
                      )
                    ],
                  ),
                );
              } else {
                return myUGCItem(index, selectedugcItem == index);
              }
            },
          ),
        ),
      ],
    );
  }

  Widget myUGCItem(int index, bool isSelected) {
    return InkWell(
      onTap: () {
        _threeDReelRecordingPageViewModel.setUGCItem(index);
      },
      child: Stack(
        fit: StackFit.expand,
        children: [
          Card(
            color: const Color.fromRGBO(255, 255, 255, 0.7),
            shape: unSelectedBorder,
            child: Image.asset(
              'assets/image/event_badge.png',
              width: 12,
              height: 12,
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(1),
            child: Card(
              margin: const EdgeInsets.all(0),
              elevation: 0,
              color: Colors.transparent,
              shape: isSelected ? selectedBorder : unSelectedBorder,
            ),
          )
        ],
      ),
    );
  }
}
