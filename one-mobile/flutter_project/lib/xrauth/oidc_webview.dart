import 'package:flutter/material.dart';
import 'package:flutter_inappwebview/flutter_inappwebview.dart';

class OIDCWebView extends StatefulWidget {
  final String url;

  const OIDCWebView({
    super.key,
    required this.url,
  });

  @override
  OIDCWebViewState createState() => OIDCWebViewState();
}

class OIDCWebViewState extends State<OIDCWebView> {
  final GlobalKey webViewKey = GlobalKey();
  final String xrspaceUrl = 'xrspace://';

  InAppWebViewController? webViewController;

  // this setting is to prevent google singin disallow problem
  // https://github.com/pichillilorenzo/flutter_inappwebview/issues/1112
  InAppWebViewGroupOptions options = InAppWebViewGroupOptions(
      crossPlatform: InAppWebViewOptions(
    userAgent: 'random',
    javaScriptEnabled: true,
    useShouldOverrideUrlLoading: true,
    useOnLoadResource: true,
    cacheEnabled: true,
  ));

  @override
  void initState() {
    super.initState();
    CookieManager.instance().deleteAllCookies();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(title: const Text('Login')),
        body: SafeArea(
            child: Column(children: <Widget>[
          Expanded(
            child: Stack(
              children: [
                InAppWebView(
                  key: webViewKey,
                  initialUrlRequest: URLRequest(url: Uri.parse(widget.url)),
                  initialOptions: options,
                  onWebViewCreated: (controller) {
                    webViewController = controller;
                  },
                  shouldOverrideUrlLoading:
                      (controller, navigationAction) async {
                    final uri = navigationAction.request.url;
                    if (uri.toString().startsWith(xrspaceUrl)) {
                      String authCode = uri?.queryParameters['code'] ?? '';
                      Navigator.of(context).pop(authCode);
                      return NavigationActionPolicy.CANCEL;
                    }
                    return NavigationActionPolicy.ALLOW;
                  },
                ),
              ],
            ),
          ),
        ])));
  }
}
