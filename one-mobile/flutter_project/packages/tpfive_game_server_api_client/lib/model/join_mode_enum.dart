//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

/// join modes of reel
class JoinModeEnum {
  /// Instantiate a new enum with the provided [value].
  const JoinModeEnum._(this.value);

  /// The underlying value of this enum member.
  final String value;

  @override
  String toString() => value;

  String toJson() => value;

  static const off = JoinModeEnum._(r'off');
  static const all = JoinModeEnum._(r'all');
  static const friendsFollowers = JoinModeEnum._(r'friends_followers');
  static const me = JoinModeEnum._(r'me');

  /// List of all possible values in this [enum][JoinModeEnum].
  static const values = <JoinModeEnum>[
    off,
    all,
    friendsFollowers,
    me,
  ];

  static JoinModeEnum? fromJson(dynamic value) => JoinModeEnumTypeTransformer().decode(value);

  static List<JoinModeEnum> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <JoinModeEnum>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = JoinModeEnum.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [JoinModeEnum] to String,
/// and [decode] dynamic data back to [JoinModeEnum].
class JoinModeEnumTypeTransformer {
  factory JoinModeEnumTypeTransformer() => _instance ??= const JoinModeEnumTypeTransformer._();

  const JoinModeEnumTypeTransformer._();

  String encode(JoinModeEnum data) => data.value;

  /// Decodes a [dynamic value][data] to a JoinModeEnum.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  JoinModeEnum? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case r'off': return JoinModeEnum.off;
        case r'all': return JoinModeEnum.all;
        case r'friends_followers': return JoinModeEnum.friendsFollowers;
        case r'me': return JoinModeEnum.me;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [JoinModeEnumTypeTransformer] instance.
  static JoinModeEnumTypeTransformer? _instance;
}

