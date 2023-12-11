import 'package:flutter/material.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/unity_holder/unity_message_widget.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';

class Room extends UnityMessageWidget {
  const Room({super.key, required super.messageKey});

  static const routeName = '/room';
  static const tag = 'Room';

  @override
  RoomState createState() => RoomState();
}

class RoomState extends UnityMessageWidgetState<Room> {
  @override
  void initSubscribe() {
    subscribe(UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE, toPreviousPage);
  }

  void toPreviousPage(message) {
    ref.read(loadingStateProvider.notifier).hide();
    Navigator.pop(context);
  }

  @override
  Widget buildChildBody(BuildContext context) {
    return SafeArea(
        child: Column(
      children: [
        const SizedBox(height: 20),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            ElevatedButton(
              onPressed: () {
                postMessageToUnity(
                    FlutterMessageType.REQUEST_TO_SOCIAL_LOBBY_PAGE, '');

                ref.read(loadingStateProvider.notifier).show(alpha: 1.0);
              },
              style: ElevatedButton.styleFrom(
                shape: const CircleBorder(),
                padding: const EdgeInsets.all(5),
                backgroundColor: Colors.transparent,
                foregroundColor: Colors.transparent,
                shadowColor: Colors.transparent,
              ),
              child: const Icon(
                TPFiveIcon.arrowLeft,
                color: Colors.black,
                size: 20,
              ),
            ),
          ],
        )
      ],
    ));
  }
}
