import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/feature/three_d_reels/models/three_d_reel_recording_page_view_model.dart';
import 'package:tpfive/feature/unity_holder/unity_message_mixin.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';

class CoCreatePageViewModel with UnityMessageMixin {
  final WidgetRef _ref;

  final coCreateMode = StateProvider<CoCreateMode>((ref) => CoCreateMode.VIEW);
  final descriptionExpanded = StateProvider<bool>((ref) => false);
  final isLike = StateProvider<bool>((ref) => false);

  CoCreatePageViewModel(WidgetRef ref) : _ref = ref {
    initUnityMessageProvider(_ref);
  }

  void setDescriptionExpanded(bool flag) {
    _ref.read(descriptionExpanded.notifier).state = flag;
  }

  void setIsLike(bool flag) {
    _ref.read(isLike.notifier).state = flag;
  }

  void sendToSocialLobbyMessageToUnity() {
    postMessageToUnity(FlutterMessageType.REQUEST_TO_SOCIAL_LOBBY_PAGE, '');
  }
}
