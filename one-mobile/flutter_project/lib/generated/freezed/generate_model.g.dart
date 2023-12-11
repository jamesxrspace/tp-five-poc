// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'generate_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$FlutterMessageImpl _$$FlutterMessageImplFromJson(Map<String, dynamic> json) =>
    _$FlutterMessageImpl(
      data: json['data'] as String,
      errorCode: $enumDecode(_$ErrorCodeEnumMap, json['errorCode']),
      errorMsg: json['errorMsg'] as String,
      sessionId: json['sessionId'] as String,
      type: $enumDecode(_$FlutterMessageTypeEnumMap, json['type']),
    );

Map<String, dynamic> _$$FlutterMessageImplToJson(
        _$FlutterMessageImpl instance) =>
    <String, dynamic>{
      'data': instance.data,
      'errorCode': _$ErrorCodeEnumMap[instance.errorCode]!,
      'errorMsg': instance.errorMsg,
      'sessionId': instance.sessionId,
      'type': _$FlutterMessageTypeEnumMap[instance.type]!,
    };

const _$ErrorCodeEnumMap = {
  ErrorCode.FAIL: 'FAIL',
  ErrorCode.SUCCESS: 'SUCCESS',
};

const _$FlutterMessageTypeEnumMap = {
  FlutterMessageType.COCREATE_JOIN: 'COCREATE_JOIN',
  FlutterMessageType.FLUTTER: 'FLUTTER',
  FlutterMessageType.GET_TRACK_COUNT: 'GET_TRACK_COUNT',
  FlutterMessageType.LOGIN_SUCCESS: 'LOGIN_SUCCESS',
  FlutterMessageType.PLAY_MUSIC: 'PLAY_MUSIC',
  FlutterMessageType.PREFS: 'PREFS',
  FlutterMessageType.REEL_DECORATION_DONE: 'REEL_DECORATION_DONE',
  FlutterMessageType.REEL_GO_BACK_DECORATION: 'REEL_GO_BACK_DECORATION',
  FlutterMessageType.REEL_SELECTED_DECORATION: 'REEL_SELECTED_DECORATION',
  FlutterMessageType.REQUEST_REEL_SCENE_CONFIG: 'REQUEST_REEL_SCENE_CONFIG',
  FlutterMessageType.REQUEST_TO_PREVIEW: 'REQUEST_TO_PREVIEW',
  FlutterMessageType.REQUEST_TO_REEL_PAGE: 'REQUEST_TO_REEL_PAGE',
  FlutterMessageType.REQUEST_TO_SOCIAL_LOBBY_PAGE:
      'REQUEST_TO_SOCIAL_LOBBY_PAGE',
  FlutterMessageType.REQUEST_TO_SPACE: 'REQUEST_TO_SPACE',
  FlutterMessageType.REQUEST_TRACKING_STATE: 'REQUEST_TRACKING_STATE',
  FlutterMessageType.RESET_RECORD: 'RESET_RECORD',
  FlutterMessageType.SELECT_TRACK: 'SELECT_TRACK',
  FlutterMessageType.SET_CAMERA: 'SET_CAMERA',
  FlutterMessageType.SET_MIC: 'SET_MIC',
  FlutterMessageType.SET_MUSIC: 'SET_MUSIC',
  FlutterMessageType.START_AIGC: 'START_AIGC',
  FlutterMessageType.START_FILM: 'START_FILM',
  FlutterMessageType.START_PREVIEW: 'START_PREVIEW',
  FlutterMessageType.START_RECORD: 'START_RECORD',
  FlutterMessageType.STOP_PREVIEW: 'STOP_PREVIEW',
  FlutterMessageType.STOP_RECORD: 'STOP_RECORD',
  FlutterMessageType.TOGGLE_TRACKING: 'TOGGLE_TRACKING',
  FlutterMessageType.UPLOAD_REEL: 'UPLOAD_REEL',
};

_$MusicDataImpl _$$MusicDataImplFromJson(Map<String, dynamic> json) =>
    _$MusicDataImpl(
      aigcPath: json['aigcPath'] as String,
      singerName: json['singerName'] as String,
      songName: json['songName'] as String,
      songPath: json['songPath'] as String,
      tag: json['tag'] as String,
      thumbnailPath: json['thumbnailPath'] as String,
    );

Map<String, dynamic> _$$MusicDataImplToJson(_$MusicDataImpl instance) =>
    <String, dynamic>{
      'aigcPath': instance.aigcPath,
      'singerName': instance.singerName,
      'songName': instance.songName,
      'songPath': instance.songPath,
      'tag': instance.tag,
      'thumbnailPath': instance.thumbnailPath,
    };

_$PrefsDataImpl _$$PrefsDataImplFromJson(Map<String, dynamic> json) =>
    _$PrefsDataImpl(
      prefs: (json['prefs'] as Map<String, dynamic>).map(
        (k, e) => MapEntry(k, Prefs.fromJson(e as Map<String, dynamic>)),
      ),
    );

Map<String, dynamic> _$$PrefsDataImplToJson(_$PrefsDataImpl instance) =>
    <String, dynamic>{
      'prefs': instance.prefs,
    };

_$PrefsImpl _$$PrefsImplFromJson(Map<String, dynamic> json) => _$PrefsImpl(
      description: json['description'] as String,
      name: json['name'] as String,
      value: json['value'],
    );

Map<String, dynamic> _$$PrefsImplToJson(_$PrefsImpl instance) =>
    <String, dynamic>{
      'description': instance.description,
      'name': instance.name,
      'value': instance.value,
    };

_$ReelConfigImpl _$$ReelConfigImplFromJson(Map<String, dynamic> json) =>
    _$ReelConfigImpl(
      bundleId: json['bundleId'] as String,
      entry: $enumDecode(_$ReelEntryTypeEnumMap, json['entry']),
      sceneName: json['sceneName'] as String,
    );

Map<String, dynamic> _$$ReelConfigImplToJson(_$ReelConfigImpl instance) =>
    <String, dynamic>{
      'bundleId': instance.bundleId,
      'entry': _$ReelEntryTypeEnumMap[instance.entry]!,
      'sceneName': instance.sceneName,
    };

const _$ReelEntryTypeEnumMap = {
  ReelEntryType.BROWSE: 'BROWSE',
  ReelEntryType.CREATE: 'CREATE',
};

_$ReelFilePathImpl _$$ReelFilePathImplFromJson(Map<String, dynamic> json) =>
    _$ReelFilePathImpl(
      audio: json['audio'] as String,
      thumbnail: json['thumbnail'] as String,
      video: json['video'] as String,
      xrs: json['xrs'] as String,
    );

Map<String, dynamic> _$$ReelFilePathImplToJson(_$ReelFilePathImpl instance) =>
    <String, dynamic>{
      'audio': instance.audio,
      'thumbnail': instance.thumbnail,
      'video': instance.video,
      'xrs': instance.xrs,
    };

_$ReelSceneConfigImpl _$$ReelSceneConfigImplFromJson(
        Map<String, dynamic> json) =>
    _$ReelSceneConfigImpl(
      decorationActive: json['decorationActive'] as bool,
      decorations: (json['decorations'] as List<dynamic>)
          .map((e) => ReelDecoration.fromJson(e as Map<String, dynamic>))
          .toList(),
      initState: $enumDecode(_$RecordStateTypeEnumMap, json['initState']),
      motionButtonActive: json['motionButtonActive'] as bool,
    );

Map<String, dynamic> _$$ReelSceneConfigImplToJson(
        _$ReelSceneConfigImpl instance) =>
    <String, dynamic>{
      'decorationActive': instance.decorationActive,
      'decorations': instance.decorations,
      'initState': _$RecordStateTypeEnumMap[instance.initState]!,
      'motionButtonActive': instance.motionButtonActive,
    };

const _$RecordStateTypeEnumMap = {
  RecordStateType.DONE: 'DONE',
  RecordStateType.PRESET: 'PRESET',
  RecordStateType.PREVIEW: 'PREVIEW',
  RecordStateType.RECORDING: 'RECORDING',
  RecordStateType.STANDBY: 'STANDBY',
  RecordStateType.START_DECORATION: 'START_DECORATION',
  RecordStateType.UPLOAD: 'UPLOAD',
  RecordStateType.WATCH: 'WATCH',
};

_$ReelDecorationImpl _$$ReelDecorationImplFromJson(Map<String, dynamic> json) =>
    _$ReelDecorationImpl(
      id: json['id'] as String,
      itemType: json['itemType'] as String,
      thumbnail: json['thumbnail'] as String,
    );

Map<String, dynamic> _$$ReelDecorationImplToJson(
        _$ReelDecorationImpl instance) =>
    <String, dynamic>{
      'id': instance.id,
      'itemType': instance.itemType,
      'thumbnail': instance.thumbnail,
    };

_$RoomConfigImpl _$$RoomConfigImplFromJson(Map<String, dynamic> json) =>
    _$RoomConfigImpl(
      sceneKey: json['sceneKey'] as String,
      spaceId: json['spaceId'] as String,
    );

Map<String, dynamic> _$$RoomConfigImplToJson(_$RoomConfigImpl instance) =>
    <String, dynamic>{
      'sceneKey': instance.sceneKey,
      'spaceId': instance.spaceId,
    };

_$TrackingConfigImpl _$$TrackingConfigImplFromJson(Map<String, dynamic> json) =>
    _$TrackingConfigImpl(
      face: json['face'] as bool,
      fullBody: json['fullBody'] as bool,
      upperBody: json['upperBody'] as bool,
    );

Map<String, dynamic> _$$TrackingConfigImplToJson(
        _$TrackingConfigImpl instance) =>
    <String, dynamic>{
      'face': instance.face,
      'fullBody': instance.fullBody,
      'upperBody': instance.upperBody,
    };

_$TrackingStateImpl _$$TrackingStateImplFromJson(Map<String, dynamic> json) =>
    _$TrackingStateImpl(
      state: $enumDecode(_$TrackingStateTypeEnumMap, json['state']),
      type: $enumDecode(_$TrackingFlagEnumMap, json['type']),
    );

Map<String, dynamic> _$$TrackingStateImplToJson(_$TrackingStateImpl instance) =>
    <String, dynamic>{
      'state': _$TrackingStateTypeEnumMap[instance.state]!,
      'type': _$TrackingFlagEnumMap[instance.type]!,
    };

const _$TrackingStateTypeEnumMap = {
  TrackingStateType.DETECTING: 'DETECTING',
  TrackingStateType.INITIATING: 'INITIATING',
  TrackingStateType.TRACKING: 'TRACKING',
};

const _$TrackingFlagEnumMap = {
  TrackingFlag.FACE: 'FACE',
  TrackingFlag.FULL_BODY: 'FULL_BODY',
  TrackingFlag.UPPER_BODY: 'UPPER_BODY',
};

_$UnityMessageImpl _$$UnityMessageImplFromJson(Map<String, dynamic> json) =>
    _$UnityMessageImpl(
      data: json['data'] as String,
      errorCode: $enumDecode(_$ErrorCodeEnumMap, json['errorCode']),
      errorMsg: json['errorMsg'] as String,
      sessionId: json['sessionId'] as String,
      type: $enumDecode(_$UnityMessageTypeEnumMap, json['type']),
    );

Map<String, dynamic> _$$UnityMessageImplToJson(_$UnityMessageImpl instance) =>
    <String, dynamic>{
      'data': instance.data,
      'errorCode': _$ErrorCodeEnumMap[instance.errorCode]!,
      'errorMsg': instance.errorMsg,
      'sessionId': instance.sessionId,
      'type': _$UnityMessageTypeEnumMap[instance.type]!,
    };

const _$UnityMessageTypeEnumMap = {
  UnityMessageType.GENERAL_STATUS: 'GENERAL_STATUS',
  UnityMessageType.GENERATED_AIGC: 'GENERATED_AIGC',
  UnityMessageType.HIDE_LOADING: 'HIDE_LOADING',
  UnityMessageType.LOGIN_ONSHOW: 'LOGIN_ONSHOW',
  UnityMessageType.RECORD_STATE: 'RECORD_STATE',
  UnityMessageType.REQUEST_ACCESS_TOKEN: 'REQUEST_ACCESS_TOKEN',
  UnityMessageType.SHOW_LOADING: 'SHOW_LOADING',
  UnityMessageType.SHOW_TOAST: 'SHOW_TOAST',
  UnityMessageType.SWITCHED_TO_COCREATE_PAGE: 'SWITCHED_TO_COCREATE_PAGE',
  UnityMessageType.SWITCHED_TO_REEL_PAGE: 'SWITCHED_TO_REEL_PAGE',
  UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE:
      'SWITCHED_TO_SOCIAL_LOBBY_PAGE',
  UnityMessageType.SWITCHED_TO_SPACE: 'SWITCHED_TO_SPACE',
  UnityMessageType.TO_AVATAR_EDIT: 'TO_AVATAR_EDIT',
  UnityMessageType.TRACKING_STATE: 'TRACKING_STATE',
  UnityMessageType.UNITY: 'UNITY',
};
