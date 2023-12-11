import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:tpfive/feature/avatar_edit/avatar_edit.dart';
import 'package:tpfive/feature/prefs/presentations/prefs_widget.dart';
import 'package:tpfive/feature/space_list/presentations/space_list.dart';
import 'package:tpfive/feature/three_d_reels/presentations/co_create_page.dart';
import 'package:tpfive/feature/three_d_reels/presentations/create_three_d_reels.dart';
import 'package:tpfive/feature/unity_holder/unity_message_widget.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive_game_server_api_client/api.dart';

class SocialLobby extends UnityMessageWidget {
  static const routeName = '/socialLobby';
  static const tag = 'SocialLobby';

  const SocialLobby({super.key, required super.messageKey});

  @override
  SocialLobbyState createState() => SocialLobbyState();
}

class SocialLobbyState extends UnityMessageWidgetState<SocialLobby> {
  int selectedIndex = 0;

  @override
  void initSubscribe() {
    subscribe(UnityMessageType.TO_AVATAR_EDIT, toAvatarEdit);
    subscribe(UnityMessageType.SWITCHED_TO_COCREATE_PAGE, toCoCreate);
    subscribe(UnityMessageType.SHOW_LOADING, showLoading);
    subscribe(UnityMessageType.HIDE_LOADING, hideLoading);
  }

  void toAvatarEdit(message) {
    Navigator.pushNamed(context, AvatarEdit.routeName, arguments: {
      'avatarArguments': AvatarEditArguments(),
    });
  }

  void toCoCreate(message) {
    hideLoading(message);
    Reel? reelDataFromUnity = Reel.fromJson(json.decode(message.data));

    Navigator.pushNamed(context, CoCreatePage.routeName, arguments: {
      'reelData': reelDataFromUnity,
    });
  }

  void showLoading(message) {
    Map<String, dynamic> data = jsonDecode(message.data);
    ref.read(loadingStateProvider.notifier).show(alpha: data['alpha']);
  }

  void hideLoading(message) {
    ref.read(loadingStateProvider.notifier).hide();
  }

  @override
  Widget buildChildBody(BuildContext context) {
    return SafeArea(
      child: Column(mainAxisAlignment: MainAxisAlignment.end, children: [
        Row(children: [
          const SizedBox(width: 15),
          Container(
              alignment: Alignment.centerLeft,
              width: 213,
              height: 56,
              decoration: const BoxDecoration(
                image: DecorationImage(
                    image: AssetImage('assets/image/social_lobby_feat_bg.png'),
                    fit: BoxFit.cover),
              ),
              child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    IconButton(
                      onPressed: () {
                        select(0);
                      },
                      icon: (selectedIndex == 0)
                          ? Image.asset(
                              'assets/image/social_lobby_home_enable.png',
                              scale: 0.75)
                          : Image.asset(
                              'assets/image/social_lobby_home_default.png'),
                      splashColor: Colors.transparent,
                      highlightColor: Colors.transparent,
                    ),
                    IconButton(
                      onPressed: () {
                        select(1);
                      },
                      icon: (selectedIndex == 1)
                          ? Image.asset(
                              'assets/image/social_lobby_feed_enable.png',
                              scale: 0.75)
                          : Image.asset(
                              'assets/image/social_lobby_feed_default.png'),
                      splashColor: Colors.transparent,
                      highlightColor: Colors.transparent,
                    ),
                    IconButton(
                      onPressed: () {
                        select(2);
                        Navigator.pushNamed(context, SpaceList.routeName);
                      },
                      icon: (selectedIndex == 2)
                          ? Image.asset(
                              'assets/image/social_lobby_world_enable.png',
                              scale: 0.75)
                          : Image.asset(
                              'assets/image/social_lobby_world_default.png'),
                      splashColor: Colors.transparent,
                      highlightColor: Colors.transparent,
                    ),
                    IconButton(
                      onPressed: () {
                        select(3);
                        Navigator.pushNamed(context, PrefsWidget.routeName);
                      },
                      icon: (selectedIndex == 3)
                          ? Image.asset(
                              'assets/image/social_lobby_social_enable.png',
                              scale: 0.75)
                          : Image.asset(
                              'assets/image/social_lobby_social_default.png'),
                      splashColor: Colors.transparent,
                      highlightColor: Colors.transparent,
                    ),
                  ])),
          const Expanded(
            child: SizedBox(),
          ),
          IconButton(
            onPressed: () {
              Navigator.pushNamed(context, CreateThreeDReal.routeName);
            },
            icon: Image.asset('assets/image/social_lobby_to_real.png'),
            iconSize: 48,
            splashColor: Colors.transparent,
            highlightColor: Colors.transparent,
          ),
          const SizedBox(width: 15),
        ]),
        const SizedBox(height: 20)
      ]),
    );
  }

  void select(index) {
    if (selectedIndex == index) {
      return;
    }

    setState(() {
      selectedIndex = index;
    });
  }
}
