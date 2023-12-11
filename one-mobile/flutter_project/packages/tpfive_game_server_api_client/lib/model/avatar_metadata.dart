//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class AvatarMetadata {
  /// Returns a new [AvatarMetadata] instance.
  AvatarMetadata({
    this.appId,
    this.author,
    this.avatarFormat,
    this.avatarId,
    this.avatarUrl,
    this.createdAt,
    this.thumbnail,
    this.type,
    this.updatedAt,
    this.xrid,
  });

  /// create from which app
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? appId;

  /// author of the avatar
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? author;

  /// xrspace avatar format. this is a json string should be parsed using AvatarFormat.Deserialize
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  Object? avatarFormat;

  /// avatar id
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? avatarId;

  /// avatar url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? avatarUrl;

  /// avatar created time
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? createdAt;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  AvatarThumbnail? thumbnail;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  AvatarModelType? type;

  /// avatar updated time
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? updatedAt;

  /// xrspace user id
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? xrid;

  @override
  bool operator ==(Object other) => identical(this, other) || other is AvatarMetadata &&
     other.appId == appId &&
     other.author == author &&
     other.avatarFormat == avatarFormat &&
     other.avatarId == avatarId &&
     other.avatarUrl == avatarUrl &&
     other.createdAt == createdAt &&
     other.thumbnail == thumbnail &&
     other.type == type &&
     other.updatedAt == updatedAt &&
     other.xrid == xrid;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (appId == null ? 0 : appId!.hashCode) +
    (author == null ? 0 : author!.hashCode) +
    (avatarFormat == null ? 0 : avatarFormat!.hashCode) +
    (avatarId == null ? 0 : avatarId!.hashCode) +
    (avatarUrl == null ? 0 : avatarUrl!.hashCode) +
    (createdAt == null ? 0 : createdAt!.hashCode) +
    (thumbnail == null ? 0 : thumbnail!.hashCode) +
    (type == null ? 0 : type!.hashCode) +
    (updatedAt == null ? 0 : updatedAt!.hashCode) +
    (xrid == null ? 0 : xrid!.hashCode);

  @override
  String toString() => 'AvatarMetadata[appId=$appId, author=$author, avatarFormat=$avatarFormat, avatarId=$avatarId, avatarUrl=$avatarUrl, createdAt=$createdAt, thumbnail=$thumbnail, type=$type, updatedAt=$updatedAt, xrid=$xrid]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.appId != null) {
      json[r'app_id'] = this.appId;
    } else {
      json[r'app_id'] = null;
    }
    if (this.author != null) {
      json[r'author'] = this.author;
    } else {
      json[r'author'] = null;
    }
    if (this.avatarFormat != null) {
      json[r'avatar_format'] = this.avatarFormat;
    } else {
      json[r'avatar_format'] = null;
    }
    if (this.avatarId != null) {
      json[r'avatar_id'] = this.avatarId;
    } else {
      json[r'avatar_id'] = null;
    }
    if (this.avatarUrl != null) {
      json[r'avatar_url'] = this.avatarUrl;
    } else {
      json[r'avatar_url'] = null;
    }
    if (this.createdAt != null) {
      json[r'created_at'] = this.createdAt!.toUtc().toIso8601String();
    } else {
      json[r'created_at'] = null;
    }
    if (this.thumbnail != null) {
      json[r'thumbnail'] = this.thumbnail;
    } else {
      json[r'thumbnail'] = null;
    }
    if (this.type != null) {
      json[r'type'] = this.type;
    } else {
      json[r'type'] = null;
    }
    if (this.updatedAt != null) {
      json[r'updated_at'] = this.updatedAt!.toUtc().toIso8601String();
    } else {
      json[r'updated_at'] = null;
    }
    if (this.xrid != null) {
      json[r'xrid'] = this.xrid;
    } else {
      json[r'xrid'] = null;
    }
    return json;
  }

  /// Returns a new [AvatarMetadata] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static AvatarMetadata? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "AvatarMetadata[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "AvatarMetadata[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return AvatarMetadata(
        appId: mapValueOfType<String>(json, r'app_id'),
        author: mapValueOfType<String>(json, r'author'),
        avatarFormat: mapValueOfType<Object>(json, r'avatar_format'),
        avatarId: mapValueOfType<String>(json, r'avatar_id'),
        avatarUrl: mapValueOfType<String>(json, r'avatar_url'),
        createdAt: mapDateTime(json, r'created_at', ''),
        thumbnail: AvatarThumbnail.fromJson(json[r'thumbnail']),
        type: AvatarModelType.fromJson(json[r'type']),
        updatedAt: mapDateTime(json, r'updated_at', ''),
        xrid: mapValueOfType<String>(json, r'xrid'),
      );
    }
    return null;
  }

  static List<AvatarMetadata> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <AvatarMetadata>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = AvatarMetadata.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, AvatarMetadata> mapFromJson(dynamic json) {
    final map = <String, AvatarMetadata>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = AvatarMetadata.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of AvatarMetadata-objects as value to a dart map
  static Map<String, List<AvatarMetadata>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<AvatarMetadata>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = AvatarMetadata.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

