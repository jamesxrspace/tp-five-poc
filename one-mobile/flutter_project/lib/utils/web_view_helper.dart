import 'package:flutter/material.dart';
import 'package:tpfive/utils/web_view.dart';

class WebViewHelper {
  static void launchUrl(BuildContext context, String link, String title) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => WebViewScreen(
          isCancelButton: true,
          title: title,
          url: link,
        ),
      ),
    );
  }
}
