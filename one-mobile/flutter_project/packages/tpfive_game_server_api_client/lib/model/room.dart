//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class Room {
  /// Returns a new [Room] instance.
  Room({
    required this.roomId,
    required this.spaceId,
    this.users = const {},
  });

  String roomId;

  String spaceId;

  Map<String, RoomUser> users;

  @override
  bool operator ==(Object other) => identical(this, other) || other is Room &&
     other.roomId == roomId &&
     other.spaceId == spaceId &&
     other.users == users;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (roomId.hashCode) +
    (spaceId.hashCode) +
    (users.hashCode);

  @override
  String toString() => 'Room[roomId=$roomId, spaceId=$spaceId, users=$users]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
      json[r'room_id'] = this.roomId;
      json[r'space_id'] = this.spaceId;
      json[r'users'] = this.users;
    return json;
  }

  /// Returns a new [Room] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static Room? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "Room[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "Room[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return Room(
        roomId: mapValueOfType<String>(json, r'room_id')!,
        spaceId: mapValueOfType<String>(json, r'space_id')!,
        users: RoomUser.mapFromJson(json[r'users']),
      );
    }
    return null;
  }

  static List<Room> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <Room>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = Room.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, Room> mapFromJson(dynamic json) {
    final map = <String, Room>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = Room.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of Room-objects as value to a dart map
  static Map<String, List<Room>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<Room>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = Room.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'room_id',
    'space_id',
  };
}

