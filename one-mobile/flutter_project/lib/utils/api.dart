import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:get_it/get_it.dart';
import 'package:tpfive/services/account_service.dart';
import 'package:tpfive_game_server_api_client/api.dart';

class Api {
  static late ApiClient apiClient;

  static void initApi() {
    apiClient = ApiClient(basePath: dotenv.env['SERVER_DOMAIN']!);
    apiClient.addDefaultHeader('Accept', 'application/json');
  }

  static ApiClient getApiClient() {
    _addAuthorization(apiClient);
    return apiClient;
  }

  static void _addAuthorization(ApiClient apiClient) {
    var accountService = GetIt.I<AccountService>();
    var token = accountService.getAccessToken();
    if (apiClient.defaultHeaderMap.containsKey('Authorization')) {
      apiClient.defaultHeaderMap.remove('Authorization');
    }
    apiClient.addDefaultHeader('Authorization', 'Bearer $token');
  }

  static bool isSuccess(int? code) {
    if (code != null) {
      return (code > 199) && (code < 300);
    }

    return false;
  }
}
