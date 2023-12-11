import 'package:tpfive/utils/api.dart';
import 'package:tpfive_game_server_api_client/api.dart';

class SpaceService {
  static const String tag = 'SpaceService';
  static const numPerPage = 10;

  Future<List<SpaceGroup>> getAllSpaceGroups() async {
    List<SpaceGroup> result = [];
    var spaceApi = SpaceApi(Api.getApiClient());
    for (var page = 0;; page++) {
      var res = await spaceApi.getSpaceGroupList(page, numPerPage);
      if (!Api.isSuccess(res!.statusCode)) {
        return [];
      }

      result.addAll(res.data!.items);
      if (res.data!.total! <= (page + 1) * numPerPage) {
        break;
      }
    }

    return result;
  }

  Future<List<Space>> getAllSpaces() async {
    List<Space> result = [];
    var spaceApi = SpaceApi(Api.getApiClient());
    for (var page = 0;; page++) {
      var res = await spaceApi.getSpaceList(page, numPerPage);
      if (!Api.isSuccess(res!.statusCode)) {
        return [];
      }

      result.addAll(res.data!.items);
      if (res.data!.total! <= (page + 1) * numPerPage) {
        break;
      }
    }

    return result;
  }
}
