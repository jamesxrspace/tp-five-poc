import 'package:cached_network_image/cached_network_image.dart';
import 'package:expandable/expandable.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/room/room.dart';
import 'package:tpfive/feature/space_list/models/space_list_view_model.dart';
import 'package:tpfive/feature/space_list/notifiers/space_notifier.dart';
import 'package:tpfive/feature/unity_holder/unity_message_widget.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';

class SpaceList extends UnityMessageWidget {
  static const routeName = '/spaceList';
  static const tag = 'SpaceList';

  const SpaceList({super.key, required super.messageKey});

  @override
  SpaceListState createState() => SpaceListState();
}

class SpaceListState extends UnityMessageWidgetState<SpaceList> {
  final RefreshController _refreshController =
      RefreshController(initialRefresh: false);
  final ScrollController _listViewController = ScrollController();
  late SpaceListViewModel _spaceListViewModel;

  @override
  void initState() {
    super.initState();

    _spaceListViewModel = SpaceListViewModel(ref);
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      ref.read(loadingStateProvider.notifier).show();
      _onRefresh();
      await _spaceListViewModel.fetchSpaces();
      ref.read(loadingStateProvider.notifier).hide();
    });
  }

  @override
  void initSubscribe() {
    subscribe(UnityMessageType.SWITCHED_TO_SPACE, switchedToSpace);
  }

  @override
  void dispose() {
    super.dispose();
    _refreshController.dispose();
  }

  void switchedToSpace(message) {
    ref.read(loadingStateProvider.notifier).hide();
    Navigator.pushNamed(context, Room.routeName);
  }

  void _onRefresh() async {
    await _spaceListViewModel.fetchSpaceGroups();
    _refreshController.refreshCompleted();
  }

  @override
  Widget buildChildBody(BuildContext context) {
    final spaceGroups = ref.watch(spaceGroupProvider);
    return Scaffold(
        backgroundColor: Colors.white,
        body: SafeArea(
            child: Column(children: [
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
                    'Space List',
                    style: GoogleFonts.poppins(
                      textStyle: const TextStyle(
                        fontSize: 23,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 64)
            ],
          ),
          const SizedBox(height: 15),
          Expanded(
              child: SmartRefresher(
                  header: const MaterialClassicHeader(),
                  controller: _refreshController,
                  onRefresh: _onRefresh,
                  child: ListView.builder(
                    controller: _listViewController,
                    itemCount: spaceGroups.length,
                    itemBuilder: (context, index) {
                      return Card(
                        margin: const EdgeInsets.all(8.0),
                        child: ExpandableTheme(
                          data: const ExpandableThemeData(
                            hasIcon: false,
                            tapHeaderToExpand: true,
                          ),
                          child: ExpandablePanel(
                              header: Column(
                                children: [
                                  getImage(spaceGroups[index].thumbnail!),
                                  Text(
                                    spaceGroups[index].name!,
                                    textAlign: TextAlign.left,
                                  ),
                                ],
                              ),
                              collapsed: Container(),
                              expanded: Wrap(
                                spacing: 10,
                                runSpacing: 10,
                                children: getSpaceWidgets(
                                    spaceGroups[index].spaceGroupId!),
                              )),
                        ),
                      );
                    },
                  )))
        ])));
  }

  List<Widget> getSpaceWidgets(String groupId) {
    final spaceLists = ref.watch(spaceListProvider);

    if (spaceLists[groupId]!.isEmpty) {
      return [];
    }

    return spaceLists[groupId]!.map((e) {
      return GestureDetector(
        onTap: () {
          postMessageToUnity(
              FlutterMessageType.REQUEST_TO_SPACE,
              roomConfigToJson(
                  RoomConfig(spaceId: e.spaceId!, sceneKey: e.addressable!)));

          ref.read(loadingStateProvider.notifier).show();
        },
        child: SizedBox(
          width: 180,
          child: Card(
            child: Column(children: [
              getImage(e.thumbnail!),
              Text(e.name!),
            ]),
          ),
        ),
      );
    }).toList();
  }

  Widget getImage(String url) {
    return CachedNetworkImage(
      imageUrl: url,
      placeholder: (context, url) => const CircularProgressIndicator(),
      errorWidget: (context, url, error) => const Icon(Icons.error),
    );
  }
}
