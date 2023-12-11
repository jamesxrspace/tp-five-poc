import 'dart:convert';

import 'package:http/http.dart' as http;
import 'package:tpfive/xrauth/constants/account_const.dart';
import 'package:tpfive/utils/logger.dart';

class HttpClientApi {
  static const String TAG = 'HttpClientApi';
  static const String _oidcScope =
      'openid offline_access profile username email';

  late String _authingDomain;
  late String _clientId;
  late String _serverDomain;

  HttpClientApi(String authingDomain, String clientId, String serverDomain) {
    _authingDomain = authingDomain;
    _clientId = clientId;
    _serverDomain = serverDomain;
  }

  Future<http.Response> signInByUsername(String username, String password) {
    Uri url = _getUri(_authingDomain, AccountConst.authingTokenEndpoint);
    Log.d(TAG, 'url: $url');
    return http.post(
      url,
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: {
        'client_id': _clientId,
        'grant_type': 'password',
        'email': username,
        'password': password,
        'scope': _oidcScope
      },
      encoding: Encoding.getByName('utf-8'),
    );
  }

  Future<http.Response> renewAuth(String refreshToken) {
    Uri url = _getUri(_authingDomain, AccountConst.authingTokenEndpoint);
    Log.d(TAG, 'url: $url.toString()');
    return http.post(
      url,
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: {
        'client_id': _clientId,
        'grant_type': 'refresh_token',
        'refresh_token': refreshToken
      },
      encoding: Encoding.getByName('utf-8'),
    );
  }

  Future<http.Response> startDeviceAuth() {
    Uri url = _getUri(_serverDomain, AccountConst.deviceAuthEndpoint);
    Log.d(TAG, 'url: $url.toString()');
    return http.post(
      url,
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: {'client_id': _clientId, 'scene': 'APP_AUTH'},
      encoding: Encoding.getByName('utf-8'),
    );
  }

  Future<http.Response> pollingDeviceCodeStatus(String userCode) {
    Uri url = _getUri(_serverDomain, AccountConst.deviceAuthStatusEndpoint,
        'client_id=$_clientId&user_code=$userCode');
    Log.d(TAG, 'url: $url.toString()');
    return http.get(url);
  }

  Future<http.Response> authingTokenToCredential(String authingToken) {
    Uri url = _getUri(
        _serverDomain, AccountConst.deviceAuthingIdTokenToAccessTokenEndpoint);
    Log.d(TAG, 'url: $url.toString()');
    return http.post(
      url,
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: {
        'client_id': _clientId,
        'grant_type': AccountConst.idTokenToCredentialsGrantType,
        'redirect_uri': AccountConst.idTokenToCredentialsRedirectUri,
        'token': authingToken,
        'scope': _oidcScope
      },
      encoding: Encoding.getByName('utf-8'),
    );
  }

  Future<http.Response> createGuestAccount(String nickname) {
    Uri uri = _getUri(_serverDomain, AccountConst.guestCreateEndpoint);
    Log.d(TAG, 'url: $uri.toString()');
    return http.post(
      uri,
      headers: {
        'Content-Type': 'application/json; charset=UTF-8',
      },
      body: json.encode({'nickname': nickname}),
      encoding: Encoding.getByName('utf-8'),
    );
  }

  Uri _getUri(String domain, String endpoints, [String? queryString]) {
    Uri uri = Uri.parse(domain);
    return Uri(
      scheme: uri.scheme,
      host: uri.host,
      port: uri.port,
      path: endpoints,
      query: queryString,
    );
  }

  bool isSuccessResponse(int httpCode) {
    return httpCode >= 200 && httpCode <= 299;
  }
}
