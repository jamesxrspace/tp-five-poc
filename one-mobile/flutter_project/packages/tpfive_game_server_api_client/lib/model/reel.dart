//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class Reel {
  /// Returns a new [Reel] instance.
  Reel({
    this.id,
    this.xrid,
    this.description,
    this.thumbnail,
    this.video,
    this.xrs,
    this.musicToMotionUrl,
    this.status,
    this.joinMode,
    this.parentReelIds = const [],
    this.rootReelId,
    this.createdAt,
    this.updatedAt,
  });

  /// reel id
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? id;

  /// xrid
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? xrid;

  /// reel description
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? description;

  /// reel thumbnail url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? thumbnail;

  /// video url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? video;

  /// xrs url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? xrs;

  /// music to motion url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? musicToMotionUrl;

  /// reel status
  ReelStatusEnum? status;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  JoinModeEnum? joinMode;

  /// parent reel ids
  List<String> parentReelIds;

  /// root reel id
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? rootReelId;

  /// reel created time
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? createdAt;

  /// reel updated time
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? updatedAt;

  @override
  bool operator ==(Object other) => identical(this, other) || other is Reel &&
     other.id == id &&
     other.xrid == xrid &&
     other.description == description &&
     other.thumbnail == thumbnail &&
     other.video == video &&
     other.xrs == xrs &&
     other.musicToMotionUrl == musicToMotionUrl &&
     other.status == status &&
     other.joinMode == joinMode &&
     other.parentReelIds == parentReelIds &&
     other.rootReelId == rootReelId &&
     other.createdAt == createdAt &&
     other.updatedAt == updatedAt;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (id == null ? 0 : id!.hashCode) +
    (xrid == null ? 0 : xrid!.hashCode) +
    (description == null ? 0 : description!.hashCode) +
    (thumbnail == null ? 0 : thumbnail!.hashCode) +
    (video == null ? 0 : video!.hashCode) +
    (xrs == null ? 0 : xrs!.hashCode) +
    (musicToMotionUrl == null ? 0 : musicToMotionUrl!.hashCode) +
    (status == null ? 0 : status!.hashCode) +
    (joinMode == null ? 0 : joinMode!.hashCode) +
    (parentReelIds.hashCode) +
    (rootReelId == null ? 0 : rootReelId!.hashCode) +
    (createdAt == null ? 0 : createdAt!.hashCode) +
    (updatedAt == null ? 0 : updatedAt!.hashCode);

  @override
  String toString() => 'Reel[id=$id, xrid=$xrid, description=$description, thumbnail=$thumbnail, video=$video, xrs=$xrs, musicToMotionUrl=$musicToMotionUrl, status=$status, joinMode=$joinMode, parentReelIds=$parentReelIds, rootReelId=$rootReelId, createdAt=$createdAt, updatedAt=$updatedAt]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.id != null) {
      json[r'id'] = this.id;
    } else {
      json[r'id'] = null;
    }
    if (this.xrid != null) {
      json[r'xrid'] = this.xrid;
    } else {
      json[r'xrid'] = null;
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
    if (this.video != null) {
      json[r'video'] = this.video;
    } else {
      json[r'video'] = null;
    }
    if (this.xrs != null) {
      json[r'xrs'] = this.xrs;
    } else {
      json[r'xrs'] = null;
    }
    if (this.musicToMotionUrl != null) {
      json[r'music_to_motion_url'] = this.musicToMotionUrl;
    } else {
      json[r'music_to_motion_url'] = null;
    }
    if (this.status != null) {
      json[r'status'] = this.status;
    } else {
      json[r'status'] = null;
    }
    if (this.joinMode != null) {
      json[r'join_mode'] = this.joinMode;
    } else {
      json[r'join_mode'] = null;
    }
      json[r'parent_reel_ids'] = this.parentReelIds;
    if (this.rootReelId != null) {
      json[r'root_reel_id'] = this.rootReelId;
    } else {
      json[r'root_reel_id'] = null;
    }
    if (this.createdAt != null) {
      json[r'created_at'] = this.createdAt;
    } else {
      json[r'created_at'] = null;
    }
    if (this.updatedAt != null) {
      json[r'updated_at'] = this.updatedAt;
    } else {
      json[r'updated_at'] = null;
    }
    return json;
  }

  /// Returns a new [Reel] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static Reel? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "Reel[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "Reel[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return Reel(
        id: mapValueOfType<String>(json, r'id'),
        xrid: mapValueOfType<String>(json, r'xrid'),
        description: mapValueOfType<String>(json, r'description'),
        thumbnail: mapValueOfType<String>(json, r'thumbnail'),
        video: mapValueOfType<String>(json, r'video'),
        xrs: mapValueOfType<String>(json, r'xrs'),
        musicToMotionUrl: mapValueOfType<String>(json, r'music_to_motion_url'),
        status: ReelStatusEnum.fromJson(json[r'status']),
        joinMode: JoinModeEnum.fromJson(json[r'join_mode']),
        parentReelIds: json[r'parent_reel_ids'] is List
            ? (json[r'parent_reel_ids'] as List).cast<String>()
            : const [],
        rootReelId: mapValueOfType<String>(json, r'root_reel_id'),
        createdAt: mapValueOfType<String>(json, r'created_at'),
        updatedAt: mapValueOfType<String>(json, r'updated_at'),
      );
    }
    return null;
  }

  static List<Reel> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <Reel>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = Reel.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, Reel> mapFromJson(dynamic json) {
    final map = <String, Reel>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = Reel.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of Reel-objects as value to a dart map
  static Map<String, List<Reel>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<Reel>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = Reel.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

/// reel status
class ReelStatusEnum {
  /// Instantiate a new enum with the provided [value].
  const ReelStatusEnum._(this.value);

  /// The underlying value of this enum member.
  final String value;

  @override
  String toString() => value;

  String toJson() => value;

  static const published = ReelStatusEnum._(r'published');
  static const draft = ReelStatusEnum._(r'draft');
  static const deleted = ReelStatusEnum._(r'deleted');

  /// List of all possible values in this [enum][ReelStatusEnum].
  static const values = <ReelStatusEnum>[
    published,
    draft,
    deleted,
  ];

  static ReelStatusEnum? fromJson(dynamic value) => ReelStatusEnumTypeTransformer().decode(value);

  static List<ReelStatusEnum> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <ReelStatusEnum>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = ReelStatusEnum.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [ReelStatusEnum] to String,
/// and [decode] dynamic data back to [ReelStatusEnum].
class ReelStatusEnumTypeTransformer {
  factory ReelStatusEnumTypeTransformer() => _instance ??= const ReelStatusEnumTypeTransformer._();

  const ReelStatusEnumTypeTransformer._();

  String encode(ReelStatusEnum data) => data.value;

  /// Decodes a [dynamic value][data] to a ReelStatusEnum.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  ReelStatusEnum? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case r'published': return ReelStatusEnum.published;
        case r'draft': return ReelStatusEnum.draft;
        case r'deleted': return ReelStatusEnum.deleted;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [ReelStatusEnumTypeTransformer] instance.
  static ReelStatusEnumTypeTransformer? _instance;
}


