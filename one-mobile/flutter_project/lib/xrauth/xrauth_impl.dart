import 'dart:async';

import 'package:flutter/services.dart';
import 'package:flutter_keychain/flutter_keychain.dart';
import 'package:http/http.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:tpfive/utils/logger.dart';
import 'package:tpfive/xrauth/callback/auth_result.dart';
import 'package:tpfive/xrauth/constants/auth_error.dart';
import 'package:tpfive/xrauth/constants/device_auth_status.dart';
import 'package:tpfive/xrauth/model/create_guest_response.dart';
import 'package:tpfive/xrauth/model/credential.dart';
import 'package:tpfive/xrauth/model/device_auth.dart';
import 'package:tpfive/xrauth/model/device_auth_status_response.dart';
import 'package:tpfive/xrauth/model/guest.dart';
import 'package:tpfive/xrauth/oidc_service.dart';
import 'package:tpfive/xrauth/util/http_client_api.dart';
import 'package:tpfive/xrauth/xrauth.dart';

class XrAuthImpl implements XrAuth {
  static const String TAG = 'XrAuthImpl';

  late final String _authingDomain;
  late final String _clientId;
  late final String _userPoolId;
  late final String _serverDomain;
  late final String _redirectServerDomain;

  late OIDCWebViewService _oidcWebViewService;
  late HttpClientApi _authClient;

  XrAuthImpl(this._authingDomain, this._clientId, this._userPoolId,
      this._serverDomain, this._redirectServerDomain) {
    _oidcWebViewService = OIDCWebViewService(
        _clientId, _userPoolId, _redirectServerDomain); // by webview
    _authClient = HttpClientApi(_authingDomain, _clientId, _serverDomain);
  }

  @override
  Future<AuthResult<Credential>> signInUserByWebView() async {
    Log.d(TAG, 'sign in by webView');
    final response = await _oidcWebViewService.login();

    if (!response.isSuccess) {
      return AuthResult.failure(response.error ?? AuthError.invalidResponse);
    }

    if (response.payload?.isEmpty ?? true) {
      return AuthResult.failure(AuthError.invalidResponse);
    }

    return _checkCredential(response.payload);
  }

  @override
  Future<AuthResult<Credential>> signInUserByUsername(
      String username, String password) {
    Log.d(TAG, 'sign in by id/pwd - username: $username');
    return _authClient.signInByUsername(username, password).then((response) {
      final error = _processHttpResponse(response);
      return error == AuthError.none
          ? _checkCredential(response.body)
          : AuthResult.failure(error);
    });
  }

  @override
  Future<AuthResult<String>> signOutUser(String idToken) async {
    throw UnimplementedError('This method is not yet implemented');
  }

  @override
  Future<AuthResult<Credential>> renewAccessToken(String refreshToken) async {
    Log.d(TAG, 'try to renew access token');
    final response = await _authClient.renewAuth(refreshToken);
    final error = _processHttpResponse(response);
    if (error != AuthError.none) {
      return AuthResult.failure(AuthError.renewAccessTokenFailed);
    }
    return _checkCredential(response.body);
  }

  @override
  Future<AuthResult<DeviceAuth>> startDeviceAuth() async {
    Log.d(TAG, 'start device auth');
    final response = await _authClient.startDeviceAuth();
    final error = _processHttpResponse(response);
    if (error != AuthError.none) {
      return AuthResult.failure(AuthError.cannotGetDeviceCode);
    }

    final deviceAuth = DeviceAuth.parse(response.body);
    Log.i(TAG,
        'startDeviceAuth() - generate device code success. device_code: ${deviceAuth.deviceCode} , user_code: ${deviceAuth.userCode}');
    return AuthResult.success(deviceAuth);
  }

  @override
  Future<AuthResult<Credential>> getCurrentDeviceAuthState(
      String userCode, int pollingInterval) async {
    Log.d(TAG, 'get device auth state, user_code: $userCode');
    var stopStatePolling = false;

    while (!stopStatePolling) {
      final response = await _authClient.pollingDeviceCodeStatus(userCode);
      final error = _processHttpResponse(response);
      if (error != AuthError.none) {
        stopStatePolling = true;
        return AuthResult.failure(AuthError.authTokenFailed);
      }

      final statusResponse = DeviceAuthStatusResponse.parse(response.body);
      switch (statusResponse.status) {
        case DeviceAuthStatus.unAuth:
        case DeviceAuthStatus.scanned:
          Log.i(TAG, 'waiting for user enter activation code.');
          break;
        case DeviceAuthStatus.confirm:
          Log.i(TAG, 'get authing token success, try to get credential.');
          stopStatePolling = true;
          final resp =
              await _authClient.authingTokenToCredential(statusResponse.token);
          final respError = _processHttpResponse(resp);
          if (respError != AuthError.none) {
            return AuthResult.failure(AuthError.authTokenFailed);
          }
          return _checkCredential(resp.body);
        case DeviceAuthStatus.expired:
        default:
          Log.w(TAG, 'device code has expired: $statusResponse');
          stopStatePolling = true;
          return AuthResult.failure(AuthError.deviceCodeExpired);
      }

      Log.i(TAG, 'wait for $pollingInterval seconds.');
      await Future.delayed(Duration(seconds: pollingInterval));
    }

    return AuthResult.failure(AuthError.unknown);
  }

  @override
  Future<AuthResult<Guest>> createGuestAccount(String nickname) async {
    Log.d(TAG, 'create guest account');
    final response = await _authClient.createGuestAccount(nickname);
    final error = _processHttpResponse(response);
    if (error != AuthError.none) {
      return AuthResult.failure(AuthError.createGuestFailed);
    }

    final responseBody = CreateGuestResponse.parse(response.body);
    final guest = responseBody.guestInfo;
    if (guest.userId.isEmpty) {
      Log.e(TAG, 'createGuestAccount() - guest user id is empty');
      return AuthResult.failure(AuthError.createGuestFailed);
    }

    Log.d(TAG, 'create guest account success. guest id: ${guest.userId}');
    return AuthResult.success(guest);
  }

  @override
  Future<AuthResult<String>> readData(String key) async {
    Log.d(TAG, 'getString($key)');
    try {
      var result = await FlutterKeychain.get(key: key);
      return AuthResult.success(result ?? '');
    } on PlatformException catch (e) {
      Log.d(TAG, 'keychain reed data fail: $e');
      return AuthResult.failure(AuthError.readDataFailed);
    }
  }

  @override
  Future<AuthResult<bool>> saveData(String key, String value) async {
    Log.d(TAG, 'setString($key)');
    try {
      await FlutterKeychain.put(key: key, value: value);
      return AuthResult.success(true);
    } catch (e) {
      Log.d(TAG, 'keychain save data fail: $e');
      return AuthResult.failure(AuthError.saveDataFailed);
    }
  }

  @override
  Future<AuthResult<bool>> deleteData(String key) async {
    Log.d(TAG, 'remove($key)');
    try {
      await FlutterKeychain.remove(key: key);
      return AuthResult.success(true);
    } catch (e) {
      Log.d(TAG, 'keychain delete data fail: $e');
      return AuthResult.failure(AuthError.deleteDataFailed);
    }
  }

  AuthError _processHttpResponse(Response response) {
    if (!_authClient.isSuccessResponse(response.statusCode)) {
      Log.w(TAG,
          '_processHttpResponse() - failed. http_code: ${response.statusCode} , error_msg: ${response.body}');
      return AuthError.requestFailed;
    }
    if (response.body.isEmpty) {
      Log.e(TAG, '_processHttpResponse() - response body is invalid');
      return AuthError.invalidResponse;
    }

    return AuthError.none;
  }

  AuthResult<Credential> _checkCredential(String? credentialsStr) {
    if (credentialsStr == null || credentialsStr.isEmpty) {
      return AuthResult.failure(AuthError.invalidCredentials);
    }

    final credentials = Credential.parse(credentialsStr);
    if (!Credential.isValid(credentials)) {
      Log.w(TAG, 'credentials is invalid.');
      return AuthResult.failure(AuthError.invalidCredentials);
    }

    Log.i(TAG,
        'access token expired at: ${JwtDecoder.getExpirationDate(credentials.accessToken)}');

    return AuthResult.success(credentials);
  }
}
