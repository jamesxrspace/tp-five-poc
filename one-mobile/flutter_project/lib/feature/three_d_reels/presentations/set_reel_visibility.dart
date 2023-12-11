import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_translate/flutter_translate.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/feature/three_d_reels/models/set_reel_visibility_view_model.dart';

class SetReelVisibility extends ConsumerStatefulWidget {
  static const routeName = '/setReelVisibility';
  static const tag = 'setReelVisibility';

  const SetReelVisibility({super.key});

  @override
  SetReelVisibilityState createState() => SetReelVisibilityState();
}

class SetReelVisibilityState extends ConsumerState<SetReelVisibility> {
  late SetReelVisibilityViewModel _setReelVisibilityViewModel;

  int? selectedPermission = 0;

  final itemList = ['All', 'My Friends/Followers', 'Only Me'];

  @override
  void initState() {
    super.initState();
    _setReelVisibilityViewModel = SetReelVisibilityViewModel(ref);
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    bool isJoinMode = ref.watch(joinModeEnable);

    List<Widget> scrollChild = [];

    scrollChild.addAll([
      const SliverToBoxAdapter(
        child: SizedBox(height: 15),
      ),
      SliverToBoxAdapter(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20),
          child: InkWell(
            onTap: () {
              Navigator.pushNamed(context, SetReelVisibility.routeName);
            },
            child: Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Join Mode',
                        style: GoogleFonts.poppins(
                          textStyle: const TextStyle(
                            color: Colors.black,
                            fontSize: 16,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                      Text(
                        'Join Mode 表示其他人可以加入你的Reel，並產生新的創作內容',
                        style: GoogleFonts.poppins(
                          textStyle: const TextStyle(
                            color: Color.fromRGBO(128, 128, 128, 1),
                            fontSize: 13,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      )
                    ],
                  ),
                ),
                CupertinoSwitch(
                  value: isJoinMode,
                  onChanged: (value) {
                    _setReelVisibilityViewModel.setJoinModeEnable(value);
                  },
                  activeColor: Colors.black,
                  trackColor: const Color.fromRGBO(204, 204, 204, 1),
                  thumbColor: isJoinMode ? ThemeColors.mainColor : Colors.white,
                ),
              ],
            ),
          ),
        ),
      ),
      const SliverToBoxAdapter(
        child: Divider(),
      )
    ]);

    if (isJoinMode) {
      for (int i = 0; i < 3; i++) {
        scrollChild.add(
          SliverToBoxAdapter(
            child: permissionItem(itemList[i], i),
          ),
        );
      }
    }

    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        toolbarHeight: 68,
        backgroundColor: const Color.fromRGBO(237, 238, 242, 1),
        centerTitle: false,
        leading: ElevatedButton(
          onPressed: () {
            Navigator.pop(context);
          },
          style: ElevatedButton.styleFrom(
            shape: const CircleBorder(),
            padding: const EdgeInsets.all(5),
            backgroundColor: Colors.transparent,
            foregroundColor: Colors.transparent,
            shadowColor: Colors.transparent,
          ),
          child: const Icon(
            TPFiveIcon.arrowLeft,
            color: Colors.black,
            size: 20,
          ),
        ),
        title: Text(
          'Set Visibility',
          style: GoogleFonts.poppins(
            textStyle: const TextStyle(
              color: Colors.black,
              fontSize: 23,
              fontWeight: FontWeight.w800,
            ),
          ),
        ),
        shadowColor: Colors.transparent,
      ),
      body: SafeArea(
        child: CustomScrollView(
          slivers: scrollChild,
        ),
      ),
    );
  }

  Widget permissionItem(String itemName, int index) {
    return Theme(
      data: Theme.of(context).copyWith(
        unselectedWidgetColor: const Color.fromRGBO(204, 204, 204, 1),
      ),
      child: Row(
        children: [
          Radio(
            value: index,
            groupValue: selectedPermission,
            onChanged: (int? index) {
              setState(() {
                selectedPermission = index;
              });
            },
            activeColor: ThemeColors.mainColor,
            hoverColor: Colors.blue,
          ),
          const SizedBox(width: 5),
          Text(
            translate(itemName),
            style: GoogleFonts.poppins(
              textStyle: const TextStyle(
                color: Colors.black,
                fontSize: 16,
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
