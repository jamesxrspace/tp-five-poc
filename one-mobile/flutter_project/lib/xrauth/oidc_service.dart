import 'dart:convert';

import 'package:authing_sdk_v3/authing.dart';
import 'package:authing_sdk_v3/oidc/auth_request.dart';
import 'package:authing_sdk_v3/oidc/oidc_client.dart';
import 'package:authing_sdk_v3/result.dart' as authing_result;
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tpfive/main.dart';
import 'package:tpfive/providers/loading_config_provider.dart';
import 'package:tpfive/utils/logger.dart';
import 'package:tpfive/xrauth/constants/account_const.dart';
import 'package:tpfive/xrauth/constants/auth_error.dart';
import 'package:tpfive/xrauth/oidc_webview.dart';

import 'callback/auth_result.dart';
import 'model/credential.dart';

class OIDCWebViewService {
  static const String TAG = 'OIDCService';
  late final String _clientId;
  late final String _userPoolId;
  late final String _redirectServerDomain;
  late final String _redirectUrl;
  late final AuthRequest _authRequest;

  static const scopes = [
    'openid',
    'profile',
    'email',
    'offline_access',
    'username'
  ];

  OIDCWebViewService(
      this._clientId, this._userPoolId, this._redirectServerDomain) {
    init();
  }

  Future<void> init() async {
    _redirectUrl = await AccountConst.redirectUri(_redirectServerDomain);
    Authing.setOnPremiseInfo(AccountConst.authingGlobalDomain);
    Authing.init(_userPoolId, _clientId);
    _authRequest = AuthRequest();
    _authRequest.createAuthRequest();
    _authRequest.redirectUrl = _redirectUrl;
    _authRequest.scope = scopes.join(' ');
  }

  Future<Credential> exchageToken(String authCode) async {
    authing_result.AuthResult res =
        await OIDCClient.authByCode(authCode, _authRequest);
    return Credential(
      accessToken: res.user?.accessToken ?? '',
      idToken: res.user?.idToken ?? '',
      refreshToken: res.user?.refreshToken ?? '',
      tokenType: '',
      scope: _authRequest.scope,
    );
  }

  Future<AuthResult<String>> login() async {
    try {
      var result = await loginWebView();
      if (result.isSuccess) {
        var jsonString = json.encode(result.payload?.toJson());
        return AuthResult.success(jsonString);
      } else {
        return AuthResult.failure(result.error ?? AuthError.invalidResponse);
      }
    } on PlatformException catch (e) {
      Log.d(TAG, 'login error: $e');
      final intCode = int.tryParse(e.code) ?? AuthError.invalidResponse.code;
      return AuthResult.failureWithCode(intCode, e.message ?? 'sign in failed');
    }
  }

  Future<AuthResult<Credential>> loginWebView() async {
    try {
      Authing.setOnPremiseInfo(AccountConst.authingTPFiveDomain);
      var url = await OIDCClient.buildAuthorizeUrl(_authRequest);
      String authCode =
          await rootAppNavigatorKey.currentState!.push(MaterialPageRoute(
              builder: (context) => OIDCWebView(
                    url: url,
                  )));
      globalProviderContainer.read(loadingStateProvider.notifier).show();
      var credential = await exchageToken(authCode);
      return AuthResult.success(credential);
    } catch (e) {
      Log.e(TAG, 'OIDC login error: $e');
      return AuthResult.failure(AuthError.unknown);
    } finally {
      globalProviderContainer.read(loadingStateProvider.notifier).hide();
    }
  }
}
