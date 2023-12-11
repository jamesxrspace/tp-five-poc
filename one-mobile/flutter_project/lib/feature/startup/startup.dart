import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_translate/flutter_translate.dart';
import 'package:get_it/get_it.dart';
import 'package:styled_text/styled_text.dart';
import 'package:tpfive/component/full_screen_loading.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/feature/launch_unity/launch_unity.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive/services/account_service.dart';
import 'package:tpfive/services/gameserver_service.dart';
import 'package:tpfive/utils/logger.dart';
import 'package:tpfive/utils/web_view_helper.dart';

class StartupScreen extends ConsumerStatefulWidget {
  static const routeName = '/startup';
  static const String tag = 'Startup';

  const StartupScreen({super.key});

  @override
  StartupScreenState createState() => StartupScreenState();
}

class StartupScreenState extends ConsumerState<StartupScreen> {
  bool _loginBtnEnabled = true;
  final AccountService _accountService = GetIt.I<AccountService>();
  final GameServerService _gameServerService = GetIt.I<GameServerService>();

  final AssetImage brandImage =
      const AssetImage('assets/image/launch_brand.png');

  @override
  void initState() {
    super.initState();
  }

  Future<void> _startLoginFlow(BuildContext context) async {
    setState(() {
      _loginBtnEnabled = false;
    });

    try {
      await _accountService.signInUserByWebView();
      await _gameServerService.login();
      _routeTo(LaunchUnity.routeName);
    } catch (e) {
      Log.e(StartupScreen.tag, 'sign in failed: $e');
    } finally {
      setState(() {
        _loginBtnEnabled = true;
      });
    }
  }

  void _routeTo(String routeName) {
    Log.i(StartupScreen.tag, 'Navigate to $routeName');
    Navigator.pushNamed(context, routeName);
  }

  @override
  Widget build(BuildContext context) {
    var displayLoading = ref.watch(loadingStateProvider).display;

    return Scaffold(
        backgroundColor: Colors.white,
        body: Stack(children: [
          Center(
              child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              ElevatedButton(
                style: ElevatedButton.styleFrom(
                  textStyle: const TextStyle(fontSize: 25),
                  backgroundColor: Colors.blue,
                  foregroundColor: Colors.white,
                ),
                onPressed: () {
                  if (!_loginBtnEnabled) {
                    return;
                  }

                  _startLoginFlow(context);
                },
                child: Text(
                  _loginBtnEnabled ? 'Login' : 'Logging in...',
                ),
              ),
              Image(
                image: brandImage,
                fit: BoxFit.none,
              ),
              SizedBox(
                width: 200,
                child: _buildStyledText(
                  translate(
                    'General_Confirm_TOU_Privacy',
                    args: {'AppName': translate('AppName_TPFive')},
                  ),
                ),
              ),
            ],
          )),
          if (displayLoading) const Positioned.fill(child: FullScreenLoading())
        ]));
  }

  StyledText _buildStyledText(final String string) {
    void openLink(String? text, Map<String?, String?> attributes) {
      // TODO: Fix missing text
      final String? link = attributes['link'];
      WebViewHelper.launchUrl(context, link!, text ?? '');
    }

    return StyledText(text: string, textAlign: TextAlign.center, tags: {
      'b': StyledTextTag(style: const TextStyle(fontWeight: FontWeight.bold)),
      'link': StyledTextActionTag(
        openLink,
        style: const TextStyle(
            color: ThemeColors.buttonPressedPink,
            decoration: TextDecoration.underline),
      ),
    });
  }
}
