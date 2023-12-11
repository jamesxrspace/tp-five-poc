//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class ReelApi {
  ReelApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Create reel
  ///
  /// Create reel.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [CreateReelRequest] createReelRequest:
  Future<Response> createReelWithHttpInfo({ CreateReelRequest? createReelRequest, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/reel/create';

    // ignore: prefer_final_locals
    Object? postBody = createReelRequest;

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

  /// Create reel
  ///
  /// Create reel.
  ///
  /// Parameters:
  ///
  /// * [CreateReelRequest] createReelRequest:
  Future<CreateReelResponse?> createReel({ CreateReelRequest? createReelRequest, }) async {
    final response = await createReelWithHttpInfo( createReelRequest: createReelRequest, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'CreateReelResponse',) as CreateReelResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Delete one reel
  ///
  /// Delete one Reel.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] reelId (required):
  ///   reel id
  Future<Response> deleteReelWithHttpInfo(String reelId,) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/reel/delete/{reel_id}'
      .replaceAll('{reel_id}', reelId);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];


    return apiClient.invokeAPI(
      path,
      'DELETE',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Delete one reel
  ///
  /// Delete one Reel.
  ///
  /// Parameters:
  ///
  /// * [String] reelId (required):
  ///   reel id
  Future<BaseResponse?> deleteReel(String reelId,) async {
    final response = await deleteReelWithHttpInfo(reelId,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'BaseResponse',) as BaseResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// List reels
  ///
  /// List reels.
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
  /// * [String] xrid:
  ///   xrid
  ///
  /// * [String] reelId:
  ///   reel_id
  ///
  /// * [String] status:
  ///   status
  Future<Response> listReelsWithHttpInfo(int size, { int? offset, String? xrid, String? reelId, String? status, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/reel/list';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    if (offset != null) {
      queryParams.addAll(_queryParams('', 'offset', offset));
    }
      queryParams.addAll(_queryParams('', 'size', size));
    if (xrid != null) {
      queryParams.addAll(_queryParams('', 'xrid', xrid));
    }
    if (reelId != null) {
      queryParams.addAll(_queryParams('', 'reel_id', reelId));
    }
    if (status != null) {
      queryParams.addAll(_queryParams('', 'status', status));
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

  /// List reels
  ///
  /// List reels.
  ///
  /// Parameters:
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [int] offset:
  ///   offset of items
  ///
  /// * [String] xrid:
  ///   xrid
  ///
  /// * [String] reelId:
  ///   reel_id
  ///
  /// * [String] status:
  ///   status
  Future<ListReelResponse?> listReels(int size, { int? offset, String? xrid, String? reelId, String? status, }) async {
    final response = await listReelsWithHttpInfo(size,  offset: offset, xrid: xrid, reelId: reelId, status: status, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'ListReelResponse',) as ListReelResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Publish reel
  ///
  /// Publish reel.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] reelId (required):
  ///   reel_id to publish
  Future<Response> publishReelWithHttpInfo(String reelId,) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/reel/publish/{reel_id}'
      .replaceAll('{reel_id}', reelId);

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

  /// Publish reel
  ///
  /// Publish reel.
  ///
  /// Parameters:
  ///
  /// * [String] reelId (required):
  ///   reel_id to publish
  Future<BaseResponse?> publishReel(String reelId,) async {
    final response = await publishReelWithHttpInfo(reelId,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'BaseResponse',) as BaseResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }
}
