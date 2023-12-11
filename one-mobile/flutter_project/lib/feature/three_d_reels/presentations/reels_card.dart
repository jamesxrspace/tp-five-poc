import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/feature/three_d_reels/models/create_three_d_reels_view_model.dart';
import 'package:tpfive/feature/three_d_reels/models/reel_scene_model.dart';
import 'package:widget_mask/widget_mask.dart';

class ReelsCard extends ConsumerStatefulWidget {
  final dynamic reel;
  final int index;
  final ReelSceneModel reelSceneModel;
  final CreateThreeDRealsViewModel viewModel;
  final bool isSelected;

  const ReelsCard({
    super.key,
    this.reel,
    required this.reelSceneModel,
    required this.viewModel,
    required this.index,
    required this.isSelected,
  });

  @override
  ReelCardState createState() => ReelCardState();
}

class ReelCardState extends ConsumerState<ReelsCard> {
  final RoundedRectangleBorder selectedBorder = RoundedRectangleBorder(
    side: const BorderSide(color: ThemeColors.mainColor, width: 3.0),
    borderRadius: BorderRadius.circular(29.0),
  );
  final RoundedRectangleBorder unSelectedBorder = RoundedRectangleBorder(
    side: const BorderSide(color: Colors.transparent, width: 3.0),
    borderRadius: BorderRadius.circular(32.0),
  );

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      height: 125,
      child: Builder(
        builder: (context) {
          return InkWell(
            onTap: () async {
              widget.viewModel.setScene(widget.index);
            },
            child: Card(
              color: Colors.white,
              shape: widget.isSelected ? selectedBorder : unSelectedBorder,
              child: Padding(
                padding: const EdgeInsets.only(right: 0.0),
                child: Stack(
                  children: [
                    Positioned(
                      right: 0,
                      bottom: 0,
                      top: 0,
                      child: Container(
                        child: _eventMaskImage(widget.reelSceneModel.thumbNail!,
                            'assets/image/asset_scene_mask.png'),
                      ),
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Padding(
                          padding: const EdgeInsets.symmetric(
                              vertical: 10, horizontal: 10),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              const SizedBox(height: 3),
                              Padding(
                                padding: const EdgeInsets.only(left: 2.0),
                                child: Row(
                                  crossAxisAlignment: CrossAxisAlignment.center,
                                  children: [
                                    const SizedBox(width: 3),
                                    Text(
                                      widget.reelSceneModel.participantLimit!,
                                      style: const TextStyle(
                                          fontSize: 16,
                                          fontWeight: FontWeight.w700,
                                          color: Colors.black),
                                    ),
                                  ],
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _eventMaskImage(String src, String mask) {
    return WidgetMask(
      blendMode: BlendMode.srcATop,
      childSaveLayer: true,
      mask: Image.asset(
        src,
        fit: BoxFit.fill,
      ),
      child: Image.asset(
        mask,
        fit: BoxFit.fill,
      ),
    );
  }
}
