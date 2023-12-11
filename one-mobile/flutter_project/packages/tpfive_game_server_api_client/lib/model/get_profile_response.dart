//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class GetProfileResponse {
  int? statusCode;
  /// Returns a new [GetProfileResponse] instance.
  GetProfileResponse({
    this.errCode,
    this.message,
    this.data,
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

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  Profile? data;

  @override
  bool operator ==(Object other) => identical(this, other) || other is GetProfileResponse &&
     other.errCode == errCode &&
     other.message == message &&
     other.data == data;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (errCode == null ? 0 : errCode!.hashCode) +
    (message == null ? 0 : message!.hashCode) +
    (data == null ? 0 : data!.hashCode);

  @override
  String toString() => 'GetProfileResponse[errCode=$errCode, message=$message, data=$data]';

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
    if (this.data != null) {
      json[r'data'] = this.data;
    } else {
      json[r'data'] = null;
    }
    return json;
  }

  /// Returns a new [GetProfileResponse] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static GetProfileResponse? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "GetProfileResponse[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "GetProfileResponse[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return GetProfileResponse(
        errCode: mapValueOfType<int>(json, r'err_code'),
        message: mapValueOfType<String>(json, r'message'),
        data: Profile.fromJson(json[r'data']),
      );
    }
    return null;
  }

  static List<GetProfileResponse> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <GetProfileResponse>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = GetProfileResponse.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, GetProfileResponse> mapFromJson(dynamic json) {
    final map = <String, GetProfileResponse>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = GetProfileResponse.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of GetProfileResponse-objects as value to a dart map
  static Map<String, List<GetProfileResponse>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<GetProfileResponse>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = GetProfileResponse.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

