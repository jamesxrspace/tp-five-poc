//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class FeedApi {
  FeedApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Get news feed in lobby
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
  /// * [List<CategoriesEnum>] categories:
  ///   categories of feed
  Future<Response> getNewsFeedWithHttpInfo(int size, { int? offset, List<CategoriesEnum>? categories, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/feed/lobby';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    if (offset != null) {
      queryParams.addAll(_queryParams('', 'offset', offset));
    }
      queryParams.addAll(_queryParams('', 'size', size));
    if (categories != null) {
      queryParams.addAll(_queryParams('multi', 'categories', categories));
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

  /// Get news feed in lobby
  ///
  /// Parameters:
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [int] offset:
  ///   offset of items
  ///
  /// * [List<CategoriesEnum>] categories:
  ///   categories of feed
  Future<GetFeedListResponse?> getNewsFeed(int size, { int? offset, List<CategoriesEnum>? categories, }) async {
    final response = await getNewsFeedWithHttpInfo(size,  offset: offset, categories: categories, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'GetFeedListResponse',) as GetFeedListResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }
}
