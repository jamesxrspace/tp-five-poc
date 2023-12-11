//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class SpaceGroup {
  /// Returns a new [SpaceGroup] instance.
  SpaceGroup({
    this.spaceGroupId,
    this.name,
    this.description,
    this.thumbnail,
    this.status,
    this.startAt,
    this.endAt,
    this.spaces = const [],
  });

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

  SpaceGroupStatusEnum? status;

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

  List<SpaceGroupSpacesInner> spaces;

  @override
  bool operator ==(Object other) => identical(this, other) || other is SpaceGroup &&
     other.spaceGroupId == spaceGroupId &&
     other.name == name &&
     other.description == description &&
     other.thumbnail == thumbnail &&
     other.status == status &&
     other.startAt == startAt &&
     other.endAt == endAt &&
     other.spaces == spaces;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (spaceGroupId == null ? 0 : spaceGroupId!.hashCode) +
    (name == null ? 0 : name!.hashCode) +
    (description == null ? 0 : description!.hashCode) +
    (thumbnail == null ? 0 : thumbnail!.hashCode) +
    (status == null ? 0 : status!.hashCode) +
    (startAt == null ? 0 : startAt!.hashCode) +
    (endAt == null ? 0 : endAt!.hashCode) +
    (spaces.hashCode);

  @override
  String toString() => 'SpaceGroup[spaceGroupId=$spaceGroupId, name=$name, description=$description, thumbnail=$thumbnail, status=$status, startAt=$startAt, endAt=$endAt, spaces=$spaces]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
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
    if (this.status != null) {
      json[r'status'] = this.status;
    } else {
      json[r'status'] = null;
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
      json[r'spaces'] = this.spaces;
    return json;
  }

  /// Returns a new [SpaceGroup] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static SpaceGroup? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "SpaceGroup[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "SpaceGroup[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return SpaceGroup(
        spaceGroupId: mapValueOfType<String>(json, r'space_group_id'),
        name: mapValueOfType<String>(json, r'name'),
        description: mapValueOfType<String>(json, r'description'),
        thumbnail: mapValueOfType<String>(json, r'thumbnail'),
        status: SpaceGroupStatusEnum.fromJson(json[r'status']),
        startAt: mapDateTime(json, r'start_at', ''),
        endAt: mapDateTime(json, r'end_at', ''),
        spaces: SpaceGroupSpacesInner.listFromJson(json[r'spaces']),
      );
    }
    return null;
  }

  static List<SpaceGroup> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <SpaceGroup>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = SpaceGroup.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, SpaceGroup> mapFromJson(dynamic json) {
    final map = <String, SpaceGroup>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = SpaceGroup.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of SpaceGroup-objects as value to a dart map
  static Map<String, List<SpaceGroup>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<SpaceGroup>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = SpaceGroup.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}


class SpaceGroupStatusEnum {
  /// Instantiate a new enum with the provided [value].
  const SpaceGroupStatusEnum._(this.value);

  /// The underlying value of this enum member.
  final String value;

  @override
  String toString() => value;

  String toJson() => value;

  static const enabled = SpaceGroupStatusEnum._(r'enabled');
  static const disabled = SpaceGroupStatusEnum._(r'disabled');

  /// List of all possible values in this [enum][SpaceGroupStatusEnum].
  static const values = <SpaceGroupStatusEnum>[
    enabled,
    disabled,
  ];

  static SpaceGroupStatusEnum? fromJson(dynamic value) => SpaceGroupStatusEnumTypeTransformer().decode(value);

  static List<SpaceGroupStatusEnum> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <SpaceGroupStatusEnum>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = SpaceGroupStatusEnum.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [SpaceGroupStatusEnum] to String,
/// and [decode] dynamic data back to [SpaceGroupStatusEnum].
class SpaceGroupStatusEnumTypeTransformer {
  factory SpaceGroupStatusEnumTypeTransformer() => _instance ??= const SpaceGroupStatusEnumTypeTransformer._();

  const SpaceGroupStatusEnumTypeTransformer._();

  String encode(SpaceGroupStatusEnum data) => data.value;

  /// Decodes a [dynamic value][data] to a SpaceGroupStatusEnum.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  SpaceGroupStatusEnum? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case r'enabled': return SpaceGroupStatusEnum.enabled;
        case r'disabled': return SpaceGroupStatusEnum.disabled;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [SpaceGroupStatusEnumTypeTransformer] instance.
  static SpaceGroupStatusEnumTypeTransformer? _instance;
}


