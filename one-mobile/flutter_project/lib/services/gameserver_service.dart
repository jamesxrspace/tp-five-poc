import 'package:tpfive/utils/api.dart';
import 'package:tpfive/utils/logger.dart';
import 'package:tpfive_game_server_api_client/api.dart';

class GameServerService {
  static const String TAG = 'GameServerService';

  bool isLoginSuccess = false;
  int lastLoginTime = 0;

  Future<void> login() {
    if (isLoginSuccess) {
      return Future(() => true);
    }

    return _login();
  }

  Future<void> _login() async {
    var loginApi = LoginApi(Api.getApiClient());
    var res = await loginApi.postLogin();
    isLoginSuccess = Api.isSuccess(res?.statusCode);
    if (!isLoginSuccess) {
      Log.e(TAG, 'Login failed: ${res?.errCode}');
      throw Exception();
    }
  }

  Future<Profile> getProfile() async {
    var loginApi = LoginApi(Api.getApiClient());
    var res = await loginApi.getUserProfile();
    if (!Api.isSuccess(res?.statusCode)) {
      Log.i(TAG, 'GetUserProfile not success');
      throw Exception();
    }

    Log.d(TAG, res!.data!.toString());
    Log.d(TAG, 'GetUserProfile success');
    return res.data!;
  }
}
