//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class Asset {
  /// Returns a new [Asset] instance.
  Asset({
    this.id,
    this.tags = const [],
    this.type,
    this.categories = const [],
    this.files = const [],
    this.createdAt,
    this.updatedAt,
  });

  /// asset id
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? id;

  /// asset tags
  List<String> tags;

  /// asset type
  AssetTypeEnum? type;

  /// asset category
  List<CategoriesEnum> categories;

  /// asset files information
  List<UploadFile> files;

  /// asset created time
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? createdAt;

  /// asset updated time
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? updatedAt;

  @override
  bool operator ==(Object other) => identical(this, other) || other is Asset &&
     other.id == id &&
     other.tags == tags &&
     other.type == type &&
     other.categories == categories &&
     other.files == files &&
     other.createdAt == createdAt &&
     other.updatedAt == updatedAt;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (id == null ? 0 : id!.hashCode) +
    (tags.hashCode) +
    (type == null ? 0 : type!.hashCode) +
    (categories.hashCode) +
    (files.hashCode) +
    (createdAt == null ? 0 : createdAt!.hashCode) +
    (updatedAt == null ? 0 : updatedAt!.hashCode);

  @override
  String toString() => 'Asset[id=$id, tags=$tags, type=$type, categories=$categories, files=$files, createdAt=$createdAt, updatedAt=$updatedAt]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.id != null) {
      json[r'id'] = this.id;
    } else {
      json[r'id'] = null;
    }
      json[r'tags'] = this.tags;
    if (this.type != null) {
      json[r'type'] = this.type;
    } else {
      json[r'type'] = null;
    }
      json[r'categories'] = this.categories;
      json[r'files'] = this.files;
    if (this.createdAt != null) {
      json[r'created_at'] = this.createdAt!.toUtc().toIso8601String();
    } else {
      json[r'created_at'] = null;
    }
    if (this.updatedAt != null) {
      json[r'updated_at'] = this.updatedAt!.toUtc().toIso8601String();
    } else {
      json[r'updated_at'] = null;
    }
    return json;
  }

  /// Returns a new [Asset] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static Asset? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "Asset[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "Asset[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return Asset(
        id: mapValueOfType<String>(json, r'id'),
        tags: json[r'tags'] is List
            ? (json[r'tags'] as List).cast<String>()
            : const [],
        type: AssetTypeEnum.fromJson(json[r'type']),
        categories: CategoriesEnum.listFromJson(json[r'categories']),
        files: UploadFile.listFromJson(json[r'files']),
        createdAt: mapDateTime(json, r'created_at', ''),
        updatedAt: mapDateTime(json, r'updated_at', ''),
      );
    }
    return null;
  }

  static List<Asset> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <Asset>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = Asset.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, Asset> mapFromJson(dynamic json) {
    final map = <String, Asset>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = Asset.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of Asset-objects as value to a dart map
  static Map<String, List<Asset>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<Asset>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = Asset.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

/// asset type
class AssetTypeEnum {
  /// Instantiate a new enum with the provided [value].
  const AssetTypeEnum._(this.value);

  /// The underlying value of this enum member.
  final String value;

  @override
  String toString() => value;

  String toJson() => value;

  static const decoration = AssetTypeEnum._(r'decoration');

  /// List of all possible values in this [enum][AssetTypeEnum].
  static const values = <AssetTypeEnum>[
    decoration,
  ];

  static AssetTypeEnum? fromJson(dynamic value) => AssetTypeEnumTypeTransformer().decode(value);

  static List<AssetTypeEnum> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <AssetTypeEnum>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = AssetTypeEnum.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [AssetTypeEnum] to String,
/// and [decode] dynamic data back to [AssetTypeEnum].
class AssetTypeEnumTypeTransformer {
  factory AssetTypeEnumTypeTransformer() => _instance ??= const AssetTypeEnumTypeTransformer._();

  const AssetTypeEnumTypeTransformer._();

  String encode(AssetTypeEnum data) => data.value;

  /// Decodes a [dynamic value][data] to a AssetTypeEnum.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  AssetTypeEnum? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case r'decoration': return AssetTypeEnum.decoration;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [AssetTypeEnumTypeTransformer] instance.
  static AssetTypeEnumTypeTransformer? _instance;
}


