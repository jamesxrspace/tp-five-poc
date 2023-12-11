//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class AvatarThumbnail {
  /// Returns a new [AvatarThumbnail] instance.
  AvatarThumbnail({
    this.fullBody,
    this.head,
    this.upperBody,
  });

  /// full bady thumbnail url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? fullBody;

  /// head thumbnail url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? head;

  /// upper body thumbnail url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? upperBody;

  @override
  bool operator ==(Object other) => identical(this, other) || other is AvatarThumbnail &&
     other.fullBody == fullBody &&
     other.head == head &&
     other.upperBody == upperBody;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (fullBody == null ? 0 : fullBody!.hashCode) +
    (head == null ? 0 : head!.hashCode) +
    (upperBody == null ? 0 : upperBody!.hashCode);

  @override
  String toString() => 'AvatarThumbnail[fullBody=$fullBody, head=$head, upperBody=$upperBody]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.fullBody != null) {
      json[r'full_body'] = this.fullBody;
    } else {
      json[r'full_body'] = null;
    }
    if (this.head != null) {
      json[r'head'] = this.head;
    } else {
      json[r'head'] = null;
    }
    if (this.upperBody != null) {
      json[r'upper_body'] = this.upperBody;
    } else {
      json[r'upper_body'] = null;
    }
    return json;
  }

  /// Returns a new [AvatarThumbnail] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static AvatarThumbnail? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "AvatarThumbnail[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "AvatarThumbnail[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return AvatarThumbnail(
        fullBody: mapValueOfType<String>(json, r'full_body'),
        head: mapValueOfType<String>(json, r'head'),
        upperBody: mapValueOfType<String>(json, r'upper_body'),
      );
    }
    return null;
  }

  static List<AvatarThumbnail> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <AvatarThumbnail>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = AvatarThumbnail.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, AvatarThumbnail> mapFromJson(dynamic json) {
    final map = <String, AvatarThumbnail>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = AvatarThumbnail.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of AvatarThumbnail-objects as value to a dart map
  static Map<String, List<AvatarThumbnail>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<AvatarThumbnail>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = AvatarThumbnail.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

