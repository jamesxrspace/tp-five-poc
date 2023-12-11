import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:flutter_translate/flutter_translate.dart';
import 'package:tpfive/app_routes.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/main.dart';

final RouteObserver<Route<dynamic>> routeObserver = RouteObserver();

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    var localizationDelegate = LocalizedApp.of(context).delegate;

    return LocalizationProvider(
      state: LocalizationProvider.of(context).state,
      child: MaterialApp(
        navigatorKey: rootAppNavigatorKey,
        debugShowCheckedModeBanner: false,
        title: 'TPFive',
        localizationsDelegates: [
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
          GlobalCupertinoLocalizations.delegate,
          localizationDelegate,
        ],
        supportedLocales: localizationDelegate.supportedLocales,
        locale: localizationDelegate.currentLocale,
        initialRoute: '/',
        navigatorObservers: [routeObserver],
        onGenerateRoute: (RouteSettings settings) {
          return routeTo(settings);
        },
        theme: AppTheme.defaultTheme(),
      ),
    );
  }
}
