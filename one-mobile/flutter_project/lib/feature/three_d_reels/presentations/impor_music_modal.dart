import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_translate/flutter_translate.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:get_it/get_it.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/services/mock_reel_service.dart';
import 'package:tpfive/utils/logger.dart';

class ImportMusicModal extends ConsumerStatefulWidget {
  const ImportMusicModal({super.key});
  static const tag = 'ImportMusicModal';

  @override
  ImportMusicModalState createState() => ImportMusicModalState();
}

class ImportMusicModalState extends ConsumerState<ImportMusicModal> {
  late ThreeDReelRecordingPageViewModel _threeDReelRecordingPageViewModel;
  final _searchFieldTextController = TextEditingController();
  Timer? _debounce;
  final MockReelService mockService = GetIt.I<MockReelService>();
  List<MusicData> musicList = [];
  int currentPlayingIndex = -1;
  int selectedMusicIndex = -1;

  @override
  void initState() {
    super.initState();

    _threeDReelRecordingPageViewModel = ThreeDReelRecordingPageViewModel(ref);
    Log.d(ImportMusicModal.tag, 'Start mock_music_list');
    mockService.onRequest(RequestOptions(path: 'mock_music_list')).then(
        (value) {
      setState(() {
        musicList = (value.data as List<dynamic>)
            .map((e) => musicDataFromJson(jsonEncode(e)))
            .toList();
      });
    }, onError: (error) => Log.d(ImportMusicModal.tag, error.toString()));
  }

  @override
  void dispose() {
    super.dispose();
    _threeDReelRecordingPageViewModel.stopMusic();
    _debounce?.cancel();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20),
      child: SizedBox(
        height: 700,
        child: Column(
          children: [
            const SizedBox(height: 10),
            Align(
              alignment: Alignment.center,
              child: Container(
                width: 32,
                height: 5,
                decoration: BoxDecoration(
                  color: const Color.fromRGBO(63, 63, 63, 1),
                  borderRadius: BorderRadius.circular(20.0),
                ),
              ),
            ),
            const SizedBox(height: 20),
            _buildSearchField(),
            const SizedBox(height: 20),
            Align(
              alignment: Alignment.centerRight,
              child: Text(
                'See More',
                style: GoogleFonts.poppins(
                  textStyle: const TextStyle(
                    color: Colors.black,
                    fontSize: 12,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
            ),
            Expanded(
              child: ListView.builder(
                padding: const EdgeInsets.only(bottom: 60),
                physics: const AlwaysScrollableScrollPhysics(),
                itemCount: musicList.length,
                itemBuilder: (context, index) {
                  return _songItem(index);
                },
              ),
            )
          ],
        ),
      ),
    );
  }

  Widget _buildSearchField() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 0),
      child: Container(
        height: 50,
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(24.0),
          border: Border.all(color: ThemeColors.mainColor),
          color: ThemeColors.mainColor,
        ),
        child: Container(
          alignment: Alignment.center,
          padding: const EdgeInsets.only(left: 12, right: 8),
          child: TextField(
            controller: _searchFieldTextController,
            onChanged: (value) async {
              if (_debounce?.isActive ?? false) _debounce?.cancel();
              _debounce = Timer(const Duration(milliseconds: 700), () async {});
            },
            style: const TextStyle(color: Colors.white),
            decoration: InputDecoration(
                prefixIcon: IconButton(
                  onPressed: () {},
                  icon: const Icon(TPFiveIcon.search),
                  color: Colors.black,
                  disabledColor: ThemeColors.iconColor,
                ),
                hintText: translate('Search Music'),
                border: InputBorder.none,
                hintStyle: const TextStyle(color: Colors.black, height: 2.0)),
          ),
        ),
      ),
    );
  }

  Widget _songItem(int index) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 10),
      child: Row(crossAxisAlignment: CrossAxisAlignment.start, children: [
        Image.asset(
          musicList[index].thumbnailPath,
          width: 42,
          height: 42,
        ),
        const SizedBox(width: 10),
        Expanded(
          child: GestureDetector(
            onTap: () {
              selectedMusicIndex != index
                  ? _threeDReelRecordingPageViewModel
                      .setCurrentMusic(musicList[index])
                  : _threeDReelRecordingPageViewModel.unsetCurrentMusic();
              setHasImportMusic(selectedMusicIndex != index, ref);
              if (selectedMusicIndex != index) {
                selectedMusicIndex = index;
                Fluttertoast.showToast(
                    msg: 'Selected music : ${musicList[index].songName}');
              } else {
                selectedMusicIndex = -1;
                Fluttertoast.showToast(
                    msg: 'Deselected music : ${musicList[index].songName}');
              }
            },
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  musicList[index].songName,
                  style: GoogleFonts.poppins(
                    textStyle: const TextStyle(
                      color: Colors.black,
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
                Text(
                  musicList[index].singerName,
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
        ),
        GestureDetector(
          onTap: () {
            currentPlayingIndex != index
                ? _threeDReelRecordingPageViewModel.playMusic(musicList[index])
                : _threeDReelRecordingPageViewModel.stopMusic();
            setState(() {
              currentPlayingIndex = (currentPlayingIndex == index) ? -1 : index;
            });
          },
          child: Container(
            width: 50.0,
            height: 50.0,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              border: (currentPlayingIndex == index)
                  ? Border.all(
                      color: ThemeColors.mainColor,
                      width: 2,
                    )
                  : null,
            ),
            child: (currentPlayingIndex == index)
                ? const Icon(
                    TPFiveIcon.pause,
                    color: ThemeColors.mainColor,
                    size: 20,
                  )
                : const Icon(
                    TPFiveIcon.circlePlay,
                    color: Color.fromRGBO(151, 151, 151, 1),
                    size: 20,
                  ),
          ),
        )
      ]),
    );
  }
}
