import 'package:flutter/material.dart';
import 'package:tpfive/constants/style.dart';

class CommonAppBarBuilder {
  static AppBar build(
      {Widget? leading,
      Widget? title,
      List<Widget>? actions,
      bool centerTitle = false,
      double? toolBarHeight = 60}) {
    bool hasLeading = leading != null;

    return AppBar(
      backgroundColor: Colors.transparent,
      centerTitle: centerTitle,
      titleSpacing: hasLeading ? 5 : 20,
      elevation: 0,
      toolbarHeight: toolBarHeight,
      leading: hasLeading
          ? Container(
              width: 44,
              margin: const EdgeInsets.only(left: ThemeSpacing.titleLeading),
              child: Center(child: leading),
            )
          : null,
      title: title,
      actions: actions,
    );
  }
}
