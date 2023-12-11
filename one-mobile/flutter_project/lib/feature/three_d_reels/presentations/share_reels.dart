import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/component/full_screen_loading.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/three_d_reels/models/set_reel_visibility_view_model.dart';
import 'package:tpfive/feature/three_d_reels/models/share_reels_view_model.dart';
import 'package:tpfive/feature/three_d_reels/models/switch_option.dart';
import 'package:tpfive/feature/three_d_reels/presentations/set_reel_visibility.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:video_player/video_player.dart';

class ShareReel extends ConsumerStatefulWidget {
  static const routeName = '/shareReel';
  static const tag = 'shareReel';
  final ReelFilePath filePaths;

  const ShareReel({super.key, required this.filePaths});

  @override
  ShareReelState createState() => ShareReelState();
}

class ShareReelState extends ConsumerState<ShareReel> {
  late ShareReelsViewModel _shareReelsViewModel;

  final _descriptionController = TextEditingController();
  late VideoPlayerController _controller;

  static const videoWidth = 168.0;
  static const videoHeight = 297.0;
  static const aspectRatio = videoWidth / videoHeight;

  @override
  void initState() {
    super.initState();
    _shareReelsViewModel = ShareReelsViewModel(ref);
    _controller = VideoPlayerController.file(File((widget.filePaths).video))
      ..initialize().then((_) => setState(() {}));
  }

  @override
  void dispose() {
    super.dispose();
    _controller.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final isLoading = ref.watch(loadingStateProvider).display;
    final descriptionLen = ref.watch(_shareReelsViewModel.description).length;

    return SafeArea(
        top: false,
        child: Stack(
          children: [
            Scaffold(
              backgroundColor: Colors.white,
              appBar: AppBar(
                toolbarHeight: 68,
                backgroundColor: Colors.white,
                centerTitle: true,
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
                    TPFiveIcon.close,
                    color: Colors.black,
                    size: 20,
                  ),
                ),
                title: Text(
                  'My Reel',
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
                  slivers: [
                    const SliverToBoxAdapter(
                      child: SizedBox(height: 15),
                    ),
                    SliverToBoxAdapter(
                      child: Center(
                        child: _controller.value.isInitialized
                            ? ClipRRect(
                                borderRadius: BorderRadius.circular(
                                    14.0), // Adjust the radius as needed
                                child: Stack(children: [
                                  SizedBox(
                                    width: videoWidth,
                                    height: videoHeight,
                                    child: AspectRatio(
                                      aspectRatio: aspectRatio,
                                      child: VideoPlayer(_controller),
                                    ),
                                  ),
                                  Positioned(
                                    right: 10,
                                    bottom: 8,
                                    child: ValueListenableBuilder(
                                      valueListenable: _controller,
                                      builder: (context, VideoPlayerValue value,
                                          child) {
                                        return Container(
                                            decoration: const BoxDecoration(
                                              color:
                                                  Color.fromRGBO(0, 0, 0, 0.5),
                                              borderRadius: BorderRadius.all(
                                                  Radius.circular(4)),
                                            ),
                                            padding: const EdgeInsets.symmetric(
                                                horizontal: 6),
                                            child: Text(
                                              _controller.value.isPlaying
                                                  ? formatDuration(_controller
                                                      .value.position)
                                                  : formatDuration(_controller
                                                      .value.duration),
                                              style: const TextStyle(
                                                fontSize: 14,
                                                fontWeight: FontWeight.w500,
                                                color: Colors.white,
                                              ),
                                            ));
                                      },
                                    ),
                                  ),
                                  Positioned(
                                    left: 10,
                                    bottom: 8,
                                    child: ValueListenableBuilder(
                                      valueListenable: _controller,
                                      builder: (context, VideoPlayerValue value,
                                          child) {
                                        return SizedBox(
                                            width: 20,
                                            height: 20,
                                            child: ElevatedButton(
                                              onPressed: () {
                                                setState(() {
                                                  if (_controller
                                                      .value.isPlaying) {
                                                    _controller.pause();
                                                  } else {
                                                    _controller.play();
                                                  }
                                                });
                                              },
                                              style: ElevatedButton.styleFrom(
                                                backgroundColor:
                                                    Colors.transparent,
                                                foregroundColor:
                                                    Colors.transparent,
                                                shadowColor: Colors.transparent,
                                                padding:
                                                    const EdgeInsets.all(0),
                                              ),
                                              child: Icon(
                                                _controller.value.isPlaying
                                                    ? TPFiveIcon.pause
                                                    : TPFiveIcon.play,
                                                color: const Color.fromRGBO(
                                                    217, 217, 217, 1),
                                                size: 20,
                                              ),
                                            ));
                                      },
                                    ),
                                  ),
                                ]))
                            : Container(),
                      ),
                    ),
                    const SliverToBoxAdapter(
                      child: SizedBox(height: 15),
                    ),
                    SliverToBoxAdapter(
                      child: Container(
                        constraints: BoxConstraints(
                          maxWidth: MediaQuery.of(context).size.width,
                        ),
                        padding: const EdgeInsets.symmetric(horizontal: 30),
                        child: TextField(
                          autofocus: false,
                          keyboardType: TextInputType.text,
                          controller: _descriptionController,
                          cursorColor: ThemeColors.mainColor,
                          decoration: InputDecoration(
                            hintText: 'Write Something',
                            hintStyle: GoogleFonts.poppins(
                              textStyle: const TextStyle(
                                color: Color.fromRGBO(136, 136, 136, 1),
                                fontSize: 14,
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                            border: InputBorder.none,
                            focusedBorder: const UnderlineInputBorder(
                              borderSide: BorderSide(
                                color: Colors.transparent,
                              ),
                            ),
                            counterText: '',
                          ),
                          style: const TextStyle(color: Colors.black),
                          maxLength: 200,
                          maxLines: null,
                          onChanged: (value) {
                            _shareReelsViewModel.setDescription(value);
                          },
                        ),
                      ),
                    ),
                    SliverToBoxAdapter(
                      child: Align(
                        alignment: Alignment.centerRight,
                        child: Padding(
                          padding: const EdgeInsets.only(right: 10),
                          child: Text(
                            '$descriptionLen / 200',
                            style: GoogleFonts.poppins(
                              textStyle: const TextStyle(
                                color: Color.fromRGBO(179, 179, 179, 1),
                                fontSize: 11,
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                    const SliverToBoxAdapter(
                      child: SizedBox(height: 15),
                    ),
                    const SliverToBoxAdapter(
                      child: Divider(),
                    ),
                    const SliverToBoxAdapter(
                      child: SizedBox(height: 15),
                    ),
                    SliverToBoxAdapter(
                      child: Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 20),
                        child: Row(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Icon(TPFiveIcon.feed,
                                color: Colors.black, size: 24),
                            const SizedBox(width: 10),
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    'Share to Reels',
                                    style: GoogleFonts.poppins(
                                      textStyle: const TextStyle(
                                        color: Colors.black,
                                        fontSize: 16,
                                        fontWeight: FontWeight.w500,
                                      ),
                                    ),
                                  ),
                                  Text(
                                    'Your Video will appear in Social Lobby and can be seen on the Reels tab of your profile.',
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
                            )
                          ],
                        ),
                      ),
                    ),
                    const SliverToBoxAdapter(
                      child: SizedBox(height: 15),
                    ),
                    SliverToBoxAdapter(
                      child: Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 20),
                        child: InkWell(
                          onTap: () {
                            Navigator.pushNamed(
                                context, SetReelVisibility.routeName);
                          },
                          child: Row(
                            children: [
                              const Icon(TPFiveIcon.join, color: Colors.black),
                              const SizedBox(width: 10),
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
                                      'All',
                                      style: GoogleFonts.poppins(
                                        textStyle: const TextStyle(
                                          color:
                                              Color.fromRGBO(128, 128, 128, 1),
                                          fontSize: 13,
                                          fontWeight: FontWeight.w500,
                                        ),
                                      ),
                                    )
                                  ],
                                ),
                              ),
                              Row(
                                children: [
                                  Text(ref.read(joinModeEnable) ? 'On' : 'Off'),
                                  const SizedBox(width: 5),
                                  const Icon(TPFiveIcon.arrowRight,
                                      color: Colors.black),
                                ],
                              )
                            ],
                          ),
                        ),
                      ),
                    ),
                    const SliverToBoxAdapter(
                      child: SizedBox(height: 15),
                    ),
                    SliverToBoxAdapter(
                      child: Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 20),
                        child: GestureDetector(
                          onTap: () {
                            _shareReelsViewModel
                                .sendToSocialLobbyMessageToUnity(
                                    SwitchOption(mode: true));
                          },
                          child: Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              const Icon(TPFiveIcon.delete,
                                  color: Color.fromRGBO(255, 58, 58, 1),
                                  size: 24),
                              const SizedBox(width: 10),
                              Text(
                                'Discard',
                                style: GoogleFonts.poppins(
                                  textStyle: const TextStyle(
                                    color: Color.fromRGBO(255, 58, 58, 1),
                                    fontSize: 16,
                                    fontWeight: FontWeight.w500,
                                  ),
                                ),
                              )
                            ],
                          ),
                        ),
                      ),
                    ),
                    const SliverToBoxAdapter(
                      child: SizedBox(height: 15),
                    ),
                    const SliverToBoxAdapter(
                      child: Divider(),
                    ),
                    SliverToBoxAdapter(
                        child: Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const SizedBox(width: 20),
                        Expanded(
                          child: ElevatedButton(
                            onPressed: () {
                              _shareReelsViewModel
                                  .sendToSocialLobbyMessageToUnity(
                                      SwitchOption(mode: true));
                            },
                            style: ElevatedButton.styleFrom(
                              backgroundColor:
                                  const Color.fromRGBO(238, 238, 238, 1),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(99.0),
                              ),
                              shadowColor: Colors.transparent,
                              padding: const EdgeInsets.symmetric(
                                  horizontal: 30, vertical: 12),
                            ),
                            child: Text(
                              'Draft',
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
                        const SizedBox(width: 10),
                        Expanded(
                          child: ElevatedButton(
                            onPressed: () {
                              _shareReelsViewModel.sendUploadReel(
                                  SwitchOption(mode: true), widget.filePaths);
                            },
                            style: ElevatedButton.styleFrom(
                              backgroundColor: ThemeColors.mainColor,
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(99.0),
                              ),
                              shadowColor: Colors.transparent,
                              padding: const EdgeInsets.symmetric(
                                  horizontal: 30, vertical: 12),
                            ),
                            child: Text(
                              'Upload',
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
                        const SizedBox(width: 20),
                      ],
                    )),
                  ],
                ),
              ),
            ),
            if (isLoading)
              const SizedBox(
                  width: double.infinity,
                  height: double.infinity,
                  child: FullScreenLoading())
          ],
        ));
  }

  String formatDuration(Duration duration) {
    String twoDigits(int n) {
      if (n >= 10) {
        return '$n';
      }
      return '0$n';
    }

    String minutes = twoDigits(duration.inMinutes.remainder(60));
    String seconds = twoDigits(duration.inSeconds.remainder(60));
    return '$minutes:$seconds';
  }
}
