//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class ConfirmUploadedResponse {
  int? statusCode;
  /// Returns a new [ConfirmUploadedResponse] instance.
  ConfirmUploadedResponse({
    this.errCode,
    this.message,
    this.data = const {},
  });

  /// error code from server
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  int? errCode;

  /// error message from server
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? message;

  Map<String, S3Object> data;

  @override
  bool operator ==(Object other) => identical(this, other) || other is ConfirmUploadedResponse &&
     other.errCode == errCode &&
     other.message == message &&
     other.data == data;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (errCode == null ? 0 : errCode!.hashCode) +
    (message == null ? 0 : message!.hashCode) +
    (data.hashCode);

  @override
  String toString() => 'ConfirmUploadedResponse[errCode=$errCode, message=$message, data=$data]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.errCode != null) {
      json[r'err_code'] = this.errCode;
    } else {
      json[r'err_code'] = null;
    }
    if (this.message != null) {
      json[r'message'] = this.message;
    } else {
      json[r'message'] = null;
    }
      json[r'data'] = this.data;
    return json;
  }

  /// Returns a new [ConfirmUploadedResponse] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static ConfirmUploadedResponse? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "ConfirmUploadedResponse[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "ConfirmUploadedResponse[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return ConfirmUploadedResponse(
        errCode: mapValueOfType<int>(json, r'err_code'),
        message: mapValueOfType<String>(json, r'message'),
        data: S3Object.mapFromJson(json[r'data']),
      );
    }
    return null;
  }

  static List<ConfirmUploadedResponse> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <ConfirmUploadedResponse>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = ConfirmUploadedResponse.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, ConfirmUploadedResponse> mapFromJson(dynamic json) {
    final map = <String, ConfirmUploadedResponse>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = ConfirmUploadedResponse.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of ConfirmUploadedResponse-objects as value to a dart map
  static Map<String, List<ConfirmUploadedResponse>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<ConfirmUploadedResponse>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = ConfirmUploadedResponse.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

