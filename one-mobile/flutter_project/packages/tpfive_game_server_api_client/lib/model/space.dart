//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class Space {
  /// Returns a new [Space] instance.
  Space({
    this.spaceId,
    this.spaceGroupId,
    this.name,
    this.description,
    this.thumbnail,
    this.addressable,
    this.startAt,
    this.endAt,
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? spaceId;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? spaceGroupId;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? name;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? description;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? thumbnail;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? addressable;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? startAt;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? endAt;

  @override
  bool operator ==(Object other) => identical(this, other) || other is Space &&
     other.spaceId == spaceId &&
     other.spaceGroupId == spaceGroupId &&
     other.name == name &&
     other.description == description &&
     other.thumbnail == thumbnail &&
     other.addressable == addressable &&
     other.startAt == startAt &&
     other.endAt == endAt;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (spaceId == null ? 0 : spaceId!.hashCode) +
    (spaceGroupId == null ? 0 : spaceGroupId!.hashCode) +
    (name == null ? 0 : name!.hashCode) +
    (description == null ? 0 : description!.hashCode) +
    (thumbnail == null ? 0 : thumbnail!.hashCode) +
    (addressable == null ? 0 : addressable!.hashCode) +
    (startAt == null ? 0 : startAt!.hashCode) +
    (endAt == null ? 0 : endAt!.hashCode);

  @override
  String toString() => 'Space[spaceId=$spaceId, spaceGroupId=$spaceGroupId, name=$name, description=$description, thumbnail=$thumbnail, addressable=$addressable, startAt=$startAt, endAt=$endAt]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.spaceId != null) {
      json[r'space_id'] = this.spaceId;
    } else {
      json[r'space_id'] = null;
    }
    if (this.spaceGroupId != null) {
      json[r'space_group_id'] = this.spaceGroupId;
    } else {
      json[r'space_group_id'] = null;
    }
    if (this.name != null) {
      json[r'name'] = this.name;
    } else {
      json[r'name'] = null;
    }
    if (this.description != null) {
      json[r'description'] = this.description;
    } else {
      json[r'description'] = null;
    }
    if (this.thumbnail != null) {
      json[r'thumbnail'] = this.thumbnail;
    } else {
      json[r'thumbnail'] = null;
    }
    if (this.addressable != null) {
      json[r'addressable'] = this.addressable;
    } else {
      json[r'addressable'] = null;
    }
    if (this.startAt != null) {
      json[r'start_at'] = this.startAt!.toUtc().toIso8601String();
    } else {
      json[r'start_at'] = null;
    }
    if (this.endAt != null) {
      json[r'end_at'] = this.endAt!.toUtc().toIso8601String();
    } else {
      json[r'end_at'] = null;
    }
    return json;
  }

  /// Returns a new [Space] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static Space? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "Space[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "Space[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return Space(
        spaceId: mapValueOfType<String>(json, r'space_id'),
        spaceGroupId: mapValueOfType<String>(json, r'space_group_id'),
        name: mapValueOfType<String>(json, r'name'),
        description: mapValueOfType<String>(json, r'description'),
        thumbnail: mapValueOfType<String>(json, r'thumbnail'),
        addressable: mapValueOfType<String>(json, r'addressable'),
        startAt: mapDateTime(json, r'start_at', ''),
        endAt: mapDateTime(json, r'end_at', ''),
      );
    }
    return null;
  }

  static List<Space> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <Space>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = Space.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, Space> mapFromJson(dynamic json) {
    final map = <String, Space>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = Space.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of Space-objects as value to a dart map
  static Map<String, List<Space>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<Space>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = Space.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

