import 'dart:convert';

class DeviceAuthStatusResponse {
  int status;
  String ticket;
  String token;

  DeviceAuthStatusResponse({
    required this.status,
    required this.ticket,
    required this.token,
  });

  static DeviceAuthStatusResponse parse(String str) =>
      DeviceAuthStatusResponse.fromJson(json.decode(str));

  static String toJsonString(DeviceAuthStatusResponse data) =>
      json.encode(data.toJson());

  factory DeviceAuthStatusResponse.fromJson(Map<String, dynamic> json) =>
      DeviceAuthStatusResponse(
        status: json['status'],
        ticket: json['ticket'],
        token: json['token'],
      );

  Map<String, dynamic> toJson() => {
        'status': status,
        'ticket': ticket,
        'token': token,
      };
}
