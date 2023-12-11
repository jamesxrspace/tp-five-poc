import 'package:flutter/material.dart';

@immutable
class ColorPalette extends ThemeExtension<ColorPalette> {
  const ColorPalette({
    required this.primary,
    required this.error,
    required this.record,
    required this.like,
    required this.timestamp,
    required this.grey,
  });

  final Color? primary;
  final Color? error;
  final Color? record;
  final Color? like;
  final Color? timestamp;
  final MaterialColor? grey;

  @override
  ColorPalette copyWith(
      {Color? primary,
      Color? error,
      Color? record,
      Color? like,
      Color? timestamp,
      MaterialColor? grey}) {
    return ColorPalette(
      primary: primary ?? this.primary,
      error: error ?? this.error,
      record: record ?? this.record,
      like: like ?? this.like,
      timestamp: timestamp ?? this.timestamp,
      grey: grey ?? this.grey,
    );
  }

  @override
  ColorPalette lerp(ColorPalette? other, double t) {
    if (other is! ColorPalette) {
      return this;
    }
    return ColorPalette(
      primary: primary,
      error: error,
      record: record,
      like: like,
      timestamp: timestamp,
      grey: grey,
    );
  }
}

class AppTheme {
  static const Color primaryColor = Color.fromRGBO(173, 255, 91, 1);
  static const int _primaryValue = 0xFFADFF5B;
  static const int _grayPrimaryValue = 0xFF888888;

  static ThemeData defaultTheme() {
    return ThemeData.light().copyWith(
      primaryColor: primaryColor,
      scaffoldBackgroundColor: Colors.black,
      extensions: <ThemeExtension<dynamic>>[
        const ColorPalette(
          primary: Color(_primaryValue),
          error: Color.fromRGBO(255, 58, 58, 1),
          record: Color.fromRGBO(255, 89, 99, 1),
          like: Color.fromRGBO(249, 55, 125, 1),
          timestamp: Color.fromRGBO(251, 255, 74, 1),
          grey: MaterialColor(
            _grayPrimaryValue,
            <int, Color>{
              50: Color.fromRGBO(238, 238, 238, 1),
              100: Color.fromRGBO(217, 217, 217, 1),
              200: Color.fromRGBO(204, 204, 204, 1),
              300: Color.fromRGBO(179, 179, 179, 1),
              400: Color.fromRGBO(151, 151, 151, 1),
              500: Color(_grayPrimaryValue),
              600: Color.fromRGBO(128, 128, 128, 1),
              700: Color.fromRGBO(102, 102, 102, 1),
              800: Color.fromRGBO(63, 63, 63, 1),
              900: Color.fromRGBO(16, 16, 28, 1),
            },
          ),
        ),
      ],
    );
  }
}

// TODO: Replace with colors from theme extension
class ThemeColors {
  static const Color mainDarkGray = Color(0xff10101c);
  static const Color iconColor = Color(0xfff3f3f3);
  static const Color buttonPressedPink = Color(0x40cccccc);
  static const Color mainColor = Color.fromRGBO(173, 255, 91, 1);
}

class ThemeSpacing {
  static const double titleLeading = 8;
  static const double main = 20;
}

class ThemeTextStyle {
  static const h1 =
      TextStyle(fontSize: 24, fontWeight: FontWeight.w700, color: Colors.white);
  static const h2 =
      TextStyle(fontSize: 18, fontWeight: FontWeight.w600, color: Colors.white);
  static const h4 =
      TextStyle(fontSize: 14, fontWeight: FontWeight.w600, color: Colors.white);
  static const h7 =
      TextStyle(fontSize: 12, fontWeight: FontWeight.w500, color: Colors.white);
  static const body1 =
      TextStyle(fontSize: 16, fontWeight: FontWeight.w500, color: Colors.white);
  static const body2 =
      TextStyle(fontSize: 15, fontWeight: FontWeight.w600, color: Colors.white);
  static const body3 = TextStyle(
      fontSize: 15,
      fontWeight: FontWeight.w400,
      height: 1.73,
      color: Colors.white);
  static const body4 =
      TextStyle(fontSize: 14, fontWeight: FontWeight.w500, color: Colors.white);
  static const body5 = TextStyle(
      fontSize: 12,
      fontWeight: FontWeight.w400,
      height: 1.73,
      color: Colors.white);
  static const buttonSmall =
      TextStyle(fontSize: 10, fontWeight: FontWeight.w600, color: Colors.white);
  static const number =
      TextStyle(fontSize: 24, fontWeight: FontWeight.w500, color: Colors.white);
  static const listItem =
      TextStyle(fontSize: 14, fontWeight: FontWeight.w500, color: Colors.white);
  static const submitButtonDisable = TextStyle(
      fontSize: 15, fontWeight: FontWeight.w500, color: Colors.white30);
}

class ThemeStyle {
  static RoundedRectangleBorder modalBorder = const RoundedRectangleBorder(
    borderRadius: BorderRadius.only(
      topLeft: Radius.circular(30.0),
      topRight: Radius.circular(30.0),
    ),
  );
}
