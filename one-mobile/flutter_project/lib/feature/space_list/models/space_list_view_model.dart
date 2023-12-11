import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/feature/space_list/notifiers/space_notifier.dart';

class SpaceListViewModel {
  SpaceListViewModel(this._ref);

  final WidgetRef _ref;

  Future<void> fetchSpaceGroups() async {
    return _ref.read(spaceGroupProvider.notifier).fetchSpaceGroups();
  }

  Future<void> fetchSpaces() async {
    return _ref.read(spaceListProvider.notifier).fetchSpaces();
  }
}
