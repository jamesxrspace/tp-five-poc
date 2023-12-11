import 'dart:async';

import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:tpfive/utils/logger.dart';
import 'package:tpfive/xrauth/callback/auth_result.dart';
import 'package:tpfive/xrauth/constants/auth_error.dart';
import 'package:tpfive/xrauth/model/credential.dart';
import 'package:tpfive/xrauth/model/device_auth.dart';
import 'package:tpfive/xrauth/model/guest.dart';
import 'package:tpfive/xrauth/xrauth.dart';
import 'package:tpfive/xrauth/xrauth_impl.dart';

class AccountService {
  static const String TAG = 'AccountService';
  static const String _tagCredentials = 'XrCredentials';
  static const String _tagGuestInfo = 'GuestInfo';

  Credential? _cachedCredential;
  Guest? _cachedGuest;

  late XrAuth _xrAuth;

  AccountService(String authingDomain, String clientId, String userPoolId,
      String serverDomain, String redirectServerDomain) {
    _xrAuth = XrAuthImpl(authingDomain, clientId, userPoolId, serverDomain,
        redirectServerDomain);
    _readLocalCredentials();
    _readLocalGuestInfo();
  }

  bool isSignedIn() {
    Log.d(TAG,
        'isSignedIn() - local credentials exists? ${_cachedCredential != null}');
    final cachedCredential = _cachedCredential;
    return cachedCredential != null && Credential.isValid(cachedCredential);
  }

  String getAccessToken() {
    Log.d(TAG, 'getAccessToken() - Get cached access token');
    final cachedCredential = _cachedCredential;
    if (cachedCredential == null || cachedCredential.accessToken.isEmpty) {
      Log.w(TAG, 'getAccessToken() - cached access token is invalid');
      return '';
    }
    return cachedCredential.accessToken;
  }

  /* If the local cached access token is valid, return it.
     Otherwise, try to renew it through refresh token.
   */
  Future<AuthResult<String>> tryGetValidToken() async {
    Log.d(TAG, 'tryGetValidToken()');
    if (_cachedCredential == null) {
      Log.d(TAG, 'Not sign in yet.');
      throw AuthResult.failure(AuthError.notSignedIn);
    }

    final cachedAccessToken = _cachedCredential?.accessToken ?? '';
    if (!Credential.isTokenExpired(cachedAccessToken)) {
      Log.d(TAG,
          'cached access token still valid. (expired at: ${JwtDecoder.getExpirationDate(_cachedCredential?.accessToken ?? '')})');
      return AuthResult.success(cachedAccessToken);
    }

    final cachedRefreshToken = _cachedCredential?.refreshToken ?? '';
    if (cachedRefreshToken.isEmpty) {
      Log.w(TAG, 'refresh token invalid');
      throw AuthResult.failure(AuthError.invalidRefreshToken);
    }

    return _xrAuth
        .renewAccessToken(cachedRefreshToken)
        .then((response) => _processCredential(response.payload));
  }

  Future<AuthResult<String>> signInUserByWebView() async {
    Log.d(TAG, 'signInUserByWebView()');
    return _xrAuth
        .signInUserByWebView()
        .then((response) => _processSignInResult(response));
  }

  Future<AuthResult<String>> signInUserByUsername(
      String username, String password) async {
    Log.d(TAG, 'signInUserByUsername()');

    final response = await _xrAuth.signInUserByUsername(username, password);
    final result = await _processSignInResult(response);
    if (result.isSuccess) {
      _saveAccountInfo(username, password);
    }

    return result;
  }

  Future<AuthResult<String>> renewToken() async {
    Log.d(TAG, 'renewToken()');
    if (_cachedCredential?.refreshToken.isEmpty ?? true) {
      Log.w(TAG, 'renewToken() - refresh token is empty');
      throw AuthResult.failure(AuthError.invalidRefreshToken);
    }
    return _xrAuth
        .renewAccessToken(_cachedCredential?.refreshToken ?? '')
        .then((response) => _processCredential(response.payload));
  }

  Future<AuthResult<String>> signOutUser() async {
    Log.d(TAG, 'signOutUser()');
    var idToken = _cachedCredential?.idToken ?? '';
    final deleteResult = await _xrAuth.deleteData(_tagCredentials);
    Log.d(TAG,
        'signOutUser() - delete credentials success? ${deleteResult.isSuccess}');
    if (deleteResult.isSuccess) _cachedCredential = null;
    return await _xrAuth.signOutUser(idToken);
  }

  Future<AuthResult<DeviceAuth>> startDeviceAuth() async {
    Log.d(TAG, 'startDeviceAuth()');
    return _xrAuth.startDeviceAuth();
  }

  Future<AuthResult<String>> getCurrentDeviceAuthState(
      String userCode, int pollingInterval) async {
    Log.d(TAG, 'getCurrentDeviceAuthState()');
    return _xrAuth
        .getCurrentDeviceAuthState(userCode, pollingInterval)
        .then((response) => _processSignInResult(response));
  }

  bool isGuestExist() {
    Log.d(TAG, 'isGuestExist()');
    return _cachedGuest?.userId.isNotEmpty ?? false;
  }

  bool isGuestSignedIn() {
    Log.d(TAG, 'isGuestSignedIn()');
    if (_cachedCredential?.accessToken.isEmpty ?? true) {
      Log.w(TAG,
          'isGuestSignedIn() - access token is empty. No one is signed in');
      return false;
    }
    var tokenPayload =
        JwtDecoder.tryDecode(_cachedCredential?.accessToken ?? '');
    if (tokenPayload?['role'] == null) return false;
    var role = tokenPayload?['role'].toString();
    return role == 'guest';
  }

  Future<AuthResult<String>> createGuestAccount(String nickname) async {
    Log.d(TAG, 'createGuestAccount()');
    final response = await _xrAuth.createGuestAccount(nickname);
    if (response.error != null) {
      Log.e(TAG,
          'createGuestAccount() - create guest account failed, code: ${response.error?.code} , msg: ${response.error?.message}');
      throw AuthResult.failure(response.error ?? AuthError.invalidResponse);
    }

    final guest = response.payload;
    if (guest == null) {
      Log.e(TAG, 'createGuestAccount() - guest is null');
      throw AuthResult.failure(AuthError.createGuestFailed);
    }

    final guestString = Guest.toJsonString(guest);
    final saveResult = await _xrAuth.saveData(_tagGuestInfo, guestString);
    Log.d(TAG,
        'createGuestAccount() - save guest info success? ${saveResult.isSuccess}');
    if (!saveResult.isSuccess) {
      throw AuthResult.failure(saveResult.error ?? AuthError.saveDataFailed);
    }
    _cachedGuest = guest;
    return AuthResult.success(guest.nickname);
  }

  Future<AuthResult<String>> signInGuest() {
    Log.d(TAG, 'signInGuest()');
    final cachedGuest = _cachedGuest;
    if (cachedGuest == null ||
        cachedGuest.userId.isEmpty ||
        cachedGuest.email.isEmpty ||
        cachedGuest.password.isEmpty) {
      Log.e(TAG, 'signInGuest() - invalid cached guest. sign in guest failed.');
      throw AuthResult.failure(AuthError.guestNotExist);
    }
    return signInUserByUsername(cachedGuest.email, cachedGuest.password);
  }

  Future<List<bool>> _saveAccountInfo(String username, String password) async {
    var prefs = await SharedPreferences.getInstance();
    return Future.wait([
      prefs.setString('username', username),
      prefs.setString('password', password)
    ]);
  }

  Future<AuthResult<String>> _processSignInResult(
      AuthResult<Credential> signInResponse) {
    final signInError = signInResponse.error;
    if (signInError != null) {
      Log.e(TAG,
          '_processSignInResult() - sign in failed, code: ${signInError.code} , msg: ${signInError.message}');
      throw AuthResult.failure(signInError);
    }
    final credentials = signInResponse.payload;
    return _processCredential(credentials);
  }

  Future<AuthResult<String>> _processCredential(Credential? credentials) async {
    if (credentials == null) {
      Log.w(TAG, '_processCredential() - credentials is null');
      throw AuthResult.failure(AuthError.invalidCredentials);
    }

    var credentialString = Credential.toJsonString(credentials);
    final saveResult =
        await _xrAuth.saveData(_tagCredentials, credentialString);
    Log.d(TAG,
        '_processCredential() - save credentials success? ${saveResult.isSuccess}');
    if (!saveResult.isSuccess) {
      throw AuthResult.failure(saveResult.error ?? AuthError.saveDataFailed);
    }

    _cachedCredential = credentials;
    return AuthResult.success(credentials.accessToken);
  }

  void _readLocalCredentials() async {
    final response = await _xrAuth.readData(_tagCredentials);
    final credentialString = response.payload ?? '';
    if (!response.isSuccess || credentialString.isEmpty) {
      Log.d(TAG, 'local credentials not exists.');
      _cachedCredential = null;
      return;
    }
    Log.d(TAG, 'read local credentials');
    _cachedCredential = Credential.parse(credentialString);
  }

  void _readLocalGuestInfo() async {
    await _xrAuth.readData(_tagGuestInfo).then((response) {
      final guestString = response.payload ?? '';
      if (!response.isSuccess || guestString.isEmpty) {
        Log.d(TAG, 'local guest info not exists.');
        _cachedGuest = null;
        return;
      }
      Log.d(TAG, 'read local guest info');
      _cachedGuest = Guest.parse(guestString);
    });
  }
}
