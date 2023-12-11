//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class DecorationApi {
  DecorationApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Get decoration categories
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [int] offset:
  ///   offset of items
  Future<Response> getDecorationCategoryListWithHttpInfo(int size, { int? offset, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/asset/decoration/category';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

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

  /// Get decoration categories
  ///
  /// Parameters:
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [int] offset:
  ///   offset of items
  Future<GetDecorationCategoryListResponse?> getDecorationCategoryList(int size, { int? offset, }) async {
    final response = await getDecorationCategoryListWithHttpInfo(size,  offset: offset, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'GetDecorationCategoryListResponse',) as GetDecorationCategoryListResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Get decoration items
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [int] offset:
  ///   offset of items
  ///
  /// * [String] cate:
  ///   filter by category
  Future<Response> getDecorationItemsWithHttpInfo(int size, { int? offset, String? cate, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/asset/decoration/list';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    if (offset != null) {
      queryParams.addAll(_queryParams('', 'offset', offset));
    }
      queryParams.addAll(_queryParams('', 'size', size));
    if (cate != null) {
      queryParams.addAll(_queryParams('', 'cate', cate));
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

  /// Get decoration items
  ///
  /// Parameters:
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [int] offset:
  ///   offset of items
  ///
  /// * [String] cate:
  ///   filter by category
  Future<GetDecorationListResponse?> getDecorationItems(int size, { int? offset, String? cate, }) async {
    final response = await getDecorationItemsWithHttpInfo(size,  offset: offset, cate: cate, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'GetDecorationListResponse',) as GetDecorationListResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }
}
