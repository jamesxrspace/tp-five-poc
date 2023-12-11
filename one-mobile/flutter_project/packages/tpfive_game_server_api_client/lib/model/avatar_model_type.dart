//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

/// Indicates the type of avatar model:   * `xr_v2` - XSpace Avatar V2 
class AvatarModelType {
  /// Instantiate a new enum with the provided [value].
  const AvatarModelType._(this.value);

  /// The underlying value of this enum member.
  final String value;

  @override
  String toString() => value;

  String toJson() => value;

  static const xrV2 = AvatarModelType._(r'xr_v2');

  /// List of all possible values in this [enum][AvatarModelType].
  static const values = <AvatarModelType>[
    xrV2,
  ];

  static AvatarModelType? fromJson(dynamic value) => AvatarModelTypeTypeTransformer().decode(value);

  static List<AvatarModelType> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <AvatarModelType>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = AvatarModelType.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [AvatarModelType] to String,
/// and [decode] dynamic data back to [AvatarModelType].
class AvatarModelTypeTypeTransformer {
  factory AvatarModelTypeTypeTransformer() => _instance ??= const AvatarModelTypeTypeTransformer._();

  const AvatarModelTypeTypeTransformer._();

  String encode(AvatarModelType data) => data.value;

  /// Decodes a [dynamic value][data] to a AvatarModelType.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  AvatarModelType? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case r'xr_v2': return AvatarModelType.xrV2;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [AvatarModelTypeTypeTransformer] instance.
  static AvatarModelTypeTypeTransformer? _instance;
}

