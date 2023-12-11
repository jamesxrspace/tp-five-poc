import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:tpfive/feature/avatar_edit/avatar_edit.dart';
import 'package:tpfive/feature/social_lobby/presentations/social_lobby.dart';
import 'package:tpfive/feature/unity_holder/unity_message_widget.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive/services/account_service.dart';
import 'package:tpfive/services/prefs_service.dart';

class LaunchUnity extends UnityMessageWidget {
  static const routeName = '/launchUnity';
  static const tag = 'LaunchUnity';

  const LaunchUnity({super.key, required super.messageKey});

  @override
  LaunchUnityState createState() => LaunchUnityState();
}

class LaunchUnityState extends UnityMessageWidgetState<LaunchUnity> {
  final PrefsService _prefsService = GetIt.I<PrefsService>();

  @override
  void initState() {
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) async {
      ref.read(loadingStateProvider.notifier).show(
          alpha: 1, comment: 'Wait for loading', position: Alignment.center);
    });
  }

  @override
  void initSubscribe() {
    subscribe(UnityMessageType.TO_AVATAR_EDIT, toAvatarEdit);
    subscribe(UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE, toSocialLobby);
    subscribe(UnityMessageType.REQUEST_ACCESS_TOKEN, onUnityLaunched);
  }

  void onUnityLaunched(message) {
    postMessageToUnity(
        FlutterMessageType.PREFS, prefsDataToJson(_prefsService.prefsData));

    postMessageToUnityAck(
      GetIt.I<AccountService>().getAccessToken(),
      (message as UnityMessage).sessionId,
      ErrorCode.SUCCESS,
      '',
    );
  }

  void toAvatarEdit(message) {
    ref.read(loadingStateProvider.notifier).hide();
    Navigator.pushNamed(context, AvatarEdit.routeName, arguments: {
      'avatarArguments': AvatarEditArguments(fromRoute: LaunchUnity.routeName),
    });
  }

  void toSocialLobby(message) {
    ref.read(loadingStateProvider.notifier).hide();
    Navigator.pushNamed(context, SocialLobby.routeName);
  }

  @override
  Widget buildChildBody(BuildContext context) {
    return Container();
  }
}
