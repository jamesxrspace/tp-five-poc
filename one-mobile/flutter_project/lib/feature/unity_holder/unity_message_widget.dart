import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_unity_widget/flutter_unity_widget.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:get_it/get_it.dart';
import 'package:tpfive/app.dart';
import 'package:tpfive/component/full_screen_loading.dart';
import 'package:tpfive/feature/unity_holder/unity_message_mixin.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive/services/prefs_service.dart';
import 'package:tpfive/services/unity_message_service.dart';
import 'package:tpfive/utils/logger.dart';

abstract class UnityMessageWidget extends ConsumerStatefulWidget {
  const UnityMessageWidget({super.key, required this.messageKey});

  static const tag = 'UnityMessageWidget';
  final String messageKey;
}

abstract class UnityMessageWidgetState<T extends UnityMessageWidget>
    extends ConsumerState<T> with UnityMessageMixin, RouteAware {
  final UnityMessageService unityMessageService =
      GetIt.I<UnityMessageService>();
  final PrefsService prefsService = GetIt.I<PrefsService>();

  late UnityWidgetController _controller;

  @override
  void initState() {
    super.initState();
    initSubscribe();
    initUnityMessageProvider(ref);

    _subscribeUnityBasicEvent();
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    routeObserver.subscribe(this, ModalRoute.of(context) as Route);
  }

  @override
  void didPopNext() {
    super.didPopNext();

    ref
        .read(unityPostMessageProvider.notifier)
        .setWidgetAndController(ref, _controller);
  }

  @mustBeOverridden
  void initSubscribe() {}

  @mustBeOverridden
  Widget buildChildBody(BuildContext context) {
    return Container();
  }

  @override
  void dispose() {
    super.dispose();
    _unsubscribeAll();
    routeObserver.unsubscribe(this);
    unityMessageService.removeUnityHolder(widget.messageKey);
  }

  void onUnityWidgetCreated() {}

  void _subscribeUnityBasicEvent() {
    subscribe(UnityMessageType.GENERAL_STATUS, (message) {
      var prefs = prefsService.prefsData.prefs['SetUnityGeneralMsg'];
      if (prefs?.value) {
        ref.read(unityGeneralMessageProvider.notifier).state = message.data;
      }
    });

    subscribe(UnityMessageType.SHOW_TOAST, (message) {
      Fluttertoast.showToast(msg: message.data, toastLength: Toast.LENGTH_LONG);
    });
  }

  // Android nature back botton event.Temp for disable
  Future<bool> onWillPop() async {
    return false;
  }

  @override
  Widget build(BuildContext context) {
    return WillPopScope(
      onWillPop: onWillPop,
      child: Scaffold(
        body: Stack(
          children: [
            UnityWidget(
              onUnityCreated: (controller) {
                _controller = controller;

                ref
                    .read(unityPostMessageProvider.notifier)
                    .setWidgetAndController(ref, _controller);

                unityMessageService.addUnityHolder(widget.messageKey);

                onUnityWidgetCreated();
              },
              onUnityMessage: onUnityMessage,
              useAndroidViewSurface: false,
              hideStatus: true,
              fullscreen: true,
            ),
            buildChildBody(context),
            Positioned.fill(child: getLoading()),
          ],
        ),
      ),
    );
  }

  Widget getLoading() {
    var displayLoading = ref.watch(loadingStateProvider).display;
    if (displayLoading) {
      return const FullScreenLoading();
    }

    return Container();
  }

  void onUnityMessage(message) {
    if (!unityMessageService.isTopmostUnityHolder(widget.messageKey)) {
      return;
    }

    Log.d(UnityMessageWidget.tag,
        'Received message from unity: ${message.toString()}');

    var receiveUnityMessage = UnityMessage.fromJson(jsonDecode(message));
    if (receiveUnityMessage.errorCode != ErrorCode.SUCCESS) {
      var prefs = prefsService.prefsData.prefs['ShowErrorMsgOnToast'];
      if (prefs?.value) {
        Fluttertoast.showToast(
            msg:
                'ErrorCode:${receiveUnityMessage.errorCode}, ErrMsg:${receiveUnityMessage.errorMsg}',
            toastLength: Toast.LENGTH_LONG);
      }
    }

    var promise = ref.read(unityPromiseProvider);
    if (promise.containsKey(receiveUnityMessage.sessionId)) {
      promise[receiveUnityMessage.sessionId]!.complete(receiveUnityMessage);
    } else {
      GetIt.I<UnityMessageService>()
          .callback(widget.messageKey, receiveUnityMessage);
    }
  }

  void subscribe(UnityMessageType type, UnityMessageCallback callback) {
    unityMessageService.subscribe(widget.messageKey, type, callback);
  }

  void _unsubscribeAll() {
    unityMessageService.unsubscribeAll(widget.messageKey);
  }
}
