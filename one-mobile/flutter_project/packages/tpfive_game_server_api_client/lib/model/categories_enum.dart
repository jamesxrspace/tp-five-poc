//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

/// categories of feed and reel
class CategoriesEnum {
  /// Instantiate a new enum with the provided [value].
  const CategoriesEnum._(this.value);

  /// The underlying value of this enum member.
  final String value;

  @override
  String toString() => value;

  String toJson() => value;

  static const friends = CategoriesEnum._(r'friends');
  static const music = CategoriesEnum._(r'music');
  static const dance = CategoriesEnum._(r'dance');
  static const talkShow = CategoriesEnum._(r'talk_show');
  static const culture = CategoriesEnum._(r'culture');

  /// List of all possible values in this [enum][CategoriesEnum].
  static const values = <CategoriesEnum>[
    friends,
    music,
    dance,
    talkShow,
    culture,
  ];

  static CategoriesEnum? fromJson(dynamic value) => CategoriesEnumTypeTransformer().decode(value);

  static List<CategoriesEnum> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <CategoriesEnum>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = CategoriesEnum.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [CategoriesEnum] to String,
/// and [decode] dynamic data back to [CategoriesEnum].
class CategoriesEnumTypeTransformer {
  factory CategoriesEnumTypeTransformer() => _instance ??= const CategoriesEnumTypeTransformer._();

  const CategoriesEnumTypeTransformer._();

  String encode(CategoriesEnum data) => data.value;

  /// Decodes a [dynamic value][data] to a CategoriesEnum.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  CategoriesEnum? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case r'friends': return CategoriesEnum.friends;
        case r'music': return CategoriesEnum.music;
        case r'dance': return CategoriesEnum.dance;
        case r'talk_show': return CategoriesEnum.talkShow;
        case r'culture': return CategoriesEnum.culture;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [CategoriesEnumTypeTransformer] instance.
  static CategoriesEnumTypeTransformer? _instance;
}

