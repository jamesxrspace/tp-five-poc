import 'package:tpfive/xrauth/constants/auth_error.dart';

class AuthResult<T> {
  bool get isSuccess => error == null && payload != null;

  T? payload;

  AuthError? error;

  AuthResult({this.payload, this.error});

  @override
  String toString() {
    return 'AuthResult{error: $error}';
  }

  static AuthResult<T> success<T>(T payload) {
    return AuthResult<T>(payload: payload);
  }

  static AuthResult<T> failure<T>(AuthError error) {
    return AuthResult<T>(error: error);
  }

  static AuthResult<T> failureWithCode<T>(int code, String message) {
    return AuthResult<T>(error: AuthError(code, message));
  }
}
