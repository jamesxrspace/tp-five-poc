//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class SpaceApi {
  SpaceApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Get the list of available space groups
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [int] offset (required):
  ///   current page
  ///
  /// * [int] size (required):
  ///   number of items per page
  Future<Response> getSpaceGroupListWithHttpInfo(int offset, int size,) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/space/group/list';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

      queryParams.addAll(_queryParams('', 'offset', offset));
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

  /// Get the list of available space groups
  ///
  /// Parameters:
  ///
  /// * [int] offset (required):
  ///   current page
  ///
  /// * [int] size (required):
  ///   number of items per page
  Future<GetSpaceGroupListResponse?> getSpaceGroupList(int offset, int size,) async {
    final response = await getSpaceGroupListWithHttpInfo(offset, size,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'GetSpaceGroupListResponse',) as GetSpaceGroupListResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Get the list of available space
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [int] offset (required):
  ///   current page (start from 0)
  ///
  /// * [int] size (required):
  ///   number of items per page
  ///
  /// * [String] spaceGroupId:
  ///   related space group id
  Future<Response> getSpaceListWithHttpInfo(int offset, int size, { String? spaceGroupId, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/space/list';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

      queryParams.addAll(_queryParams('', 'offset', offset));
      queryParams.addAll(_queryParams('', 'size', size));
    if (spaceGroupId != null) {
      queryParams.addAll(_queryParams('', 'space_group_id', spaceGroupId));
    }

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

  /// Get the list of available space
  ///
  /// Parameters:
  ///
  /// * [int] offset (required):
  ///   current page (start from 0)
  ///
  /// * [int] size (required):
  ///   number of items per page
  ///
  /// * [String] spaceGroupId:
  ///   related space group id
  Future<GetSpaceListResponse?> getSpaceList(int offset, int size, { String? spaceGroupId, }) async {
    final response = await getSpaceListWithHttpInfo(offset, size,  spaceGroupId: spaceGroupId, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'GetSpaceListResponse',) as GetSpaceListResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }
}
