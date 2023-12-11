//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class RoomApi {
  RoomApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Register one user of a specified room with GameServer.
  ///
  /// Register one user of a specified room with GameServer.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [RoomUserRegistry] roomUserRegistry (required):
  Future<Response> registerRoomUserWithHttpInfo(RoomUserRegistry roomUserRegistry,) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/room/join';

    // ignore: prefer_final_locals
    Object? postBody = roomUserRegistry;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>['application/json'];


    return apiClient.invokeAPI(
      path,
      'POST',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Register one user of a specified room with GameServer.
  ///
  /// Register one user of a specified room with GameServer.
  ///
  /// Parameters:
  ///
  /// * [RoomUserRegistry] roomUserRegistry (required):
  Future<RegisterRoomUserResponse?> registerRoomUser(RoomUserRegistry roomUserRegistry,) async {
    final response = await registerRoomUserWithHttpInfo(roomUserRegistry,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'RegisterRoomUserResponse',) as RegisterRoomUserResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Unregister one user of a specified room with GameServer.
  ///
  /// Unregister one user of a specified room with GameServer.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [RoomUserRegistry] roomUserRegistry (required):
  Future<Response> unregisterRoomUserWithHttpInfo(RoomUserRegistry roomUserRegistry,) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/room/leave';

    // ignore: prefer_final_locals
    Object? postBody = roomUserRegistry;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>['application/json'];


    return apiClient.invokeAPI(
      path,
      'POST',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Unregister one user of a specified room with GameServer.
  ///
  /// Unregister one user of a specified room with GameServer.
  ///
  /// Parameters:
  ///
  /// * [RoomUserRegistry] roomUserRegistry (required):
  Future<UnregisterRoomUserResponse?> unregisterRoomUser(RoomUserRegistry roomUserRegistry,) async {
    final response = await unregisterRoomUserWithHttpInfo(roomUserRegistry,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'UnregisterRoomUserResponse',) as UnregisterRoomUserResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }
}
