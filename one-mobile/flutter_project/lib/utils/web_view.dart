import 'package:flutter/material.dart';
import 'package:tpfive/component/common_app_bar_builder.dart';
import 'package:tpfive/constants/style.dart';
import 'package:webview_flutter/webview_flutter.dart';

class WebViewScreen extends StatelessWidget {
  final String title;
  final String url;
  final bool isCancelButton;

  const WebViewScreen(
      {Key? key,
      required this.url,
      required this.title,
      required this.isCancelButton})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    final WebViewController controller = WebViewController()
      ..setJavaScriptMode(JavaScriptMode.unrestricted)
      ..setBackgroundColor(const Color(0x00000000))
      ..setNavigationDelegate(
        NavigationDelegate(
          onProgress: (int progress) {},
          onPageStarted: (String url) {},
          onPageFinished: (String url) {},
          onWebResourceError: (WebResourceError error) {},
        ),
      )
      ..loadRequest(Uri.parse(url));

    return Container(
      color: ThemeColors.mainDarkGray,
      child: SafeArea(
        top: true,
        bottom: true,
        child: Scaffold(
          backgroundColor: ThemeColors.mainDarkGray,
          appBar: CommonAppBarBuilder.build(
            centerTitle: false,
            leading: IconButton(
              icon: const Icon(Icons.favorite),
              iconSize: 24,
              color: ThemeColors.iconColor,
              onPressed: () {
                Navigator.pop(context, true);
              },
            ),
            title: Text(title, style: ThemeTextStyle.h2),
          ),
          body: WebViewWidget(controller: controller),
        ),
      ),
    );
  }
}
