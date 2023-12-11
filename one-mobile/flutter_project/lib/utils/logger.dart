import 'package:logger/logger.dart';

class Log {
  static final Logger _log = Logger(
    printer: PrettyPrinter(),
  );

  static void v(String tag, String message) {
    _log.v('[$tag]::$message');
  }

  static void d(String tag, String message) {
    _log.d('[$tag]::$message');
  }

  static void i(String tag, String message) {
    _log.i('[$tag]::$message');
  }

  static void w(String tag, String message) {
    _log.w('[$tag]::$message');
  }

  static void e(String tag, String message) {
    _log.e('[$tag]::$message');
  }

  static void wtf(String tag, String message) {
    _log.wtf('[$tag]::$message');
  }
}
