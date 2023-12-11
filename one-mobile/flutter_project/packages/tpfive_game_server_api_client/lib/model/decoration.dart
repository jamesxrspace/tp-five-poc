//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class Decoration {
  /// Returns a new [Decoration] instance.
  Decoration({
    this.id,
    this.titleI18n,
    this.thumbnailUrl,
    this.tags = const [],
    this.bundleId,
    this.decorationKey,
    this.categoryId = const [],
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? id;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? titleI18n;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? thumbnailUrl;

  List<String> tags;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? bundleId;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? decorationKey;

  List<String> categoryId;

  @override
  bool operator ==(Object other) => identical(this, other) || other is Decoration &&
     other.id == id &&
     other.titleI18n == titleI18n &&
     other.thumbnailUrl == thumbnailUrl &&
     other.tags == tags &&
     other.bundleId == bundleId &&
     other.decorationKey == decorationKey &&
     other.categoryId == categoryId;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (id == null ? 0 : id!.hashCode) +
    (titleI18n == null ? 0 : titleI18n!.hashCode) +
    (thumbnailUrl == null ? 0 : thumbnailUrl!.hashCode) +
    (tags.hashCode) +
    (bundleId == null ? 0 : bundleId!.hashCode) +
    (decorationKey == null ? 0 : decorationKey!.hashCode) +
    (categoryId.hashCode);

  @override
  String toString() => 'Decoration[id=$id, titleI18n=$titleI18n, thumbnailUrl=$thumbnailUrl, tags=$tags, bundleId=$bundleId, decorationKey=$decorationKey, categoryId=$categoryId]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.id != null) {
      json[r'id'] = this.id;
    } else {
      json[r'id'] = null;
    }
    if (this.titleI18n != null) {
      json[r'title_i18n'] = this.titleI18n;
    } else {
      json[r'title_i18n'] = null;
    }
    if (this.thumbnailUrl != null) {
      json[r'thumbnail_url'] = this.thumbnailUrl;
    } else {
      json[r'thumbnail_url'] = null;
    }
      json[r'tags'] = this.tags;
    if (this.bundleId != null) {
      json[r'bundle_id'] = this.bundleId;
    } else {
      json[r'bundle_id'] = null;
    }
    if (this.decorationKey != null) {
      json[r'decoration_key'] = this.decorationKey;
    } else {
      json[r'decoration_key'] = null;
    }
      json[r'category_id'] = this.categoryId;
    return json;
  }

  /// Returns a new [Decoration] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static Decoration? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "Decoration[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "Decoration[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return Decoration(
        id: mapValueOfType<String>(json, r'id'),
        titleI18n: mapValueOfType<String>(json, r'title_i18n'),
        thumbnailUrl: mapValueOfType<String>(json, r'thumbnail_url'),
        tags: json[r'tags'] is List
            ? (json[r'tags'] as List).cast<String>()
            : const [],
        bundleId: mapValueOfType<String>(json, r'bundle_id'),
        decorationKey: mapValueOfType<String>(json, r'decoration_key'),
        categoryId: json[r'category_id'] is List
            ? (json[r'category_id'] as List).cast<String>()
            : const [],
      );
    }
    return null;
  }

  static List<Decoration> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <Decoration>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = Decoration.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, Decoration> mapFromJson(dynamic json) {
    final map = <String, Decoration>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = Decoration.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of Decoration-objects as value to a dart map
  static Map<String, List<Decoration>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<Decoration>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = Decoration.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

