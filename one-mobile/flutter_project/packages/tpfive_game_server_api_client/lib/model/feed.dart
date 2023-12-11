//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class Feed {
  /// Returns a new [Feed] instance.
  Feed({
    this.id,
    this.type,
    this.ownerXrid,
    this.ownerNickname,
    this.categories = const [],
    this.updatedAt,
    this.content,
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? id;

  FeedTypeEnum? type;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? ownerXrid;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? ownerNickname;

  List<CategoriesEnum> categories;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? updatedAt;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  FeedContent? content;

  @override
  bool operator ==(Object other) => identical(this, other) || other is Feed &&
     other.id == id &&
     other.type == type &&
     other.ownerXrid == ownerXrid &&
     other.ownerNickname == ownerNickname &&
     other.categories == categories &&
     other.updatedAt == updatedAt &&
     other.content == content;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (id == null ? 0 : id!.hashCode) +
    (type == null ? 0 : type!.hashCode) +
    (ownerXrid == null ? 0 : ownerXrid!.hashCode) +
    (ownerNickname == null ? 0 : ownerNickname!.hashCode) +
    (categories.hashCode) +
    (updatedAt == null ? 0 : updatedAt!.hashCode) +
    (content == null ? 0 : content!.hashCode);

  @override
  String toString() => 'Feed[id=$id, type=$type, ownerXrid=$ownerXrid, ownerNickname=$ownerNickname, categories=$categories, updatedAt=$updatedAt, content=$content]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.id != null) {
      json[r'id'] = this.id;
    } else {
      json[r'id'] = null;
    }
    if (this.type != null) {
      json[r'type'] = this.type;
    } else {
      json[r'type'] = null;
    }
    if (this.ownerXrid != null) {
      json[r'owner_xrid'] = this.ownerXrid;
    } else {
      json[r'owner_xrid'] = null;
    }
    if (this.ownerNickname != null) {
      json[r'owner_nickname'] = this.ownerNickname;
    } else {
      json[r'owner_nickname'] = null;
    }
      json[r'categories'] = this.categories;
    if (this.updatedAt != null) {
      json[r'updated_at'] = this.updatedAt!.toUtc().toIso8601String();
    } else {
      json[r'updated_at'] = null;
    }
    if (this.content != null) {
      json[r'content'] = this.content;
    } else {
      json[r'content'] = null;
    }
    return json;
  }

  /// Returns a new [Feed] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static Feed? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "Feed[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "Feed[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return Feed(
        id: mapValueOfType<String>(json, r'id'),
        type: FeedTypeEnum.fromJson(json[r'type']),
        ownerXrid: mapValueOfType<String>(json, r'owner_xrid'),
        ownerNickname: mapValueOfType<String>(json, r'owner_nickname'),
        categories: CategoriesEnum.listFromJson(json[r'categories']),
        updatedAt: mapDateTime(json, r'updated_at', ''),
        content: FeedContent.fromJson(json[r'content']),
      );
    }
    return null;
  }

  static List<Feed> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <Feed>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = Feed.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, Feed> mapFromJson(dynamic json) {
    final map = <String, Feed>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = Feed.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of Feed-objects as value to a dart map
  static Map<String, List<Feed>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<Feed>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = Feed.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}


class FeedTypeEnum {
  /// Instantiate a new enum with the provided [value].
  const FeedTypeEnum._(this.value);

  /// The underlying value of this enum member.
  final String value;

  @override
  String toString() => value;

  String toJson() => value;

  static const avatarReel = FeedTypeEnum._(r'avatar_reel');
  static const space = FeedTypeEnum._(r'space');
  static const avatarText = FeedTypeEnum._(r'avatar_text');
  static const avatarImg = FeedTypeEnum._(r'avatar_img');
  static const avatar = FeedTypeEnum._(r'avatar');

  /// List of all possible values in this [enum][FeedTypeEnum].
  static const values = <FeedTypeEnum>[
    avatarReel,
    space,
    avatarText,
    avatarImg,
    avatar,
  ];

  static FeedTypeEnum? fromJson(dynamic value) => FeedTypeEnumTypeTransformer().decode(value);

  static List<FeedTypeEnum> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <FeedTypeEnum>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = FeedTypeEnum.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [FeedTypeEnum] to String,
/// and [decode] dynamic data back to [FeedTypeEnum].
class FeedTypeEnumTypeTransformer {
  factory FeedTypeEnumTypeTransformer() => _instance ??= const FeedTypeEnumTypeTransformer._();

  const FeedTypeEnumTypeTransformer._();

  String encode(FeedTypeEnum data) => data.value;

  /// Decodes a [dynamic value][data] to a FeedTypeEnum.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  FeedTypeEnum? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case r'avatar_reel': return FeedTypeEnum.avatarReel;
        case r'space': return FeedTypeEnum.space;
        case r'avatar_text': return FeedTypeEnum.avatarText;
        case r'avatar_img': return FeedTypeEnum.avatarImg;
        case r'avatar': return FeedTypeEnum.avatar;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [FeedTypeEnumTypeTransformer] instance.
  static FeedTypeEnumTypeTransformer? _instance;
}


