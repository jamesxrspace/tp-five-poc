import 'dart:convert';

class DeviceAuth {
  static const String TAG = 'DeviceAuth';

  String deviceCode;
  String userCode;
  String verificationUri;
  int expiresIn;
  int interval;
  String verificationUriComplete;

  DeviceAuth({
    required this.deviceCode,
    required this.userCode,
    required this.verificationUri,
    required this.expiresIn,
    required this.interval,
    required this.verificationUriComplete,
  });

  static DeviceAuth parse(String str) => DeviceAuth.fromJson(json.decode(str));

  static String toJsonString(DeviceAuth data) => json.encode(data.toJson());

  factory DeviceAuth.fromJson(Map<String, dynamic> json) => DeviceAuth(
        deviceCode: json['device_code'],
        userCode: json['user_code'],
        verificationUri: json['verification_uri'],
        expiresIn: json['expires_in'],
        interval: json['interval'],
        verificationUriComplete: json['verification_uri_complete'],
      );

  Map<String, dynamic> toJson() => {
        'device_code': deviceCode,
        'user_code': userCode,
        'verification_uri': verificationUri,
        'expires_in': expiresIn,
        'interval': interval,
        'verification_uri_complete': verificationUriComplete,
      };
}
