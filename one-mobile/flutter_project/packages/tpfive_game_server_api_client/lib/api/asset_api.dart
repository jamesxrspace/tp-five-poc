//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class AssetApi {
  AssetApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Confirm files uploaded to s3
  ///
  /// Confirm files uploaded to s3.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [String] requestId (required):
  ///   request id
  Future<Response> confirmUploadedWithHttpInfo(String requestId,) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/asset/uploaded/{request_id}'
      .replaceAll('{request_id}', requestId);

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

  /// Confirm files uploaded to s3
  ///
  /// Confirm files uploaded to s3.
  ///
  /// Parameters:
  ///
  /// * [String] requestId (required):
  ///   request id
  Future<ConfirmUploadedResponse?> confirmUploaded(String requestId,) async {
    final response = await confirmUploadedWithHttpInfo(requestId,);
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'ConfirmUploadedResponse',) as ConfirmUploadedResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Create upload request
  ///
  /// Create upload request.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [CreateUploadRequest] createUploadRequest:
  Future<Response> createUploadRequestWithHttpInfo({ CreateUploadRequest? createUploadRequest, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/asset/upload';

    // ignore: prefer_final_locals
    Object? postBody = createUploadRequest;

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

  /// Create upload request
  ///
  /// Create upload request.
  ///
  /// Parameters:
  ///
  /// * [CreateUploadRequest] createUploadRequest:
  Future<CreateUploadRequestResponse?> createUploadRequest({ CreateUploadRequest? createUploadRequest, }) async {
    final response = await createUploadRequestWithHttpInfo( createUploadRequest: createUploadRequest, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'CreateUploadRequestResponse',) as CreateUploadRequestResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }

  /// Get asset items by type
  ///
  /// Get asset items by type. This API endpoint allows you to retrieve a list of asset items based on their type. The response includes pagination information and a list of asset items.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [String] type (required):
  ///   asset type
  ///
  /// * [int] offset:
  ///   offset of items
  Future<Response> getAssetItemsWithHttpInfo(int size, String type, { int? offset, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/asset/{type}/list'
      .replaceAll('{type}', type);

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

  /// Get asset items by type
  ///
  /// Get asset items by type. This API endpoint allows you to retrieve a list of asset items based on their type. The response includes pagination information and a list of asset items.
  ///
  /// Parameters:
  ///
  /// * [int] size (required):
  ///   size of queries times
  ///
  /// * [String] type (required):
  ///   asset type
  ///
  /// * [int] offset:
  ///   offset of items
  Future<AssetListResponse?> getAssetItems(int size, String type, { int? offset, }) async {
    final response = await getAssetItemsWithHttpInfo(size, type,  offset: offset, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'AssetListResponse',) as AssetListResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }
}
