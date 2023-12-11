import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:get_it/get_it.dart';
import 'package:tpfive/services/space_service.dart';
import 'package:tpfive_game_server_api_client/api.dart';

class SpaceGroupNotifier extends StateNotifier<List<SpaceGroup>> {
  SpaceGroupNotifier(this.ref) : super([]);

  final Ref ref;
  final _spaceService = GetIt.I<SpaceService>();

  bool hasFetch = false;

  void _refresh(List<SpaceGroup> groups) {
    state = groups;
    hasFetch = true;
  }

  Future<void> fetchSpaceGroups() async {
    if (hasFetch) {
      return;
    }

    var res = await _spaceService.getAllSpaceGroups();
    _refresh(res);
  }
}

final StateNotifierProvider<SpaceGroupNotifier, List<SpaceGroup>>
    spaceGroupProvider =
    StateNotifierProvider<SpaceGroupNotifier, List<SpaceGroup>>((ref) {
  return SpaceGroupNotifier(ref);
});

class SpaceListNotifier extends StateNotifier<Map<String, List<Space>>> {
  SpaceListNotifier(this.ref) : super({});

  final Ref ref;
  final _spaceService = GetIt.I<SpaceService>();

  bool hasFetch = false;

  void _refresh(List<Space> spaces) {
    state = spaces.fold({}, (data, space) {
      if (!data.containsKey(space.spaceGroupId)) {
        data[space.spaceGroupId!] = [];
      }
      data[space.spaceGroupId]!.add(space);

      return data;
    });

    hasFetch = true;
  }

  Future<void> fetchSpaces() async {
    if (hasFetch) {
      return;
    }

    var res = await _spaceService.getAllSpaces();
    _refresh(res);
  }
}

final StateNotifierProvider<SpaceListNotifier, Map<String, List<Space>>>
    spaceListProvider =
    StateNotifierProvider<SpaceListNotifier, Map<String, List<Space>>>((ref) {
  return SpaceListNotifier(ref);
});
