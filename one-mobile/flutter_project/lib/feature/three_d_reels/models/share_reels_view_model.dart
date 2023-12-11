import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/feature/three_d_reels/models/switch_option.dart';
import 'package:tpfive/feature/unity_holder/unity_message_mixin.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive_game_server_api_client/api.dart';

class ShareReelsViewModel with UnityMessageMixin {
  final WidgetRef _ref;
  final description = StateProvider<String>((ref) => '');
  static const timeoutSec = 100;

  ShareReelsViewModel(WidgetRef ref) : _ref = ref {
    initUnityMessageProvider(_ref);
  }

  void setDescription(String description) {
    _ref.read(this.description.notifier).state = description;
  }

  void sendToSocialLobbyMessageToUnity(SwitchOption option) {
    postMessageToUnity(FlutterMessageType.REQUEST_TO_SOCIAL_LOBBY_PAGE,
        json.encode(option.toJson()));
  }

  void sendUploadReel(
      SwitchOption publishOption, ReelFilePath reelFilePath) async {
    _ref.read(loadingStateProvider.notifier).show();
    var reelData = CreateReelRequest(
        thumbnail: reelFilePath.thumbnail,
        video: reelFilePath.video,
        xrs: reelFilePath.xrs,
        joinMode: JoinModeEnum.all,
        description: _ref.read(description));
    var result = await postMessageToUnityWait(
        FlutterMessageType.UPLOAD_REEL, json.encode(reelData.toJson()),
        timeout: timeoutSec);
    if (result.isSuccess()) {
      sendToSocialLobbyMessageToUnity(publishOption);
    }
    _ref.read(loadingStateProvider.notifier).hide();
  }
}
