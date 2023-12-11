import 'package:flutter/material.dart';
import 'package:tpfive/feature/avatar_edit/avatar_edit.dart';
import 'package:tpfive/feature/launch_unity/launch_unity.dart';
import 'package:tpfive/feature/room/room.dart';
import 'package:tpfive/feature/prefs/presentations/prefs_widget.dart';
import 'package:tpfive/feature/social_lobby/presentations/social_lobby.dart';
import 'package:tpfive/feature/space_list/presentations/space_list.dart';
import 'package:tpfive/feature/startup/startup.dart';
import 'package:tpfive/feature/three_d_reels/presentations/co_create_page.dart';
import 'package:tpfive/feature/three_d_reels/presentations/create_three_d_reels.dart';
import 'package:tpfive/feature/three_d_reels/presentations/set_reel_visibility.dart';
import 'package:tpfive/feature/three_d_reels/presentations/share_reels.dart';
import 'package:tpfive/feature/three_d_reels/presentations/three_d_reel_recording_page.dart';

final appRoutes = {
  '/': (context) => const StartupScreen(),
  StartupScreen.routeName: (context) => const StartupScreen(),
  LaunchUnity.routeName: (context) =>
      const LaunchUnity(messageKey: LaunchUnity.routeName),
  AvatarEdit.routeName: (context, Map<String, dynamic> argument) => AvatarEdit(
      messageKey: AvatarEdit.routeName,
      avatarArguments: argument['avatarArguments']),
  SocialLobby.routeName: (context) =>
      const SocialLobby(messageKey: SocialLobby.routeName),
  CreateThreeDReal.routeName: (context) =>
      const CreateThreeDReal(messageKey: CreateThreeDReal.routeName),
  ThreeDReelRecordingPage.routeName: (context) => const ThreeDReelRecordingPage(
      messageKey: ThreeDReelRecordingPage.routeName),
  SpaceList.routeName: (context) =>
      const SpaceList(messageKey: SpaceList.routeName),
  Room.routeName: (context) => const Room(messageKey: Room.routeName),
  CoCreatePage.routeName: (context, Map<String, dynamic> argument) =>
      CoCreatePage(
          messageKey: CoCreatePage.routeName, reelData: argument['reelData']),
  ShareReel.routeName: (context, Map<String, dynamic> argument) => ShareReel(
        filePaths: argument['filePaths'],
      ),
  SetReelVisibility.routeName: (context) => const SetReelVisibility(),
  PrefsWidget.routeName: (context) => const PrefsWidget(),
};

MaterialPageRoute routeTo(RouteSettings settings) {
  if (settings.arguments != null) {
    return MaterialPageRoute(
        settings: settings,
        builder: (ctx) {
          return appRoutes[settings.name]!(ctx, settings.arguments);
        });
  } else {
    return MaterialPageRoute(
        settings: settings, builder: (ctx) => appRoutes[settings.name]!(ctx));
  }
}
