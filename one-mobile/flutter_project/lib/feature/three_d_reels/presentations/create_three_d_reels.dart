import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/three_d_reels/models/create_three_d_reels_view_model.dart';
import 'package:tpfive/feature/three_d_reels/models/reel_scene_model.dart';
import 'package:tpfive/feature/three_d_reels/presentations/reels_card.dart';
import 'package:tpfive/feature/three_d_reels/presentations/three_d_reel_recording_page.dart';
import 'package:tpfive/feature/unity_holder/unity_message_widget.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive/providers/record_state_provider.dart';
import 'package:tpfive/services/mock_reel_service.dart';
import 'package:tpfive/utils/logger.dart';

class CreateThreeDReal extends UnityMessageWidget {
  static const routeName = '/createThreeDReal';
  static const tag = 'CreateThreeDReal';

  const CreateThreeDReal({super.key, required super.messageKey});

  @override
  CreateThreeDRealState createState() => CreateThreeDRealState();
}

class CreateThreeDRealState extends UnityMessageWidgetState<CreateThreeDReal> {
  late CreateThreeDRealsViewModel _createThreeDRealsViewModel;

  final MockReelService mockService = GetIt.I<MockReelService>();
  List<ReelSceneModel> sceneList = [];

  @override
  void initState() {
    super.initState();

    _createThreeDRealsViewModel = CreateThreeDRealsViewModel(ref);
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      ref.watch(recordStateType);
    });

    mockService.onRequest(RequestOptions(path: 'mock_scene_list')).then(
        (value) {
      setState(() {
        sceneList = (value.data as List<dynamic>)
            .map((e) => ReelSceneModel.fromJson(e))
            .toList();
      });
    }, onError: (error) => Log.d(CreateThreeDReal.tag, error.toString()));
  }

  @override
  void initSubscribe() {
    subscribe(UnityMessageType.SWITCHED_TO_REEL_PAGE, toReelPage);
  }

  @override
  Widget buildChildBody(BuildContext context) {
    final selectedSceneIdx =
        ref.watch(_createThreeDRealsViewModel.selectedScene);

    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          SafeArea(
            child: Column(
              children: [
                const SizedBox(height: 20),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    ElevatedButton(
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
                    Expanded(
                      child: Center(
                        child: Text(
                          'Create 3D Reel',
                          style: GoogleFonts.poppins(
                            textStyle: const TextStyle(
                              fontSize: 23,
                              fontWeight: FontWeight.w800,
                            ),
                          ),
                        ),
                      ),
                    ),
                    ElevatedButton(
                      onPressed: (selectedSceneIdx == -1)
                          ? null
                          : () {
                              ref.read(loadingStateProvider.notifier).show();

                              postMessageToUnity(
                                FlutterMessageType.REQUEST_TO_REEL_PAGE,
                                reelConfigToJson(ReelConfig(
                                    bundleId:
                                        sceneList[selectedSceneIdx].bundleId!,
                                    entry: ReelEntryType.CREATE,
                                    sceneName: sceneList[selectedSceneIdx]
                                        .sceneName!)),
                              );
                            },
                      style: ElevatedButton.styleFrom(
                        shape: const CircleBorder(),
                        padding: const EdgeInsets.all(5),
                        backgroundColor: selectedSceneIdx != -1
                            ? ThemeColors.mainColor
                            : Colors.transparent,
                        foregroundColor: Colors.transparent,
                        shadowColor: Colors.transparent,
                      ),
                      child: const Icon(
                        TPFiveIcon.tick,
                        color: Colors.black,
                        size: 20,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 15),
                Text(
                  'Pick up a space for your 3D reel',
                  style: GoogleFonts.poppins(
                    textStyle: const TextStyle(
                      color: Color.fromRGBO(128, 128, 128, 1),
                      fontSize: 15,
                      fontWeight: FontWeight.w400,
                    ),
                  ),
                ),
                const SizedBox(height: 5),
                Expanded(
                  child: _buildSelectSceneField(),
                )
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSelectSceneField() {
    final selectedSceneIdx =
        ref.watch(_createThreeDRealsViewModel.selectedScene);
    return ListView.builder(
      padding: const EdgeInsets.only(bottom: 60),
      physics: const AlwaysScrollableScrollPhysics(),
      itemCount: sceneList.length,
      itemBuilder: (context, index) {
        return Padding(
          padding: const EdgeInsets.symmetric(horizontal: 30),
          child: ReelsCard(
            reelSceneModel: sceneList[index],
            viewModel: _createThreeDRealsViewModel,
            index: index,
            isSelected: selectedSceneIdx == index,
          ),
        );
      },
    );
  }

  void toReelPage(message) {
    Log.d(ThreeDReelRecordingPage.tag,
        'Received message from unity: ${message.toString()}');
    setRecordType(RecordStateType.PRESET);
    ref.read(loadingStateProvider.notifier).hide();
    Navigator.pushNamed(context, ThreeDReelRecordingPage.routeName);
  }
}
