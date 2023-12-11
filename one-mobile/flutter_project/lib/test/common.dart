import 'package:get_it/get_it.dart';
import 'package:tpfive/services/prefs_service.dart';
import 'package:tpfive/services/unity_message_service.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter/material.dart';
import 'package:tpfive/app_routes.dart';

Widget createTestWidget(Widget widget, UnityMessageService messageService) {
  GetIt.I.registerLazySingleton<UnityMessageService>(() {
    return messageService;
  });
  GetIt.I.registerSingletonAsync<PrefsService>(() async {
    var service = PrefsService();
    return service;
  });

  return ProviderScope(
      child: MaterialApp(
    home: widget,
    onGenerateRoute: (RouteSettings settings) {
      return routeTo(settings);
    },
  ));
}
