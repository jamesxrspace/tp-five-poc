//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class CreateReelRequest {
  /// Returns a new [CreateReelRequest] instance.
  CreateReelRequest({
    required this.thumbnail,
    required this.video,
    required this.xrs,
    this.categories = const [],
    required this.joinMode,
    this.description,
    this.musicToMotionUrl,
    this.parentReelId,
  });

  /// reel thumbnail url
  String thumbnail;

  /// video url
  String video;

  /// xrs url
  String xrs;

  /// categories of reel belonging feed
  List<CategoriesEnum> categories;

  JoinModeEnum joinMode;

  /// reel description
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? description;

  /// music to motion url
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? musicToMotionUrl;

  /// parent reel id
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? parentReelId;

  @override
  bool operator ==(Object other) => identical(this, other) || other is CreateReelRequest &&
     other.thumbnail == thumbnail &&
     other.video == video &&
     other.xrs == xrs &&
     other.categories == categories &&
     other.joinMode == joinMode &&
     other.description == description &&
     other.musicToMotionUrl == musicToMotionUrl &&
     other.parentReelId == parentReelId;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (thumbnail.hashCode) +
    (video.hashCode) +
    (xrs.hashCode) +
    (categories.hashCode) +
    (joinMode.hashCode) +
    (description == null ? 0 : description!.hashCode) +
    (musicToMotionUrl == null ? 0 : musicToMotionUrl!.hashCode) +
    (parentReelId == null ? 0 : parentReelId!.hashCode);

  @override
  String toString() => 'CreateReelRequest[thumbnail=$thumbnail, video=$video, xrs=$xrs, categories=$categories, joinMode=$joinMode, description=$description, musicToMotionUrl=$musicToMotionUrl, parentReelId=$parentReelId]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
      json[r'thumbnail'] = this.thumbnail;
      json[r'video'] = this.video;
      json[r'xrs'] = this.xrs;
      json[r'categories'] = this.categories;
      json[r'join_mode'] = this.joinMode;
    if (this.description != null) {
      json[r'description'] = this.description;
    } else {
      json[r'description'] = null;
    }
    if (this.musicToMotionUrl != null) {
      json[r'music_to_motion_url'] = this.musicToMotionUrl;
    } else {
      json[r'music_to_motion_url'] = null;
    }
    if (this.parentReelId != null) {
      json[r'parent_reel_id'] = this.parentReelId;
    } else {
      json[r'parent_reel_id'] = null;
    }
    return json;
  }

  /// Returns a new [CreateReelRequest] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static CreateReelRequest? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "CreateReelRequest[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "CreateReelRequest[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return CreateReelRequest(
        thumbnail: mapValueOfType<String>(json, r'thumbnail')!,
        video: mapValueOfType<String>(json, r'video')!,
        xrs: mapValueOfType<String>(json, r'xrs')!,
        categories: CategoriesEnum.listFromJson(json[r'categories']),
        joinMode: JoinModeEnum.fromJson(json[r'join_mode'])!,
        description: mapValueOfType<String>(json, r'description'),
        musicToMotionUrl: mapValueOfType<String>(json, r'music_to_motion_url'),
        parentReelId: mapValueOfType<String>(json, r'parent_reel_id'),
      );
    }
    return null;
  }

  static List<CreateReelRequest> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <CreateReelRequest>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = CreateReelRequest.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, CreateReelRequest> mapFromJson(dynamic json) {
    final map = <String, CreateReelRequest>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = CreateReelRequest.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of CreateReelRequest-objects as value to a dart map
  static Map<String, List<CreateReelRequest>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<CreateReelRequest>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = CreateReelRequest.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'thumbnail',
    'video',
    'xrs',
    'categories',
    'join_mode',
  };
}

