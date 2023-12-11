import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_translate/flutter_translate.dart';
import 'package:get_it/get_it.dart';
import 'package:tpfive/services/account_service.dart';
import 'package:tpfive/services/gameserver_service.dart';
import 'package:tpfive/services/mock_reel_service.dart';
import 'package:tpfive/services/prefs_service.dart';
import 'package:tpfive/services/space_service.dart';
import 'package:tpfive/services/unity_message_service.dart';
import 'package:tpfive/utils/api.dart';

import 'app.dart';

final ProviderContainer globalProviderContainer = ProviderContainer();
final GlobalKey<NavigatorState> rootAppNavigatorKey =
    GlobalKey<NavigatorState>();

void _registerServices() {
  GetIt.I.registerLazySingleton<AccountService>(() {
    return AccountService(
        dotenv.env['AUTHING_DOMAIN'] ?? '',
        dotenv.env['AUTHING_CLIENT_ID'] ?? '',
        dotenv.env['USER_POOL_ID'] ?? '',
        dotenv.env['SERVER_DOMAIN'] ?? '',
        dotenv.env['AUTHING_REDIRECT_DOMAIN'] ?? '');
  });

  GetIt.I.registerLazySingleton<GameServerService>(() {
    return GameServerService();
  });

  GetIt.I.registerLazySingleton<UnityMessageService>(() {
    return UnityMessageService();
  });

  GetIt.I.registerLazySingleton<SpaceService>(() {
    return SpaceService();
  });

  GetIt.I.registerLazySingleton<MockReelService>(() {
    return MockReelService();
  });

  GetIt.I.registerSingletonAsync<PrefsService>(() async {
    var service = PrefsService();
    await service.init();
    return service;
  });
}

FutureOr<void> main() async {
  await dotenv.load(fileName: '.env');

  Api.initApi();
  _registerServices();

  var localizedApp = await _createLocalizedApp();

  await SystemChrome.setPreferredOrientations([DeviceOrientation.portraitUp]);
  runApp(UncontrolledProviderScope(
      container: globalProviderContainer, child: localizedApp));
}

Future<Widget> _createLocalizedApp() async {
  var delegate = await LocalizationDelegate.create(
    fallbackLocale: 'en_US',
    supportedLocales: ['en_US', 'zh_TW'],
  );

  return LocalizedApp(
    delegate,
    const DefaultTextHeightBehavior(
      textHeightBehavior: TextHeightBehavior(),
      child: MyApp(),
    ),
  );
}
