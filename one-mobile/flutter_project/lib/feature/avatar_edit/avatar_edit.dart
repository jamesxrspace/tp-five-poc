import 'package:flutter/material.dart';
import 'package:tpfive/feature/launch_unity/launch_unity.dart';
import 'package:tpfive/feature/social_lobby/presentations/social_lobby.dart';
import 'package:tpfive/feature/unity_holder/unity_message_widget.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';

class AvatarEditArguments {
  final String fromRoute;

  AvatarEditArguments({this.fromRoute = ''});
}

class AvatarEdit extends UnityMessageWidget {
  static const routeName = '/avatarEdit';
  static const tag = 'AvatarEdit';

  final AvatarEditArguments avatarArguments;

  const AvatarEdit(
      {super.key, required super.messageKey, required this.avatarArguments});

  @override
  AvatarEditState createState() => AvatarEditState();
}

class AvatarEditState extends UnityMessageWidgetState<AvatarEdit> {
  @override
  void initSubscribe() {
    subscribe(UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE, toSocialLobby);
  }

  void toSocialLobby(message) {
    /*
     * There two cases to open AvatarEdit
     * 1. Login -> LaunchUnity -> AvatarEdit -> SocialLobby (only happens in login without avatar data)
     * 2. SocialLobby -> AvatarEdit (usually situation in game)
     * In case 1
     *  We have to use popAndPushNamed route to SocialLobby.If using pushNamed,
     *  there will have two AvatarEdit in stack when case 2 happens.
     */
    if (widget.avatarArguments.fromRoute == LaunchUnity.routeName) {
      Navigator.popAndPushNamed(context, SocialLobby.routeName);
    } else {
      Navigator.pop(context);
    }
  }

  @override
  Widget buildChildBody(BuildContext context) {
    return Container();
  }
}
