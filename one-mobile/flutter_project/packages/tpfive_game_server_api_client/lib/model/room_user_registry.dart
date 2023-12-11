//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class RoomUserRegistry {
  /// Returns a new [RoomUserRegistry] instance.
  RoomUserRegistry({
    required this.spaceId,
    required this.roomId,
    required this.userId,
  });

  /// id of the space by which the room is defined.
  String spaceId;

  /// id of the Fusion session to which the room belongs
  String roomId;

  /// id of the user
  String userId;

  @override
  bool operator ==(Object other) => identical(this, other) || other is RoomUserRegistry &&
     other.spaceId == spaceId &&
     other.roomId == roomId &&
     other.userId == userId;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (spaceId.hashCode) +
    (roomId.hashCode) +
    (userId.hashCode);

  @override
  String toString() => 'RoomUserRegistry[spaceId=$spaceId, roomId=$roomId, userId=$userId]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
      json[r'space_id'] = this.spaceId;
      json[r'room_id'] = this.roomId;
      json[r'user_id'] = this.userId;
    return json;
  }

  /// Returns a new [RoomUserRegistry] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static RoomUserRegistry? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "RoomUserRegistry[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "RoomUserRegistry[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return RoomUserRegistry(
        spaceId: mapValueOfType<String>(json, r'space_id')!,
        roomId: mapValueOfType<String>(json, r'room_id')!,
        userId: mapValueOfType<String>(json, r'user_id')!,
      );
    }
    return null;
  }

  static List<RoomUserRegistry> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <RoomUserRegistry>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = RoomUserRegistry.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, RoomUserRegistry> mapFromJson(dynamic json) {
    final map = <String, RoomUserRegistry>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = RoomUserRegistry.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of RoomUserRegistry-objects as value to a dart map
  static Map<String, List<RoomUserRegistry>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<RoomUserRegistry>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = RoomUserRegistry.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'space_id',
    'room_id',
    'user_id',
  };
}

