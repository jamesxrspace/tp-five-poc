//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class GetSpaceGroupListResponseAllOfData {
  /// Returns a new [GetSpaceGroupListResponseAllOfData] instance.
  GetSpaceGroupListResponseAllOfData({
    this.total,
    this.items = const [],
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  int? total;

  List<SpaceGroup> items;

  @override
  bool operator ==(Object other) => identical(this, other) || other is GetSpaceGroupListResponseAllOfData &&
     other.total == total &&
     other.items == items;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (total == null ? 0 : total!.hashCode) +
    (items.hashCode);

  @override
  String toString() => 'GetSpaceGroupListResponseAllOfData[total=$total, items=$items]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.total != null) {
      json[r'total'] = this.total;
    } else {
      json[r'total'] = null;
    }
      json[r'items'] = this.items;
    return json;
  }

  /// Returns a new [GetSpaceGroupListResponseAllOfData] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static GetSpaceGroupListResponseAllOfData? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "GetSpaceGroupListResponseAllOfData[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "GetSpaceGroupListResponseAllOfData[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return GetSpaceGroupListResponseAllOfData(
        total: mapValueOfType<int>(json, r'total'),
        items: SpaceGroup.listFromJson(json[r'items']),
      );
    }
    return null;
  }

  static List<GetSpaceGroupListResponseAllOfData> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <GetSpaceGroupListResponseAllOfData>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = GetSpaceGroupListResponseAllOfData.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, GetSpaceGroupListResponseAllOfData> mapFromJson(dynamic json) {
    final map = <String, GetSpaceGroupListResponseAllOfData>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = GetSpaceGroupListResponseAllOfData.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of GetSpaceGroupListResponseAllOfData-objects as value to a dart map
  static Map<String, List<GetSpaceGroupListResponseAllOfData>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<GetSpaceGroupListResponseAllOfData>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = GetSpaceGroupListResponseAllOfData.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

