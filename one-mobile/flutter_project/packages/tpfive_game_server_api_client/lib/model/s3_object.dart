//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class S3Object {
  /// Returns a new [S3Object] instance.
  S3Object({
    this.url,
    this.path,
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? url;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? path;

  @override
  bool operator ==(Object other) => identical(this, other) || other is S3Object &&
     other.url == url &&
     other.path == path;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (url == null ? 0 : url!.hashCode) +
    (path == null ? 0 : path!.hashCode);

  @override
  String toString() => 'S3Object[url=$url, path=$path]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.url != null) {
      json[r'url'] = this.url;
    } else {
      json[r'url'] = null;
    }
    if (this.path != null) {
      json[r'path'] = this.path;
    } else {
      json[r'path'] = null;
    }
    return json;
  }

  /// Returns a new [S3Object] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static S3Object? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "S3Object[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "S3Object[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return S3Object(
        url: mapValueOfType<String>(json, r'url'),
        path: mapValueOfType<String>(json, r'path'),
      );
    }
    return null;
  }

  static List<S3Object> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <S3Object>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = S3Object.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, S3Object> mapFromJson(dynamic json) {
    final map = <String, S3Object>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = S3Object.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of S3Object-objects as value to a dart map
  static Map<String, List<S3Object>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<S3Object>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = S3Object.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

