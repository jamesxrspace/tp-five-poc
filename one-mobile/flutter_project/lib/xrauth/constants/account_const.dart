import 'dart:io';

import 'package:package_info_plus/package_info_plus.dart';

class AccountConst {
  static const String authingTokenEndpoint = '/oidc/token';
  static const String guestCreateEndpoint = '/authing/guest';
  static const String deviceAuthEndpoint = '/authing/device/code';
  static const String deviceAuthStatusEndpoint = '/authing/device/code/status';
  static const String deviceAuthingIdTokenToAccessTokenEndpoint =
      '/oauth/oidc/token';
  static const String idTokenToCredentialsGrantType =
      'http://authing.cn/oidc/grant_type/authing_token';
  static const String idTokenToCredentialsRedirectUri =
      'https://xraccount.xrspace.net.cn/authing/authn';
  static const String authingGlobalDomain = '.us.authing.co';
  static const String authingTPFiveDomain = 'tpfive-qa.us.authing.co';

  static Future<String> redirectUri(String serverDomain) async {
    final packageInfo = await PackageInfo.fromPlatform();
    return '$serverDomain/api/v1/auth/authing/authn/${Platform.operatingSystem}/${packageInfo.packageName}';
  }
}
