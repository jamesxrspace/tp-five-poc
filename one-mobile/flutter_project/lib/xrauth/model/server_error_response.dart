import 'dart:convert';

class ServerErrorResponse {
  String status;
  ErrorResponse error;

  ServerErrorResponse({
    required this.status,
    required this.error,
  });

  static ServerErrorResponse parse(String str) =>
      ServerErrorResponse.fromJson(json.decode(str));

  static String toJsonString(ServerErrorResponse data) =>
      json.encode(data.toJson());

  factory ServerErrorResponse.fromJson(Map<String, dynamic> json) =>
      ServerErrorResponse(
        status: json['status'],
        error: ErrorResponse.fromJson(json['error']),
      );

  Map<String, dynamic> toJson() => {
        'status': status,
        'error': error.toJson(),
      };
}

class ErrorResponse {
  String code;
  String codes;
  String message;

  ErrorResponse({
    required this.code,
    required this.codes,
    required this.message,
  });

  factory ErrorResponse.fromJson(Map<String, dynamic> json) => ErrorResponse(
        code: json['code'],
        codes: json['codes'],
        message: json['message'],
      );

  Map<String, dynamic> toJson() => {
        'code': code,
        'codes': codes,
        'message': message,
      };
}
