import 'package:tpfive/xrauth/callback/auth_result.dart';
import 'package:tpfive/xrauth/model/credential.dart';
import 'package:tpfive/xrauth/model/device_auth.dart';
import 'package:tpfive/xrauth/model/guest.dart';

abstract class XrAuth {
  Future<AuthResult<Credential>> signInUserByWebView();

  Future<AuthResult<Credential>> signInUserByUsername(
      String username, String password);

  Future<AuthResult<String>> signOutUser(String idToken);

  Future<AuthResult<Credential>> renewAccessToken(String refreshToken);

  Future<AuthResult<DeviceAuth>> startDeviceAuth();

  Future<AuthResult<Credential>> getCurrentDeviceAuthState(
      String userCode, int pollingInterval);

  Future<AuthResult<Guest>> createGuestAccount(String nickname);

  Future<AuthResult<String>> readData(String key);

  Future<AuthResult<bool>> saveData(String key, String value);

  Future<AuthResult<bool>> deleteData(String key);
}
