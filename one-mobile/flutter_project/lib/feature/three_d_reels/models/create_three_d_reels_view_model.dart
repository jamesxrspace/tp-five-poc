import 'package:flutter_riverpod/flutter_riverpod.dart';

class CreateThreeDRealsViewModel {
  final WidgetRef _ref;

  final selectedScene = StateProvider<int>((ref) => -1);

  CreateThreeDRealsViewModel(WidgetRef ref) : _ref = ref;

  void setScene(int sceneId) {
    _ref.read(selectedScene.notifier).state = sceneId;
  }
}
