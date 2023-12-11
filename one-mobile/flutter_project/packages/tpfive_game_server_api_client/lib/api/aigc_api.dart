//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;


class AigcApi {
  AigcApi([ApiClient? apiClient]) : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Generate motion from uploaded music file.
  ///
  /// Note: This method returns the HTTP [Response].
  ///
  /// Parameters:
  ///
  /// * [GenerateMotionRequest] generateMotionRequest:
  Future<Response> generateMotionWithHttpInfo({ GenerateMotionRequest? generateMotionRequest, }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/v1/aigc/motion/generate';

    // ignore: prefer_final_locals
    Object? postBody = generateMotionRequest;

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

  /// Generate motion from uploaded music file.
  ///
  /// Parameters:
  ///
  /// * [GenerateMotionRequest] generateMotionRequest:
  Future<GenerateMotionResponse?> generateMotion({ GenerateMotionRequest? generateMotionRequest, }) async {
    final response = await generateMotionWithHttpInfo( generateMotionRequest: generateMotionRequest, );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty && response.statusCode != HttpStatus.noContent) {
      var result = await apiClient.deserializeAsync(await _decodeBodyBytes(response), 'GenerateMotionResponse',) as GenerateMotionResponse;
      result.statusCode = response.statusCode;
      return result;
    
    }
    return null;
  }
}
