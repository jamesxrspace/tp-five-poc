//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class AvatarMetadataListResponseData {
  /// Returns a new [AvatarMetadataListResponseData] instance.
  AvatarMetadataListResponseData({
    this.items = const [],
  });

  /// avatar detail list
  List<AvatarMetadata> items;

  @override
  bool operator ==(Object other) => identical(this, other) || other is AvatarMetadataListResponseData &&
     other.items == items;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (items.hashCode);

  @override
  String toString() => 'AvatarMetadataListResponseData[items=$items]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
      json[r'items'] = this.items;
    return json;
  }

  /// Returns a new [AvatarMetadataListResponseData] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static AvatarMetadataListResponseData? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "AvatarMetadataListResponseData[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "AvatarMetadataListResponseData[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return AvatarMetadataListResponseData(
        items: AvatarMetadata.listFromJson(json[r'items']),
      );
    }
    return null;
  }

  static List<AvatarMetadataListResponseData> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <AvatarMetadataListResponseData>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = AvatarMetadataListResponseData.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, AvatarMetadataListResponseData> mapFromJson(dynamic json) {
    final map = <String, AvatarMetadataListResponseData>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = AvatarMetadataListResponseData.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of AvatarMetadataListResponseData-objects as value to a dart map
  static Map<String, List<AvatarMetadataListResponseData>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<AvatarMetadataListResponseData>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = AvatarMetadataListResponseData.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

