//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class AvatarApi {
  AvatarApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Activate avatar
  ///
  /// Activate avatar by avatar id
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] avatarId (required):
  ///   avatar id
  Future<Response> activateAvatarWithHttpInfo(String avatarId,) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/avatar/activate/{avatar_id}'
      .replaceAll('{avatar_id}', avatarId);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];


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

  /// Activate avatar
  ///
  /// Activate avatar by avatar id
  ///
  /// Parameters:
  ///
  /// * [String] avatarId (required):
  ///   avatar id
  Future<AvatarActivateResponse?> activateAvatar(String avatarId,) async {
    final response = await activateAvatarWithHttpInfo(avatarId,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'AvatarActivateResponse',) as AvatarActivateResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Get current avatar detail list
  ///
  /// Get current avatar detail list by xrids
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [List<String>] xrids (required):
  ///   Get the xrids of which users avatar detail
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [int] offset:
  ///   offset of items
  Future<Response> getCurrentAvatarMetadataListWithHttpInfo(List<String> xrids, int size, { int? offset, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/avatar/current/list';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

      queryParams.addAll(_queryParams('csv', 'xrids', xrids));
    if (offset != null) {
      queryParams.addAll(_queryParams('', 'offset', offset));
    }
      queryParams.addAll(_queryParams('', 'size', size));

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'GET',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Get current avatar detail list
  ///
  /// Get current avatar detail list by xrids
  ///
  /// Parameters:
  ///
  /// * [List<String>] xrids (required):
  ///   Get the xrids of which users avatar detail
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [int] offset:
  ///   offset of items
  Future<AvatarMetadataListResponse?> getCurrentAvatarMetadataList(List<String> xrids, int size, { int? offset, }) async {
    final response = await getCurrentAvatarMetadataListWithHttpInfo(xrids, size,  offset: offset, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'AvatarMetadataListResponse',) as AvatarMetadataListResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Get myself current avatar detail
  ///
  /// Get myself current avatar detail
  ///
  /// Note: This method returns the HTTP [Response].
  Future<Response> getMyselfCurrentAvatarMetadataWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/avatar/current';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'GET',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Get myself current avatar detail
  ///
  /// Get myself current avatar detail
  Future<AvatarMetadataResponse?> getMyselfCurrentAvatarMetadata() async {
    final response = await getMyselfCurrentAvatarMetadataWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'AvatarMetadataResponse',) as AvatarMetadataResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Save avatar
  ///
  /// Save edited avatar.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [AvatarModelType] type (required):
  ///
  /// * [Object] avatarFormat:
  ///   avatar format
  ///
  /// * [MultipartFile] avatarAsset:
  ///   avatar asset
  ///
  /// * [MultipartFile] avatarHead:
  ///   avatar head
  ///
  /// * [MultipartFile] avatarUpperBody:
  ///   avatar upper body
  ///
  /// * [MultipartFile] avatarFullBody:
  ///   avatar full body
  ///
  /// * [String] avatarId:
  ///   avatar id
  Future<Response> saveAvatarWithHttpInfo(AvatarModelType type, { Object? avatarFormat, MultipartFile? avatarAsset, MultipartFile? avatarHead, MultipartFile? avatarUpperBody, MultipartFile? avatarFullBody, String? avatarId, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/avatar/save';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>['multipart/form-data'];

    bool hasFields = false;
    final mp = MultipartRequest('POST', Uri.parse(path));
    if (type != null) {
      hasFields = true;
      mp.fields[r'type'] = parameterToString(type);
    }
    if (avatarFormat != null) {
      hasFields = true;
      mp.fields[r'avatar_format'] = parameterToString(avatarFormat);
    }
    if (avatarAsset != null) {
      hasFields = true;
      mp.fields[r'avatar_asset'] = avatarAsset.field;
      mp.files.add(avatarAsset);
    }
    if (avatarHead != null) {
      hasFields = true;
      mp.fields[r'avatar_head'] = avatarHead.field;
      mp.files.add(avatarHead);
    }
    if (avatarUpperBody != null) {
      hasFields = true;
      mp.fields[r'avatar_upper_body'] = avatarUpperBody.field;
      mp.files.add(avatarUpperBody);
    }
    if (avatarFullBody != null) {
      hasFields = true;
      mp.fields[r'avatar_full_body'] = avatarFullBody.field;
      mp.files.add(avatarFullBody);
    }
    if (avatarId != null) {
      hasFields = true;
      mp.fields[r'avatar_id'] = parameterToString(avatarId);
    }
    if (hasFields) {
      postBody = mp;
    }

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

  /// Save avatar
  ///
  /// Save edited avatar.
  ///
  /// Parameters:
  ///
  /// * [AvatarModelType] type (required):
  ///
  /// * [Object] avatarFormat:
  ///   avatar format
  ///
  /// * [MultipartFile] avatarAsset:
  ///   avatar asset
  ///
  /// * [MultipartFile] avatarHead:
  ///   avatar head
  ///
  /// * [MultipartFile] avatarUpperBody:
  ///   avatar upper body
  ///
  /// * [MultipartFile] avatarFullBody:
  ///   avatar full body
  ///
  /// * [String] avatarId:
  ///   avatar id
  Future<AvatarMetadataResponse?> saveAvatar(AvatarModelType type, { Object? avatarFormat, MultipartFile? avatarAsset, MultipartFile? avatarHead, MultipartFile? avatarUpperBody, MultipartFile? avatarFullBody, String? avatarId, }) async {
    final response = await saveAvatarWithHttpInfo(type,  avatarFormat: avatarFormat, avatarAsset: avatarAsset, avatarHead: avatarHead, avatarUpperBody: avatarUpperBody, avatarFullBody: avatarFullBody, avatarId: avatarId, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'AvatarMetadataResponse',) as AvatarMetadataResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }
}
