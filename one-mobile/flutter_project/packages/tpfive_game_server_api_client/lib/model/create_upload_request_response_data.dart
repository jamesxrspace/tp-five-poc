//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class CreateUploadRequestResponseData {
  /// Returns a new [CreateUploadRequestResponseData] instance.
  CreateUploadRequestResponseData({
    this.requestId,
    this.presignedUrls = const {},
  });

  /// upload request id
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? requestId;

  /// s3 presigned urls for uploading files
  Map<String, String> presignedUrls;

  @override
  bool operator ==(Object other) => identical(this, other) || other is CreateUploadRequestResponseData &&
     other.requestId == requestId &&
     other.presignedUrls == presignedUrls;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (requestId == null ? 0 : requestId!.hashCode) +
    (presignedUrls.hashCode);

  @override
  String toString() => 'CreateUploadRequestResponseData[requestId=$requestId, presignedUrls=$presignedUrls]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.requestId != null) {
      json[r'request_id'] = this.requestId;
    } else {
      json[r'request_id'] = null;
    }
      json[r'presigned_urls'] = this.presignedUrls;
    return json;
  }

  /// Returns a new [CreateUploadRequestResponseData] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static CreateUploadRequestResponseData? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "CreateUploadRequestResponseData[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "CreateUploadRequestResponseData[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return CreateUploadRequestResponseData(
        requestId: mapValueOfType<String>(json, r'request_id'),
        presignedUrls: mapCastOfType<String, String>(json, r'presigned_urls') ?? const {},
      );
    }
    return null;
  }

  static List<CreateUploadRequestResponseData> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <CreateUploadRequestResponseData>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = CreateUploadRequestResponseData.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, CreateUploadRequestResponseData> mapFromJson(dynamic json) {
    final map = <String, CreateUploadRequestResponseData>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = CreateUploadRequestResponseData.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of CreateUploadRequestResponseData-objects as value to a dart map
  static Map<String, List<CreateUploadRequestResponseData>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<CreateUploadRequestResponseData>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = CreateUploadRequestResponseData.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

