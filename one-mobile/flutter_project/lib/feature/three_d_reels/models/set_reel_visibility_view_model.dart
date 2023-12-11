import 'package:flutter_riverpod/flutter_riverpod.dart';

final joinModeEnable = StateProvider<bool>((ref) => true);

class SetReelVisibilityViewModel {
  final WidgetRef _ref;

  SetReelVisibilityViewModel(WidgetRef ref) : _ref = ref;

  void setJoinModeEnable(bool flag) {
    _ref.read(joinModeEnable.notifier).state = flag;
  }
}
