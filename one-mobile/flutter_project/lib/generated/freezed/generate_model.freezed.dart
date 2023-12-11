// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'generate_model.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
    'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#custom-getters-and-methods');

FlutterMessage _$FlutterMessageFromJson(Map<String, dynamic> json) {
  return _FlutterMessage.fromJson(json);
}

/// @nodoc
mixin _$FlutterMessage {
  String get data => throw _privateConstructorUsedError;
  ErrorCode get errorCode => throw _privateConstructorUsedError;
  String get errorMsg => throw _privateConstructorUsedError;
  String get sessionId => throw _privateConstructorUsedError;
  FlutterMessageType get type => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $FlutterMessageCopyWith<FlutterMessage> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $FlutterMessageCopyWith<$Res> {
  factory $FlutterMessageCopyWith(
          FlutterMessage value, $Res Function(FlutterMessage) then) =
      _$FlutterMessageCopyWithImpl<$Res, FlutterMessage>;
  @useResult
  $Res call(
      {String data,
      ErrorCode errorCode,
      String errorMsg,
      String sessionId,
      FlutterMessageType type});
}

/// @nodoc
class _$FlutterMessageCopyWithImpl<$Res, $Val extends FlutterMessage>
    implements $FlutterMessageCopyWith<$Res> {
  _$FlutterMessageCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? data = null,
    Object? errorCode = null,
    Object? errorMsg = null,
    Object? sessionId = null,
    Object? type = null,
  }) {
    return _then(_value.copyWith(
      data: null == data
          ? _value.data
          : data // ignore: cast_nullable_to_non_nullable
              as String,
      errorCode: null == errorCode
          ? _value.errorCode
          : errorCode // ignore: cast_nullable_to_non_nullable
              as ErrorCode,
      errorMsg: null == errorMsg
          ? _value.errorMsg
          : errorMsg // ignore: cast_nullable_to_non_nullable
              as String,
      sessionId: null == sessionId
          ? _value.sessionId
          : sessionId // ignore: cast_nullable_to_non_nullable
              as String,
      type: null == type
          ? _value.type
          : type // ignore: cast_nullable_to_non_nullable
              as FlutterMessageType,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$FlutterMessageImplCopyWith<$Res>
    implements $FlutterMessageCopyWith<$Res> {
  factory _$$FlutterMessageImplCopyWith(_$FlutterMessageImpl value,
          $Res Function(_$FlutterMessageImpl) then) =
      __$$FlutterMessageImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {String data,
      ErrorCode errorCode,
      String errorMsg,
      String sessionId,
      FlutterMessageType type});
}

/// @nodoc
class __$$FlutterMessageImplCopyWithImpl<$Res>
    extends _$FlutterMessageCopyWithImpl<$Res, _$FlutterMessageImpl>
    implements _$$FlutterMessageImplCopyWith<$Res> {
  __$$FlutterMessageImplCopyWithImpl(
      _$FlutterMessageImpl _value, $Res Function(_$FlutterMessageImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? data = null,
    Object? errorCode = null,
    Object? errorMsg = null,
    Object? sessionId = null,
    Object? type = null,
  }) {
    return _then(_$FlutterMessageImpl(
      data: null == data
          ? _value.data
          : data // ignore: cast_nullable_to_non_nullable
              as String,
      errorCode: null == errorCode
          ? _value.errorCode
          : errorCode // ignore: cast_nullable_to_non_nullable
              as ErrorCode,
      errorMsg: null == errorMsg
          ? _value.errorMsg
          : errorMsg // ignore: cast_nullable_to_non_nullable
              as String,
      sessionId: null == sessionId
          ? _value.sessionId
          : sessionId // ignore: cast_nullable_to_non_nullable
              as String,
      type: null == type
          ? _value.type
          : type // ignore: cast_nullable_to_non_nullable
              as FlutterMessageType,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$FlutterMessageImpl implements _FlutterMessage {
  const _$FlutterMessageImpl(
      {required this.data,
      required this.errorCode,
      required this.errorMsg,
      required this.sessionId,
      required this.type});

  factory _$FlutterMessageImpl.fromJson(Map<String, dynamic> json) =>
      _$$FlutterMessageImplFromJson(json);

  @override
  final String data;
  @override
  final ErrorCode errorCode;
  @override
  final String errorMsg;
  @override
  final String sessionId;
  @override
  final FlutterMessageType type;

  @override
  String toString() {
    return 'FlutterMessage(data: $data, errorCode: $errorCode, errorMsg: $errorMsg, sessionId: $sessionId, type: $type)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$FlutterMessageImpl &&
            (identical(other.data, data) || other.data == data) &&
            (identical(other.errorCode, errorCode) ||
                other.errorCode == errorCode) &&
            (identical(other.errorMsg, errorMsg) ||
                other.errorMsg == errorMsg) &&
            (identical(other.sessionId, sessionId) ||
                other.sessionId == sessionId) &&
            (identical(other.type, type) || other.type == type));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode =>
      Object.hash(runtimeType, data, errorCode, errorMsg, sessionId, type);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$FlutterMessageImplCopyWith<_$FlutterMessageImpl> get copyWith =>
      __$$FlutterMessageImplCopyWithImpl<_$FlutterMessageImpl>(
          this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$FlutterMessageImplToJson(
      this,
    );
  }
}

abstract class _FlutterMessage implements FlutterMessage {
  const factory _FlutterMessage(
      {required final String data,
      required final ErrorCode errorCode,
      required final String errorMsg,
      required final String sessionId,
      required final FlutterMessageType type}) = _$FlutterMessageImpl;

  factory _FlutterMessage.fromJson(Map<String, dynamic> json) =
      _$FlutterMessageImpl.fromJson;

  @override
  String get data;
  @override
  ErrorCode get errorCode;
  @override
  String get errorMsg;
  @override
  String get sessionId;
  @override
  FlutterMessageType get type;
  @override
  @JsonKey(ignore: true)
  _$$FlutterMessageImplCopyWith<_$FlutterMessageImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

MusicData _$MusicDataFromJson(Map<String, dynamic> json) {
  return _MusicData.fromJson(json);
}

/// @nodoc
mixin _$MusicData {
  String get aigcPath => throw _privateConstructorUsedError;
  String get singerName => throw _privateConstructorUsedError;
  String get songName => throw _privateConstructorUsedError;
  String get songPath => throw _privateConstructorUsedError;
  String get tag => throw _privateConstructorUsedError;
  String get thumbnailPath => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $MusicDataCopyWith<MusicData> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $MusicDataCopyWith<$Res> {
  factory $MusicDataCopyWith(MusicData value, $Res Function(MusicData) then) =
      _$MusicDataCopyWithImpl<$Res, MusicData>;
  @useResult
  $Res call(
      {String aigcPath,
      String singerName,
      String songName,
      String songPath,
      String tag,
      String thumbnailPath});
}

/// @nodoc
class _$MusicDataCopyWithImpl<$Res, $Val extends MusicData>
    implements $MusicDataCopyWith<$Res> {
  _$MusicDataCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? aigcPath = null,
    Object? singerName = null,
    Object? songName = null,
    Object? songPath = null,
    Object? tag = null,
    Object? thumbnailPath = null,
  }) {
    return _then(_value.copyWith(
      aigcPath: null == aigcPath
          ? _value.aigcPath
          : aigcPath // ignore: cast_nullable_to_non_nullable
              as String,
      singerName: null == singerName
          ? _value.singerName
          : singerName // ignore: cast_nullable_to_non_nullable
              as String,
      songName: null == songName
          ? _value.songName
          : songName // ignore: cast_nullable_to_non_nullable
              as String,
      songPath: null == songPath
          ? _value.songPath
          : songPath // ignore: cast_nullable_to_non_nullable
              as String,
      tag: null == tag
          ? _value.tag
          : tag // ignore: cast_nullable_to_non_nullable
              as String,
      thumbnailPath: null == thumbnailPath
          ? _value.thumbnailPath
          : thumbnailPath // ignore: cast_nullable_to_non_nullable
              as String,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$MusicDataImplCopyWith<$Res>
    implements $MusicDataCopyWith<$Res> {
  factory _$$MusicDataImplCopyWith(
          _$MusicDataImpl value, $Res Function(_$MusicDataImpl) then) =
      __$$MusicDataImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {String aigcPath,
      String singerName,
      String songName,
      String songPath,
      String tag,
      String thumbnailPath});
}

/// @nodoc
class __$$MusicDataImplCopyWithImpl<$Res>
    extends _$MusicDataCopyWithImpl<$Res, _$MusicDataImpl>
    implements _$$MusicDataImplCopyWith<$Res> {
  __$$MusicDataImplCopyWithImpl(
      _$MusicDataImpl _value, $Res Function(_$MusicDataImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? aigcPath = null,
    Object? singerName = null,
    Object? songName = null,
    Object? songPath = null,
    Object? tag = null,
    Object? thumbnailPath = null,
  }) {
    return _then(_$MusicDataImpl(
      aigcPath: null == aigcPath
          ? _value.aigcPath
          : aigcPath // ignore: cast_nullable_to_non_nullable
              as String,
      singerName: null == singerName
          ? _value.singerName
          : singerName // ignore: cast_nullable_to_non_nullable
              as String,
      songName: null == songName
          ? _value.songName
          : songName // ignore: cast_nullable_to_non_nullable
              as String,
      songPath: null == songPath
          ? _value.songPath
          : songPath // ignore: cast_nullable_to_non_nullable
              as String,
      tag: null == tag
          ? _value.tag
          : tag // ignore: cast_nullable_to_non_nullable
              as String,
      thumbnailPath: null == thumbnailPath
          ? _value.thumbnailPath
          : thumbnailPath // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$MusicDataImpl implements _MusicData {
  const _$MusicDataImpl(
      {required this.aigcPath,
      required this.singerName,
      required this.songName,
      required this.songPath,
      required this.tag,
      required this.thumbnailPath});

  factory _$MusicDataImpl.fromJson(Map<String, dynamic> json) =>
      _$$MusicDataImplFromJson(json);

  @override
  final String aigcPath;
  @override
  final String singerName;
  @override
  final String songName;
  @override
  final String songPath;
  @override
  final String tag;
  @override
  final String thumbnailPath;

  @override
  String toString() {
    return 'MusicData(aigcPath: $aigcPath, singerName: $singerName, songName: $songName, songPath: $songPath, tag: $tag, thumbnailPath: $thumbnailPath)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$MusicDataImpl &&
            (identical(other.aigcPath, aigcPath) ||
                other.aigcPath == aigcPath) &&
            (identical(other.singerName, singerName) ||
                other.singerName == singerName) &&
            (identical(other.songName, songName) ||
                other.songName == songName) &&
            (identical(other.songPath, songPath) ||
                other.songPath == songPath) &&
            (identical(other.tag, tag) || other.tag == tag) &&
            (identical(other.thumbnailPath, thumbnailPath) ||
                other.thumbnailPath == thumbnailPath));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, aigcPath, singerName, songName,
      songPath, tag, thumbnailPath);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$MusicDataImplCopyWith<_$MusicDataImpl> get copyWith =>
      __$$MusicDataImplCopyWithImpl<_$MusicDataImpl>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$MusicDataImplToJson(
      this,
    );
  }
}

abstract class _MusicData implements MusicData {
  const factory _MusicData(
      {required final String aigcPath,
      required final String singerName,
      required final String songName,
      required final String songPath,
      required final String tag,
      required final String thumbnailPath}) = _$MusicDataImpl;

  factory _MusicData.fromJson(Map<String, dynamic> json) =
      _$MusicDataImpl.fromJson;

  @override
  String get aigcPath;
  @override
  String get singerName;
  @override
  String get songName;
  @override
  String get songPath;
  @override
  String get tag;
  @override
  String get thumbnailPath;
  @override
  @JsonKey(ignore: true)
  _$$MusicDataImplCopyWith<_$MusicDataImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

PrefsData _$PrefsDataFromJson(Map<String, dynamic> json) {
  return _PrefsData.fromJson(json);
}

/// @nodoc
mixin _$PrefsData {
  Map<String, Prefs> get prefs => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $PrefsDataCopyWith<PrefsData> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $PrefsDataCopyWith<$Res> {
  factory $PrefsDataCopyWith(PrefsData value, $Res Function(PrefsData) then) =
      _$PrefsDataCopyWithImpl<$Res, PrefsData>;
  @useResult
  $Res call({Map<String, Prefs> prefs});
}

/// @nodoc
class _$PrefsDataCopyWithImpl<$Res, $Val extends PrefsData>
    implements $PrefsDataCopyWith<$Res> {
  _$PrefsDataCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? prefs = null,
  }) {
    return _then(_value.copyWith(
      prefs: null == prefs
          ? _value.prefs
          : prefs // ignore: cast_nullable_to_non_nullable
              as Map<String, Prefs>,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$PrefsDataImplCopyWith<$Res>
    implements $PrefsDataCopyWith<$Res> {
  factory _$$PrefsDataImplCopyWith(
          _$PrefsDataImpl value, $Res Function(_$PrefsDataImpl) then) =
      __$$PrefsDataImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({Map<String, Prefs> prefs});
}

/// @nodoc
class __$$PrefsDataImplCopyWithImpl<$Res>
    extends _$PrefsDataCopyWithImpl<$Res, _$PrefsDataImpl>
    implements _$$PrefsDataImplCopyWith<$Res> {
  __$$PrefsDataImplCopyWithImpl(
      _$PrefsDataImpl _value, $Res Function(_$PrefsDataImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? prefs = null,
  }) {
    return _then(_$PrefsDataImpl(
      prefs: null == prefs
          ? _value._prefs
          : prefs // ignore: cast_nullable_to_non_nullable
              as Map<String, Prefs>,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$PrefsDataImpl implements _PrefsData {
  const _$PrefsDataImpl({required final Map<String, Prefs> prefs})
      : _prefs = prefs;

  factory _$PrefsDataImpl.fromJson(Map<String, dynamic> json) =>
      _$$PrefsDataImplFromJson(json);

  final Map<String, Prefs> _prefs;
  @override
  Map<String, Prefs> get prefs {
    if (_prefs is EqualUnmodifiableMapView) return _prefs;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableMapView(_prefs);
  }

  @override
  String toString() {
    return 'PrefsData(prefs: $prefs)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$PrefsDataImpl &&
            const DeepCollectionEquality().equals(other._prefs, _prefs));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode =>
      Object.hash(runtimeType, const DeepCollectionEquality().hash(_prefs));

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$PrefsDataImplCopyWith<_$PrefsDataImpl> get copyWith =>
      __$$PrefsDataImplCopyWithImpl<_$PrefsDataImpl>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$PrefsDataImplToJson(
      this,
    );
  }
}

abstract class _PrefsData implements PrefsData {
  const factory _PrefsData({required final Map<String, Prefs> prefs}) =
      _$PrefsDataImpl;

  factory _PrefsData.fromJson(Map<String, dynamic> json) =
      _$PrefsDataImpl.fromJson;

  @override
  Map<String, Prefs> get prefs;
  @override
  @JsonKey(ignore: true)
  _$$PrefsDataImplCopyWith<_$PrefsDataImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

Prefs _$PrefsFromJson(Map<String, dynamic> json) {
  return _Prefs.fromJson(json);
}

/// @nodoc
mixin _$Prefs {
  String get description => throw _privateConstructorUsedError;
  String get name => throw _privateConstructorUsedError;
  dynamic get value => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $PrefsCopyWith<Prefs> get copyWith => throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $PrefsCopyWith<$Res> {
  factory $PrefsCopyWith(Prefs value, $Res Function(Prefs) then) =
      _$PrefsCopyWithImpl<$Res, Prefs>;
  @useResult
  $Res call({String description, String name, dynamic value});
}

/// @nodoc
class _$PrefsCopyWithImpl<$Res, $Val extends Prefs>
    implements $PrefsCopyWith<$Res> {
  _$PrefsCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? description = null,
    Object? name = null,
    Object? value = freezed,
  }) {
    return _then(_value.copyWith(
      description: null == description
          ? _value.description
          : description // ignore: cast_nullable_to_non_nullable
              as String,
      name: null == name
          ? _value.name
          : name // ignore: cast_nullable_to_non_nullable
              as String,
      value: freezed == value
          ? _value.value
          : value // ignore: cast_nullable_to_non_nullable
              as dynamic,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$PrefsImplCopyWith<$Res> implements $PrefsCopyWith<$Res> {
  factory _$$PrefsImplCopyWith(
          _$PrefsImpl value, $Res Function(_$PrefsImpl) then) =
      __$$PrefsImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String description, String name, dynamic value});
}

/// @nodoc
class __$$PrefsImplCopyWithImpl<$Res>
    extends _$PrefsCopyWithImpl<$Res, _$PrefsImpl>
    implements _$$PrefsImplCopyWith<$Res> {
  __$$PrefsImplCopyWithImpl(
      _$PrefsImpl _value, $Res Function(_$PrefsImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? description = null,
    Object? name = null,
    Object? value = freezed,
  }) {
    return _then(_$PrefsImpl(
      description: null == description
          ? _value.description
          : description // ignore: cast_nullable_to_non_nullable
              as String,
      name: null == name
          ? _value.name
          : name // ignore: cast_nullable_to_non_nullable
              as String,
      value: freezed == value
          ? _value.value
          : value // ignore: cast_nullable_to_non_nullable
              as dynamic,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$PrefsImpl implements _Prefs {
  const _$PrefsImpl(
      {required this.description, required this.name, required this.value});

  factory _$PrefsImpl.fromJson(Map<String, dynamic> json) =>
      _$$PrefsImplFromJson(json);

  @override
  final String description;
  @override
  final String name;
  @override
  final dynamic value;

  @override
  String toString() {
    return 'Prefs(description: $description, name: $name, value: $value)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$PrefsImpl &&
            (identical(other.description, description) ||
                other.description == description) &&
            (identical(other.name, name) || other.name == name) &&
            const DeepCollectionEquality().equals(other.value, value));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, description, name,
      const DeepCollectionEquality().hash(value));

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$PrefsImplCopyWith<_$PrefsImpl> get copyWith =>
      __$$PrefsImplCopyWithImpl<_$PrefsImpl>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$PrefsImplToJson(
      this,
    );
  }
}

abstract class _Prefs implements Prefs {
  const factory _Prefs(
      {required final String description,
      required final String name,
      required final dynamic value}) = _$PrefsImpl;

  factory _Prefs.fromJson(Map<String, dynamic> json) = _$PrefsImpl.fromJson;

  @override
  String get description;
  @override
  String get name;
  @override
  dynamic get value;
  @override
  @JsonKey(ignore: true)
  _$$PrefsImplCopyWith<_$PrefsImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

ReelConfig _$ReelConfigFromJson(Map<String, dynamic> json) {
  return _ReelConfig.fromJson(json);
}

/// @nodoc
mixin _$ReelConfig {
  String get bundleId => throw _privateConstructorUsedError;
  ReelEntryType get entry => throw _privateConstructorUsedError;
  String get sceneName => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $ReelConfigCopyWith<ReelConfig> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $ReelConfigCopyWith<$Res> {
  factory $ReelConfigCopyWith(
          ReelConfig value, $Res Function(ReelConfig) then) =
      _$ReelConfigCopyWithImpl<$Res, ReelConfig>;
  @useResult
  $Res call({String bundleId, ReelEntryType entry, String sceneName});
}

/// @nodoc
class _$ReelConfigCopyWithImpl<$Res, $Val extends ReelConfig>
    implements $ReelConfigCopyWith<$Res> {
  _$ReelConfigCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? bundleId = null,
    Object? entry = null,
    Object? sceneName = null,
  }) {
    return _then(_value.copyWith(
      bundleId: null == bundleId
          ? _value.bundleId
          : bundleId // ignore: cast_nullable_to_non_nullable
              as String,
      entry: null == entry
          ? _value.entry
          : entry // ignore: cast_nullable_to_non_nullable
              as ReelEntryType,
      sceneName: null == sceneName
          ? _value.sceneName
          : sceneName // ignore: cast_nullable_to_non_nullable
              as String,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$ReelConfigImplCopyWith<$Res>
    implements $ReelConfigCopyWith<$Res> {
  factory _$$ReelConfigImplCopyWith(
          _$ReelConfigImpl value, $Res Function(_$ReelConfigImpl) then) =
      __$$ReelConfigImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String bundleId, ReelEntryType entry, String sceneName});
}

/// @nodoc
class __$$ReelConfigImplCopyWithImpl<$Res>
    extends _$ReelConfigCopyWithImpl<$Res, _$ReelConfigImpl>
    implements _$$ReelConfigImplCopyWith<$Res> {
  __$$ReelConfigImplCopyWithImpl(
      _$ReelConfigImpl _value, $Res Function(_$ReelConfigImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? bundleId = null,
    Object? entry = null,
    Object? sceneName = null,
  }) {
    return _then(_$ReelConfigImpl(
      bundleId: null == bundleId
          ? _value.bundleId
          : bundleId // ignore: cast_nullable_to_non_nullable
              as String,
      entry: null == entry
          ? _value.entry
          : entry // ignore: cast_nullable_to_non_nullable
              as ReelEntryType,
      sceneName: null == sceneName
          ? _value.sceneName
          : sceneName // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$ReelConfigImpl implements _ReelConfig {
  const _$ReelConfigImpl(
      {required this.bundleId, required this.entry, required this.sceneName});

  factory _$ReelConfigImpl.fromJson(Map<String, dynamic> json) =>
      _$$ReelConfigImplFromJson(json);

  @override
  final String bundleId;
  @override
  final ReelEntryType entry;
  @override
  final String sceneName;

  @override
  String toString() {
    return 'ReelConfig(bundleId: $bundleId, entry: $entry, sceneName: $sceneName)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$ReelConfigImpl &&
            (identical(other.bundleId, bundleId) ||
                other.bundleId == bundleId) &&
            (identical(other.entry, entry) || other.entry == entry) &&
            (identical(other.sceneName, sceneName) ||
                other.sceneName == sceneName));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, bundleId, entry, sceneName);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$ReelConfigImplCopyWith<_$ReelConfigImpl> get copyWith =>
      __$$ReelConfigImplCopyWithImpl<_$ReelConfigImpl>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$ReelConfigImplToJson(
      this,
    );
  }
}

abstract class _ReelConfig implements ReelConfig {
  const factory _ReelConfig(
      {required final String bundleId,
      required final ReelEntryType entry,
      required final String sceneName}) = _$ReelConfigImpl;

  factory _ReelConfig.fromJson(Map<String, dynamic> json) =
      _$ReelConfigImpl.fromJson;

  @override
  String get bundleId;
  @override
  ReelEntryType get entry;
  @override
  String get sceneName;
  @override
  @JsonKey(ignore: true)
  _$$ReelConfigImplCopyWith<_$ReelConfigImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

ReelFilePath _$ReelFilePathFromJson(Map<String, dynamic> json) {
  return _ReelFilePath.fromJson(json);
}

/// @nodoc
mixin _$ReelFilePath {
  String get audio => throw _privateConstructorUsedError;
  String get thumbnail => throw _privateConstructorUsedError;
  String get video => throw _privateConstructorUsedError;
  String get xrs => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $ReelFilePathCopyWith<ReelFilePath> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $ReelFilePathCopyWith<$Res> {
  factory $ReelFilePathCopyWith(
          ReelFilePath value, $Res Function(ReelFilePath) then) =
      _$ReelFilePathCopyWithImpl<$Res, ReelFilePath>;
  @useResult
  $Res call({String audio, String thumbnail, String video, String xrs});
}

/// @nodoc
class _$ReelFilePathCopyWithImpl<$Res, $Val extends ReelFilePath>
    implements $ReelFilePathCopyWith<$Res> {
  _$ReelFilePathCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? audio = null,
    Object? thumbnail = null,
    Object? video = null,
    Object? xrs = null,
  }) {
    return _then(_value.copyWith(
      audio: null == audio
          ? _value.audio
          : audio // ignore: cast_nullable_to_non_nullable
              as String,
      thumbnail: null == thumbnail
          ? _value.thumbnail
          : thumbnail // ignore: cast_nullable_to_non_nullable
              as String,
      video: null == video
          ? _value.video
          : video // ignore: cast_nullable_to_non_nullable
              as String,
      xrs: null == xrs
          ? _value.xrs
          : xrs // ignore: cast_nullable_to_non_nullable
              as String,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$ReelFilePathImplCopyWith<$Res>
    implements $ReelFilePathCopyWith<$Res> {
  factory _$$ReelFilePathImplCopyWith(
          _$ReelFilePathImpl value, $Res Function(_$ReelFilePathImpl) then) =
      __$$ReelFilePathImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String audio, String thumbnail, String video, String xrs});
}

/// @nodoc
class __$$ReelFilePathImplCopyWithImpl<$Res>
    extends _$ReelFilePathCopyWithImpl<$Res, _$ReelFilePathImpl>
    implements _$$ReelFilePathImplCopyWith<$Res> {
  __$$ReelFilePathImplCopyWithImpl(
      _$ReelFilePathImpl _value, $Res Function(_$ReelFilePathImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? audio = null,
    Object? thumbnail = null,
    Object? video = null,
    Object? xrs = null,
  }) {
    return _then(_$ReelFilePathImpl(
      audio: null == audio
          ? _value.audio
          : audio // ignore: cast_nullable_to_non_nullable
              as String,
      thumbnail: null == thumbnail
          ? _value.thumbnail
          : thumbnail // ignore: cast_nullable_to_non_nullable
              as String,
      video: null == video
          ? _value.video
          : video // ignore: cast_nullable_to_non_nullable
              as String,
      xrs: null == xrs
          ? _value.xrs
          : xrs // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$ReelFilePathImpl implements _ReelFilePath {
  const _$ReelFilePathImpl(
      {required this.audio,
      required this.thumbnail,
      required this.video,
      required this.xrs});

  factory _$ReelFilePathImpl.fromJson(Map<String, dynamic> json) =>
      _$$ReelFilePathImplFromJson(json);

  @override
  final String audio;
  @override
  final String thumbnail;
  @override
  final String video;
  @override
  final String xrs;

  @override
  String toString() {
    return 'ReelFilePath(audio: $audio, thumbnail: $thumbnail, video: $video, xrs: $xrs)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$ReelFilePathImpl &&
            (identical(other.audio, audio) || other.audio == audio) &&
            (identical(other.thumbnail, thumbnail) ||
                other.thumbnail == thumbnail) &&
            (identical(other.video, video) || other.video == video) &&
            (identical(other.xrs, xrs) || other.xrs == xrs));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, audio, thumbnail, video, xrs);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$ReelFilePathImplCopyWith<_$ReelFilePathImpl> get copyWith =>
      __$$ReelFilePathImplCopyWithImpl<_$ReelFilePathImpl>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$ReelFilePathImplToJson(
      this,
    );
  }
}

abstract class _ReelFilePath implements ReelFilePath {
  const factory _ReelFilePath(
      {required final String audio,
      required final String thumbnail,
      required final String video,
      required final String xrs}) = _$ReelFilePathImpl;

  factory _ReelFilePath.fromJson(Map<String, dynamic> json) =
      _$ReelFilePathImpl.fromJson;

  @override
  String get audio;
  @override
  String get thumbnail;
  @override
  String get video;
  @override
  String get xrs;
  @override
  @JsonKey(ignore: true)
  _$$ReelFilePathImplCopyWith<_$ReelFilePathImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

ReelSceneConfig _$ReelSceneConfigFromJson(Map<String, dynamic> json) {
  return _ReelSceneConfig.fromJson(json);
}

/// @nodoc
mixin _$ReelSceneConfig {
  bool get decorationActive => throw _privateConstructorUsedError;
  List<ReelDecoration> get decorations => throw _privateConstructorUsedError;
  RecordStateType get initState => throw _privateConstructorUsedError;
  bool get motionButtonActive => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $ReelSceneConfigCopyWith<ReelSceneConfig> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $ReelSceneConfigCopyWith<$Res> {
  factory $ReelSceneConfigCopyWith(
          ReelSceneConfig value, $Res Function(ReelSceneConfig) then) =
      _$ReelSceneConfigCopyWithImpl<$Res, ReelSceneConfig>;
  @useResult
  $Res call(
      {bool decorationActive,
      List<ReelDecoration> decorations,
      RecordStateType initState,
      bool motionButtonActive});
}

/// @nodoc
class _$ReelSceneConfigCopyWithImpl<$Res, $Val extends ReelSceneConfig>
    implements $ReelSceneConfigCopyWith<$Res> {
  _$ReelSceneConfigCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? decorationActive = null,
    Object? decorations = null,
    Object? initState = null,
    Object? motionButtonActive = null,
  }) {
    return _then(_value.copyWith(
      decorationActive: null == decorationActive
          ? _value.decorationActive
          : decorationActive // ignore: cast_nullable_to_non_nullable
              as bool,
      decorations: null == decorations
          ? _value.decorations
          : decorations // ignore: cast_nullable_to_non_nullable
              as List<ReelDecoration>,
      initState: null == initState
          ? _value.initState
          : initState // ignore: cast_nullable_to_non_nullable
              as RecordStateType,
      motionButtonActive: null == motionButtonActive
          ? _value.motionButtonActive
          : motionButtonActive // ignore: cast_nullable_to_non_nullable
              as bool,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$ReelSceneConfigImplCopyWith<$Res>
    implements $ReelSceneConfigCopyWith<$Res> {
  factory _$$ReelSceneConfigImplCopyWith(_$ReelSceneConfigImpl value,
          $Res Function(_$ReelSceneConfigImpl) then) =
      __$$ReelSceneConfigImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {bool decorationActive,
      List<ReelDecoration> decorations,
      RecordStateType initState,
      bool motionButtonActive});
}

/// @nodoc
class __$$ReelSceneConfigImplCopyWithImpl<$Res>
    extends _$ReelSceneConfigCopyWithImpl<$Res, _$ReelSceneConfigImpl>
    implements _$$ReelSceneConfigImplCopyWith<$Res> {
  __$$ReelSceneConfigImplCopyWithImpl(
      _$ReelSceneConfigImpl _value, $Res Function(_$ReelSceneConfigImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? decorationActive = null,
    Object? decorations = null,
    Object? initState = null,
    Object? motionButtonActive = null,
  }) {
    return _then(_$ReelSceneConfigImpl(
      decorationActive: null == decorationActive
          ? _value.decorationActive
          : decorationActive // ignore: cast_nullable_to_non_nullable
              as bool,
      decorations: null == decorations
          ? _value._decorations
          : decorations // ignore: cast_nullable_to_non_nullable
              as List<ReelDecoration>,
      initState: null == initState
          ? _value.initState
          : initState // ignore: cast_nullable_to_non_nullable
              as RecordStateType,
      motionButtonActive: null == motionButtonActive
          ? _value.motionButtonActive
          : motionButtonActive // ignore: cast_nullable_to_non_nullable
              as bool,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$ReelSceneConfigImpl implements _ReelSceneConfig {
  const _$ReelSceneConfigImpl(
      {required this.decorationActive,
      required final List<ReelDecoration> decorations,
      required this.initState,
      required this.motionButtonActive})
      : _decorations = decorations;

  factory _$ReelSceneConfigImpl.fromJson(Map<String, dynamic> json) =>
      _$$ReelSceneConfigImplFromJson(json);

  @override
  final bool decorationActive;
  final List<ReelDecoration> _decorations;
  @override
  List<ReelDecoration> get decorations {
    if (_decorations is EqualUnmodifiableListView) return _decorations;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_decorations);
  }

  @override
  final RecordStateType initState;
  @override
  final bool motionButtonActive;

  @override
  String toString() {
    return 'ReelSceneConfig(decorationActive: $decorationActive, decorations: $decorations, initState: $initState, motionButtonActive: $motionButtonActive)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$ReelSceneConfigImpl &&
            (identical(other.decorationActive, decorationActive) ||
                other.decorationActive == decorationActive) &&
            const DeepCollectionEquality()
                .equals(other._decorations, _decorations) &&
            (identical(other.initState, initState) ||
                other.initState == initState) &&
            (identical(other.motionButtonActive, motionButtonActive) ||
                other.motionButtonActive == motionButtonActive));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(
      runtimeType,
      decorationActive,
      const DeepCollectionEquality().hash(_decorations),
      initState,
      motionButtonActive);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$ReelSceneConfigImplCopyWith<_$ReelSceneConfigImpl> get copyWith =>
      __$$ReelSceneConfigImplCopyWithImpl<_$ReelSceneConfigImpl>(
          this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$ReelSceneConfigImplToJson(
      this,
    );
  }
}

abstract class _ReelSceneConfig implements ReelSceneConfig {
  const factory _ReelSceneConfig(
      {required final bool decorationActive,
      required final List<ReelDecoration> decorations,
      required final RecordStateType initState,
      required final bool motionButtonActive}) = _$ReelSceneConfigImpl;

  factory _ReelSceneConfig.fromJson(Map<String, dynamic> json) =
      _$ReelSceneConfigImpl.fromJson;

  @override
  bool get decorationActive;
  @override
  List<ReelDecoration> get decorations;
  @override
  RecordStateType get initState;
  @override
  bool get motionButtonActive;
  @override
  @JsonKey(ignore: true)
  _$$ReelSceneConfigImplCopyWith<_$ReelSceneConfigImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

ReelDecoration _$ReelDecorationFromJson(Map<String, dynamic> json) {
  return _ReelDecoration.fromJson(json);
}

/// @nodoc
mixin _$ReelDecoration {
  String get id => throw _privateConstructorUsedError;
  String get itemType => throw _privateConstructorUsedError;
  String get thumbnail => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $ReelDecorationCopyWith<ReelDecoration> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $ReelDecorationCopyWith<$Res> {
  factory $ReelDecorationCopyWith(
          ReelDecoration value, $Res Function(ReelDecoration) then) =
      _$ReelDecorationCopyWithImpl<$Res, ReelDecoration>;
  @useResult
  $Res call({String id, String itemType, String thumbnail});
}

/// @nodoc
class _$ReelDecorationCopyWithImpl<$Res, $Val extends ReelDecoration>
    implements $ReelDecorationCopyWith<$Res> {
  _$ReelDecorationCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? id = null,
    Object? itemType = null,
    Object? thumbnail = null,
  }) {
    return _then(_value.copyWith(
      id: null == id
          ? _value.id
          : id // ignore: cast_nullable_to_non_nullable
              as String,
      itemType: null == itemType
          ? _value.itemType
          : itemType // ignore: cast_nullable_to_non_nullable
              as String,
      thumbnail: null == thumbnail
          ? _value.thumbnail
          : thumbnail // ignore: cast_nullable_to_non_nullable
              as String,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$ReelDecorationImplCopyWith<$Res>
    implements $ReelDecorationCopyWith<$Res> {
  factory _$$ReelDecorationImplCopyWith(_$ReelDecorationImpl value,
          $Res Function(_$ReelDecorationImpl) then) =
      __$$ReelDecorationImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String id, String itemType, String thumbnail});
}

/// @nodoc
class __$$ReelDecorationImplCopyWithImpl<$Res>
    extends _$ReelDecorationCopyWithImpl<$Res, _$ReelDecorationImpl>
    implements _$$ReelDecorationImplCopyWith<$Res> {
  __$$ReelDecorationImplCopyWithImpl(
      _$ReelDecorationImpl _value, $Res Function(_$ReelDecorationImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? id = null,
    Object? itemType = null,
    Object? thumbnail = null,
  }) {
    return _then(_$ReelDecorationImpl(
      id: null == id
          ? _value.id
          : id // ignore: cast_nullable_to_non_nullable
              as String,
      itemType: null == itemType
          ? _value.itemType
          : itemType // ignore: cast_nullable_to_non_nullable
              as String,
      thumbnail: null == thumbnail
          ? _value.thumbnail
          : thumbnail // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$ReelDecorationImpl implements _ReelDecoration {
  const _$ReelDecorationImpl(
      {required this.id, required this.itemType, required this.thumbnail});

  factory _$ReelDecorationImpl.fromJson(Map<String, dynamic> json) =>
      _$$ReelDecorationImplFromJson(json);

  @override
  final String id;
  @override
  final String itemType;
  @override
  final String thumbnail;

  @override
  String toString() {
    return 'ReelDecoration(id: $id, itemType: $itemType, thumbnail: $thumbnail)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$ReelDecorationImpl &&
            (identical(other.id, id) || other.id == id) &&
            (identical(other.itemType, itemType) ||
                other.itemType == itemType) &&
            (identical(other.thumbnail, thumbnail) ||
                other.thumbnail == thumbnail));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, id, itemType, thumbnail);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$ReelDecorationImplCopyWith<_$ReelDecorationImpl> get copyWith =>
      __$$ReelDecorationImplCopyWithImpl<_$ReelDecorationImpl>(
          this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$ReelDecorationImplToJson(
      this,
    );
  }
}

abstract class _ReelDecoration implements ReelDecoration {
  const factory _ReelDecoration(
      {required final String id,
      required final String itemType,
      required final String thumbnail}) = _$ReelDecorationImpl;

  factory _ReelDecoration.fromJson(Map<String, dynamic> json) =
      _$ReelDecorationImpl.fromJson;

  @override
  String get id;
  @override
  String get itemType;
  @override
  String get thumbnail;
  @override
  @JsonKey(ignore: true)
  _$$ReelDecorationImplCopyWith<_$ReelDecorationImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

RoomConfig _$RoomConfigFromJson(Map<String, dynamic> json) {
  return _RoomConfig.fromJson(json);
}

/// @nodoc
mixin _$RoomConfig {
  String get sceneKey => throw _privateConstructorUsedError;
  String get spaceId => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $RoomConfigCopyWith<RoomConfig> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $RoomConfigCopyWith<$Res> {
  factory $RoomConfigCopyWith(
          RoomConfig value, $Res Function(RoomConfig) then) =
      _$RoomConfigCopyWithImpl<$Res, RoomConfig>;
  @useResult
  $Res call({String sceneKey, String spaceId});
}

/// @nodoc
class _$RoomConfigCopyWithImpl<$Res, $Val extends RoomConfig>
    implements $RoomConfigCopyWith<$Res> {
  _$RoomConfigCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? sceneKey = null,
    Object? spaceId = null,
  }) {
    return _then(_value.copyWith(
      sceneKey: null == sceneKey
          ? _value.sceneKey
          : sceneKey // ignore: cast_nullable_to_non_nullable
              as String,
      spaceId: null == spaceId
          ? _value.spaceId
          : spaceId // ignore: cast_nullable_to_non_nullable
              as String,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$RoomConfigImplCopyWith<$Res>
    implements $RoomConfigCopyWith<$Res> {
  factory _$$RoomConfigImplCopyWith(
          _$RoomConfigImpl value, $Res Function(_$RoomConfigImpl) then) =
      __$$RoomConfigImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({String sceneKey, String spaceId});
}

/// @nodoc
class __$$RoomConfigImplCopyWithImpl<$Res>
    extends _$RoomConfigCopyWithImpl<$Res, _$RoomConfigImpl>
    implements _$$RoomConfigImplCopyWith<$Res> {
  __$$RoomConfigImplCopyWithImpl(
      _$RoomConfigImpl _value, $Res Function(_$RoomConfigImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? sceneKey = null,
    Object? spaceId = null,
  }) {
    return _then(_$RoomConfigImpl(
      sceneKey: null == sceneKey
          ? _value.sceneKey
          : sceneKey // ignore: cast_nullable_to_non_nullable
              as String,
      spaceId: null == spaceId
          ? _value.spaceId
          : spaceId // ignore: cast_nullable_to_non_nullable
              as String,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$RoomConfigImpl implements _RoomConfig {
  const _$RoomConfigImpl({required this.sceneKey, required this.spaceId});

  factory _$RoomConfigImpl.fromJson(Map<String, dynamic> json) =>
      _$$RoomConfigImplFromJson(json);

  @override
  final String sceneKey;
  @override
  final String spaceId;

  @override
  String toString() {
    return 'RoomConfig(sceneKey: $sceneKey, spaceId: $spaceId)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$RoomConfigImpl &&
            (identical(other.sceneKey, sceneKey) ||
                other.sceneKey == sceneKey) &&
            (identical(other.spaceId, spaceId) || other.spaceId == spaceId));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, sceneKey, spaceId);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$RoomConfigImplCopyWith<_$RoomConfigImpl> get copyWith =>
      __$$RoomConfigImplCopyWithImpl<_$RoomConfigImpl>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$RoomConfigImplToJson(
      this,
    );
  }
}

abstract class _RoomConfig implements RoomConfig {
  const factory _RoomConfig(
      {required final String sceneKey,
      required final String spaceId}) = _$RoomConfigImpl;

  factory _RoomConfig.fromJson(Map<String, dynamic> json) =
      _$RoomConfigImpl.fromJson;

  @override
  String get sceneKey;
  @override
  String get spaceId;
  @override
  @JsonKey(ignore: true)
  _$$RoomConfigImplCopyWith<_$RoomConfigImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

TrackingConfig _$TrackingConfigFromJson(Map<String, dynamic> json) {
  return _TrackingConfig.fromJson(json);
}

/// @nodoc
mixin _$TrackingConfig {
  bool get face => throw _privateConstructorUsedError;
  bool get fullBody => throw _privateConstructorUsedError;
  bool get upperBody => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $TrackingConfigCopyWith<TrackingConfig> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $TrackingConfigCopyWith<$Res> {
  factory $TrackingConfigCopyWith(
          TrackingConfig value, $Res Function(TrackingConfig) then) =
      _$TrackingConfigCopyWithImpl<$Res, TrackingConfig>;
  @useResult
  $Res call({bool face, bool fullBody, bool upperBody});
}

/// @nodoc
class _$TrackingConfigCopyWithImpl<$Res, $Val extends TrackingConfig>
    implements $TrackingConfigCopyWith<$Res> {
  _$TrackingConfigCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? face = null,
    Object? fullBody = null,
    Object? upperBody = null,
  }) {
    return _then(_value.copyWith(
      face: null == face
          ? _value.face
          : face // ignore: cast_nullable_to_non_nullable
              as bool,
      fullBody: null == fullBody
          ? _value.fullBody
          : fullBody // ignore: cast_nullable_to_non_nullable
              as bool,
      upperBody: null == upperBody
          ? _value.upperBody
          : upperBody // ignore: cast_nullable_to_non_nullable
              as bool,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$TrackingConfigImplCopyWith<$Res>
    implements $TrackingConfigCopyWith<$Res> {
  factory _$$TrackingConfigImplCopyWith(_$TrackingConfigImpl value,
          $Res Function(_$TrackingConfigImpl) then) =
      __$$TrackingConfigImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({bool face, bool fullBody, bool upperBody});
}

/// @nodoc
class __$$TrackingConfigImplCopyWithImpl<$Res>
    extends _$TrackingConfigCopyWithImpl<$Res, _$TrackingConfigImpl>
    implements _$$TrackingConfigImplCopyWith<$Res> {
  __$$TrackingConfigImplCopyWithImpl(
      _$TrackingConfigImpl _value, $Res Function(_$TrackingConfigImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? face = null,
    Object? fullBody = null,
    Object? upperBody = null,
  }) {
    return _then(_$TrackingConfigImpl(
      face: null == face
          ? _value.face
          : face // ignore: cast_nullable_to_non_nullable
              as bool,
      fullBody: null == fullBody
          ? _value.fullBody
          : fullBody // ignore: cast_nullable_to_non_nullable
              as bool,
      upperBody: null == upperBody
          ? _value.upperBody
          : upperBody // ignore: cast_nullable_to_non_nullable
              as bool,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$TrackingConfigImpl implements _TrackingConfig {
  const _$TrackingConfigImpl(
      {required this.face, required this.fullBody, required this.upperBody});

  factory _$TrackingConfigImpl.fromJson(Map<String, dynamic> json) =>
      _$$TrackingConfigImplFromJson(json);

  @override
  final bool face;
  @override
  final bool fullBody;
  @override
  final bool upperBody;

  @override
  String toString() {
    return 'TrackingConfig(face: $face, fullBody: $fullBody, upperBody: $upperBody)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$TrackingConfigImpl &&
            (identical(other.face, face) || other.face == face) &&
            (identical(other.fullBody, fullBody) ||
                other.fullBody == fullBody) &&
            (identical(other.upperBody, upperBody) ||
                other.upperBody == upperBody));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, face, fullBody, upperBody);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$TrackingConfigImplCopyWith<_$TrackingConfigImpl> get copyWith =>
      __$$TrackingConfigImplCopyWithImpl<_$TrackingConfigImpl>(
          this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$TrackingConfigImplToJson(
      this,
    );
  }
}

abstract class _TrackingConfig implements TrackingConfig {
  const factory _TrackingConfig(
      {required final bool face,
      required final bool fullBody,
      required final bool upperBody}) = _$TrackingConfigImpl;

  factory _TrackingConfig.fromJson(Map<String, dynamic> json) =
      _$TrackingConfigImpl.fromJson;

  @override
  bool get face;
  @override
  bool get fullBody;
  @override
  bool get upperBody;
  @override
  @JsonKey(ignore: true)
  _$$TrackingConfigImplCopyWith<_$TrackingConfigImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

TrackingState _$TrackingStateFromJson(Map<String, dynamic> json) {
  return _TrackingState.fromJson(json);
}

/// @nodoc
mixin _$TrackingState {
  TrackingStateType get state => throw _privateConstructorUsedError;
  TrackingFlag get type => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $TrackingStateCopyWith<TrackingState> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $TrackingStateCopyWith<$Res> {
  factory $TrackingStateCopyWith(
          TrackingState value, $Res Function(TrackingState) then) =
      _$TrackingStateCopyWithImpl<$Res, TrackingState>;
  @useResult
  $Res call({TrackingStateType state, TrackingFlag type});
}

/// @nodoc
class _$TrackingStateCopyWithImpl<$Res, $Val extends TrackingState>
    implements $TrackingStateCopyWith<$Res> {
  _$TrackingStateCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? state = null,
    Object? type = null,
  }) {
    return _then(_value.copyWith(
      state: null == state
          ? _value.state
          : state // ignore: cast_nullable_to_non_nullable
              as TrackingStateType,
      type: null == type
          ? _value.type
          : type // ignore: cast_nullable_to_non_nullable
              as TrackingFlag,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$TrackingStateImplCopyWith<$Res>
    implements $TrackingStateCopyWith<$Res> {
  factory _$$TrackingStateImplCopyWith(
          _$TrackingStateImpl value, $Res Function(_$TrackingStateImpl) then) =
      __$$TrackingStateImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call({TrackingStateType state, TrackingFlag type});
}

/// @nodoc
class __$$TrackingStateImplCopyWithImpl<$Res>
    extends _$TrackingStateCopyWithImpl<$Res, _$TrackingStateImpl>
    implements _$$TrackingStateImplCopyWith<$Res> {
  __$$TrackingStateImplCopyWithImpl(
      _$TrackingStateImpl _value, $Res Function(_$TrackingStateImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? state = null,
    Object? type = null,
  }) {
    return _then(_$TrackingStateImpl(
      state: null == state
          ? _value.state
          : state // ignore: cast_nullable_to_non_nullable
              as TrackingStateType,
      type: null == type
          ? _value.type
          : type // ignore: cast_nullable_to_non_nullable
              as TrackingFlag,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$TrackingStateImpl implements _TrackingState {
  const _$TrackingStateImpl({required this.state, required this.type});

  factory _$TrackingStateImpl.fromJson(Map<String, dynamic> json) =>
      _$$TrackingStateImplFromJson(json);

  @override
  final TrackingStateType state;
  @override
  final TrackingFlag type;

  @override
  String toString() {
    return 'TrackingState(state: $state, type: $type)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$TrackingStateImpl &&
            (identical(other.state, state) || other.state == state) &&
            (identical(other.type, type) || other.type == type));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode => Object.hash(runtimeType, state, type);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$TrackingStateImplCopyWith<_$TrackingStateImpl> get copyWith =>
      __$$TrackingStateImplCopyWithImpl<_$TrackingStateImpl>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$TrackingStateImplToJson(
      this,
    );
  }
}

abstract class _TrackingState implements TrackingState {
  const factory _TrackingState(
      {required final TrackingStateType state,
      required final TrackingFlag type}) = _$TrackingStateImpl;

  factory _TrackingState.fromJson(Map<String, dynamic> json) =
      _$TrackingStateImpl.fromJson;

  @override
  TrackingStateType get state;
  @override
  TrackingFlag get type;
  @override
  @JsonKey(ignore: true)
  _$$TrackingStateImplCopyWith<_$TrackingStateImpl> get copyWith =>
      throw _privateConstructorUsedError;
}

UnityMessage _$UnityMessageFromJson(Map<String, dynamic> json) {
  return _UnityMessage.fromJson(json);
}

/// @nodoc
mixin _$UnityMessage {
  String get data => throw _privateConstructorUsedError;
  ErrorCode get errorCode => throw _privateConstructorUsedError;
  String get errorMsg => throw _privateConstructorUsedError;
  String get sessionId => throw _privateConstructorUsedError;
  UnityMessageType get type => throw _privateConstructorUsedError;

  Map<String, dynamic> toJson() => throw _privateConstructorUsedError;
  @JsonKey(ignore: true)
  $UnityMessageCopyWith<UnityMessage> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $UnityMessageCopyWith<$Res> {
  factory $UnityMessageCopyWith(
          UnityMessage value, $Res Function(UnityMessage) then) =
      _$UnityMessageCopyWithImpl<$Res, UnityMessage>;
  @useResult
  $Res call(
      {String data,
      ErrorCode errorCode,
      String errorMsg,
      String sessionId,
      UnityMessageType type});
}

/// @nodoc
class _$UnityMessageCopyWithImpl<$Res, $Val extends UnityMessage>
    implements $UnityMessageCopyWith<$Res> {
  _$UnityMessageCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? data = null,
    Object? errorCode = null,
    Object? errorMsg = null,
    Object? sessionId = null,
    Object? type = null,
  }) {
    return _then(_value.copyWith(
      data: null == data
          ? _value.data
          : data // ignore: cast_nullable_to_non_nullable
              as String,
      errorCode: null == errorCode
          ? _value.errorCode
          : errorCode // ignore: cast_nullable_to_non_nullable
              as ErrorCode,
      errorMsg: null == errorMsg
          ? _value.errorMsg
          : errorMsg // ignore: cast_nullable_to_non_nullable
              as String,
      sessionId: null == sessionId
          ? _value.sessionId
          : sessionId // ignore: cast_nullable_to_non_nullable
              as String,
      type: null == type
          ? _value.type
          : type // ignore: cast_nullable_to_non_nullable
              as UnityMessageType,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$UnityMessageImplCopyWith<$Res>
    implements $UnityMessageCopyWith<$Res> {
  factory _$$UnityMessageImplCopyWith(
          _$UnityMessageImpl value, $Res Function(_$UnityMessageImpl) then) =
      __$$UnityMessageImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {String data,
      ErrorCode errorCode,
      String errorMsg,
      String sessionId,
      UnityMessageType type});
}

/// @nodoc
class __$$UnityMessageImplCopyWithImpl<$Res>
    extends _$UnityMessageCopyWithImpl<$Res, _$UnityMessageImpl>
    implements _$$UnityMessageImplCopyWith<$Res> {
  __$$UnityMessageImplCopyWithImpl(
      _$UnityMessageImpl _value, $Res Function(_$UnityMessageImpl) _then)
      : super(_value, _then);

  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? data = null,
    Object? errorCode = null,
    Object? errorMsg = null,
    Object? sessionId = null,
    Object? type = null,
  }) {
    return _then(_$UnityMessageImpl(
      data: null == data
          ? _value.data
          : data // ignore: cast_nullable_to_non_nullable
              as String,
      errorCode: null == errorCode
          ? _value.errorCode
          : errorCode // ignore: cast_nullable_to_non_nullable
              as ErrorCode,
      errorMsg: null == errorMsg
          ? _value.errorMsg
          : errorMsg // ignore: cast_nullable_to_non_nullable
              as String,
      sessionId: null == sessionId
          ? _value.sessionId
          : sessionId // ignore: cast_nullable_to_non_nullable
              as String,
      type: null == type
          ? _value.type
          : type // ignore: cast_nullable_to_non_nullable
              as UnityMessageType,
    ));
  }
}

/// @nodoc
@JsonSerializable()
class _$UnityMessageImpl implements _UnityMessage {
  const _$UnityMessageImpl(
      {required this.data,
      required this.errorCode,
      required this.errorMsg,
      required this.sessionId,
      required this.type});

  factory _$UnityMessageImpl.fromJson(Map<String, dynamic> json) =>
      _$$UnityMessageImplFromJson(json);

  @override
  final String data;
  @override
  final ErrorCode errorCode;
  @override
  final String errorMsg;
  @override
  final String sessionId;
  @override
  final UnityMessageType type;

  @override
  String toString() {
    return 'UnityMessage(data: $data, errorCode: $errorCode, errorMsg: $errorMsg, sessionId: $sessionId, type: $type)';
  }

  @override
  bool operator ==(dynamic other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$UnityMessageImpl &&
            (identical(other.data, data) || other.data == data) &&
            (identical(other.errorCode, errorCode) ||
                other.errorCode == errorCode) &&
            (identical(other.errorMsg, errorMsg) ||
                other.errorMsg == errorMsg) &&
            (identical(other.sessionId, sessionId) ||
                other.sessionId == sessionId) &&
            (identical(other.type, type) || other.type == type));
  }

  @JsonKey(ignore: true)
  @override
  int get hashCode =>
      Object.hash(runtimeType, data, errorCode, errorMsg, sessionId, type);

  @JsonKey(ignore: true)
  @override
  @pragma('vm:prefer-inline')
  _$$UnityMessageImplCopyWith<_$UnityMessageImpl> get copyWith =>
      __$$UnityMessageImplCopyWithImpl<_$UnityMessageImpl>(this, _$identity);

  @override
  Map<String, dynamic> toJson() {
    return _$$UnityMessageImplToJson(
      this,
    );
  }
}

abstract class _UnityMessage implements UnityMessage {
  const factory _UnityMessage(
      {required final String data,
      required final ErrorCode errorCode,
      required final String errorMsg,
      required final String sessionId,
      required final UnityMessageType type}) = _$UnityMessageImpl;

  factory _UnityMessage.fromJson(Map<String, dynamic> json) =
      _$UnityMessageImpl.fromJson;

  @override
  String get data;
  @override
  ErrorCode get errorCode;
  @override
  String get errorMsg;
  @override
  String get sessionId;
  @override
  UnityMessageType get type;
  @override
  @JsonKey(ignore: true)
  _$$UnityMessageImplCopyWith<_$UnityMessageImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
