//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class AgoraStreamingTokenPayload {
  /// Returns a new [AgoraStreamingTokenPayload] instance.
  AgoraStreamingTokenPayload({
    required this.channelId,
    required this.role,
    this.expiresIn,
  });

  /// name of the streaming channel
  String channelId;

  /// role of streaming
  AgoraStreamingTokenPayloadRoleEnum role;

  /// expire time for token, default is 600 seconds
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  int? expiresIn;

  @override
  bool operator ==(Object other) => identical(this, other) || other is AgoraStreamingTokenPayload &&
     other.channelId == channelId &&
     other.role == role &&
     other.expiresIn == expiresIn;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (channelId.hashCode) +
    (role.hashCode) +
    (expiresIn == null ? 0 : expiresIn!.hashCode);

  @override
  String toString() => 'AgoraStreamingTokenPayload[channelId=$channelId, role=$role, expiresIn=$expiresIn]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
      json[r'channel_id'] = this.channelId;
      json[r'role'] = this.role;
    if (this.expiresIn != null) {
      json[r'expires_in'] = this.expiresIn;
    } else {
      json[r'expires_in'] = null;
    }
    return json;
  }

  /// Returns a new [AgoraStreamingTokenPayload] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static AgoraStreamingTokenPayload? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "AgoraStreamingTokenPayload[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "AgoraStreamingTokenPayload[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return AgoraStreamingTokenPayload(
        channelId: mapValueOfType<String>(json, r'channel_id')!,
        role: AgoraStreamingTokenPayloadRoleEnum.fromJson(json[r'role'])!,
        expiresIn: mapValueOfType<int>(json, r'expires_in'),
      );
    }
    return null;
  }

  static List<AgoraStreamingTokenPayload> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <AgoraStreamingTokenPayload>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = AgoraStreamingTokenPayload.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, AgoraStreamingTokenPayload> mapFromJson(dynamic json) {
    final map = <String, AgoraStreamingTokenPayload>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = AgoraStreamingTokenPayload.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of AgoraStreamingTokenPayload-objects as value to a dart map
  static Map<String, List<AgoraStreamingTokenPayload>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<AgoraStreamingTokenPayload>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = AgoraStreamingTokenPayload.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'channel_id',
    'role',
  };
}

/// role of streaming
class AgoraStreamingTokenPayloadRoleEnum {
  /// Instantiate a new enum with the provided [value].
  const AgoraStreamingTokenPayloadRoleEnum._(this.value);

  /// The underlying value of this enum member.
  final String value;

  @override
  String toString() => value;

  String toJson() => value;

  static const publisher = AgoraStreamingTokenPayloadRoleEnum._(r'publisher');
  static const subscriber = AgoraStreamingTokenPayloadRoleEnum._(r'subscriber');

  /// List of all possible values in this [enum][AgoraStreamingTokenPayloadRoleEnum].
  static const values = <AgoraStreamingTokenPayloadRoleEnum>[
    publisher,
    subscriber,
  ];

  static AgoraStreamingTokenPayloadRoleEnum? fromJson(dynamic value) => AgoraStreamingTokenPayloadRoleEnumTypeTransformer().decode(value);

  static List<AgoraStreamingTokenPayloadRoleEnum> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <AgoraStreamingTokenPayloadRoleEnum>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = AgoraStreamingTokenPayloadRoleEnum.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [AgoraStreamingTokenPayloadRoleEnum] to String,
/// and [decode] dynamic data back to [AgoraStreamingTokenPayloadRoleEnum].
class AgoraStreamingTokenPayloadRoleEnumTypeTransformer {
  factory AgoraStreamingTokenPayloadRoleEnumTypeTransformer() => _instance ??= const AgoraStreamingTokenPayloadRoleEnumTypeTransformer._();

  const AgoraStreamingTokenPayloadRoleEnumTypeTransformer._();

  String encode(AgoraStreamingTokenPayloadRoleEnum data) => data.value;

  /// Decodes a [dynamic value][data] to a AgoraStreamingTokenPayloadRoleEnum.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  AgoraStreamingTokenPayloadRoleEnum? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case r'publisher': return AgoraStreamingTokenPayloadRoleEnum.publisher;
        case r'subscriber': return AgoraStreamingTokenPayloadRoleEnum.subscriber;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [AgoraStreamingTokenPayloadRoleEnumTypeTransformer] instance.
  static AgoraStreamingTokenPayloadRoleEnumTypeTransformer? _instance;
}


