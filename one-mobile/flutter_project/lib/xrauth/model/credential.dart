import 'dart:convert';

import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:tpfive/utils/logger.dart';

class Credential {
  static const String TAG = 'Credential';

  String accessToken;
  String idToken;
  String refreshToken;
  int? expiredAt; // refresh token expiration time
  String tokenType;
  String scope;

  Credential({
    required this.accessToken,
    required this.idToken,
    required this.refreshToken,
    required this.tokenType,
    required this.scope,
    this.expiredAt,
  }) {
    try {
      expiredAt = JwtDecoder.getExpirationDate(idToken)
              .millisecondsSinceEpoch ~/
          Duration
              .millisecondsPerSecond; // convert to seconds for aligning with unity game-account
    } catch (e) {
      Log.e(TAG, 'jwt decoder getExpirationDate exception: $e');
      expiredAt = 0;
    }
  }

  static String toJsonString(Credential credential) =>
      json.encode(credential.toJson());

  static Credential parse(String str) => Credential.fromJson(json.decode(str));

  static bool isValid(Credential credential) {
    Log.d(TAG, 'Check credential IsValid()');
    if (credential.accessToken.isEmpty) {
      Log.d(TAG, 'access_token not exist. Credential is not valid.');
      return false;
    }

    bool isAccessTokenExpired = isTokenExpired(credential.accessToken);
    if (!isAccessTokenExpired) {
      Log.d(TAG, 'The access token is valid');
      return true;
    }
    // Because the refresh_token does not have expiration time, we check the id_token instead.
    // NOTE: the expiration time of the refresh_token is set to be the same as the id_token in Authing
    bool isRefreshTokenExpired = isTokenExpired(credential.idToken);
    if (credential.refreshToken.isNotEmpty && !isRefreshTokenExpired) {
      Log.d(TAG, 'Although access token is expired but can be refresh.(valid)');
      return true;
    }
    Log.d(TAG, 'Credential is not valid');
    return false;
  }

  static bool isTokenExpired(String? token) {
    if (token == null || token.isEmpty) return true;
    try {
      return JwtDecoder.isExpired(token);
    } catch (e) {
      Log.e(TAG, 'check token expiration exception: $e');
      return true;
    }
  }

  factory Credential.fromJson(Map<String, dynamic> json) => Credential(
        accessToken: json['access_token'],
        idToken: json['id_token'],
        refreshToken: json['refresh_token'],
        tokenType: json['token_type'],
        scope: json['scope'],
        expiredAt: json['expired_at'] as int?,
      );

  Map<String, dynamic> toJson() => {
        'access_token': accessToken,
        'id_token': idToken,
        'refresh_token': refreshToken,
        'expired_at': expiredAt,
        'token_type': tokenType,
        'scope': scope,
      };
}
