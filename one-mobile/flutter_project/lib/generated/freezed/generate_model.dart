// To parse this JSON data, do
//
//     final errorCode = errorCodeFromJson(jsonString);
//     final flutterMessage = flutterMessageFromJson(jsonString);
//     final flutterMessageType = flutterMessageTypeFromJson(jsonString);
//     final musicData = musicDataFromJson(jsonString);
//     final prefs = prefsFromJson(jsonString);
//     final prefsData = prefsDataFromJson(jsonString);
//     final recordStateType = recordStateTypeFromJson(jsonString);
//     final reelConfig = reelConfigFromJson(jsonString);
//     final reelDecoration = reelDecorationFromJson(jsonString);
//     final reelEntryType = reelEntryTypeFromJson(jsonString);
//     final reelFilePath = reelFilePathFromJson(jsonString);
//     final reelSceneConfig = reelSceneConfigFromJson(jsonString);
//     final roomConfig = roomConfigFromJson(jsonString);
//     final trackingConfig = trackingConfigFromJson(jsonString);
//     final trackingFlag = trackingFlagFromJson(jsonString);
//     final trackingState = trackingStateFromJson(jsonString);
//     final trackingStateType = trackingStateTypeFromJson(jsonString);
//     final unityMessage = unityMessageFromJson(jsonString);
//     final unityMessageType = unityMessageTypeFromJson(jsonString);

import 'package:meta/meta.dart';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'dart:convert';

part 'generate_model.freezed.dart';
part 'generate_model.g.dart';

ErrorCode errorCodeFromJson(String str) => errorCodeValues.map[json.decode(str)]!;

String errorCodeToJson(ErrorCode data) => json.encode(errorCodeValues.reverse[data]);

FlutterMessage flutterMessageFromJson(String str) => FlutterMessage.fromJson(json.decode(str));

String flutterMessageToJson(FlutterMessage data) => json.encode(data.toJson());

FlutterMessageType flutterMessageTypeFromJson(String str) => flutterMessageTypeValues.map[json.decode(str)]!;

String flutterMessageTypeToJson(FlutterMessageType data) => json.encode(flutterMessageTypeValues.reverse[data]);

MusicData musicDataFromJson(String str) => MusicData.fromJson(json.decode(str));

String musicDataToJson(MusicData data) => json.encode(data.toJson());

Prefs prefsFromJson(String str) => Prefs.fromJson(json.decode(str));

String prefsToJson(Prefs data) => json.encode(data.toJson());

PrefsData prefsDataFromJson(String str) => PrefsData.fromJson(json.decode(str));

String prefsDataToJson(PrefsData data) => json.encode(data.toJson());

RecordStateType recordStateTypeFromJson(String str) => recordStateTypeValues.map[json.decode(str)]!;

String recordStateTypeToJson(RecordStateType data) => json.encode(recordStateTypeValues.reverse[data]);

ReelConfig reelConfigFromJson(String str) => ReelConfig.fromJson(json.decode(str));

String reelConfigToJson(ReelConfig data) => json.encode(data.toJson());

ReelDecoration reelDecorationFromJson(String str) => ReelDecoration.fromJson(json.decode(str));

String reelDecorationToJson(ReelDecoration data) => json.encode(data.toJson());

ReelEntryType reelEntryTypeFromJson(String str) => reelEntryTypeValues.map[json.decode(str)]!;

String reelEntryTypeToJson(ReelEntryType data) => json.encode(reelEntryTypeValues.reverse[data]);

ReelFilePath reelFilePathFromJson(String str) => ReelFilePath.fromJson(json.decode(str));

String reelFilePathToJson(ReelFilePath data) => json.encode(data.toJson());

ReelSceneConfig reelSceneConfigFromJson(String str) => ReelSceneConfig.fromJson(json.decode(str));

String reelSceneConfigToJson(ReelSceneConfig data) => json.encode(data.toJson());

RoomConfig roomConfigFromJson(String str) => RoomConfig.fromJson(json.decode(str));

String roomConfigToJson(RoomConfig data) => json.encode(data.toJson());

TrackingConfig trackingConfigFromJson(String str) => TrackingConfig.fromJson(json.decode(str));

String trackingConfigToJson(TrackingConfig data) => json.encode(data.toJson());

TrackingFlag trackingFlagFromJson(String str) => trackingFlagValues.map[json.decode(str)]!;

String trackingFlagToJson(TrackingFlag data) => json.encode(trackingFlagValues.reverse[data]);

TrackingState trackingStateFromJson(String str) => TrackingState.fromJson(json.decode(str));

String trackingStateToJson(TrackingState data) => json.encode(data.toJson());

TrackingStateType trackingStateTypeFromJson(String str) => trackingStateTypeValues.map[json.decode(str)]!;

String trackingStateTypeToJson(TrackingStateType data) => json.encode(trackingStateTypeValues.reverse[data]);

UnityMessage unityMessageFromJson(String str) => UnityMessage.fromJson(json.decode(str));

String unityMessageToJson(UnityMessage data) => json.encode(data.toJson());

UnityMessageType unityMessageTypeFromJson(String str) => unityMessageTypeValues.map[json.decode(str)]!;

String unityMessageTypeToJson(UnityMessageType data) => json.encode(unityMessageTypeValues.reverse[data]);

@freezed
class FlutterMessage with _$FlutterMessage {
    const factory FlutterMessage({
        required String data,
        required ErrorCode errorCode,
        required String errorMsg,
        required String sessionId,
        required FlutterMessageType type,
    }) = _FlutterMessage;

    factory FlutterMessage.fromJson(Map<String, dynamic> json) => _$FlutterMessageFromJson(json);
}

enum ErrorCode {
    FAIL,
    SUCCESS
}

final errorCodeValues = EnumValues({
    "FAIL": ErrorCode.FAIL,
    "SUCCESS": ErrorCode.SUCCESS
});

enum FlutterMessageType {
    COCREATE_JOIN,
    FLUTTER,
    GET_TRACK_COUNT,
    LOGIN_SUCCESS,
    PLAY_MUSIC,
    PREFS,
    REEL_DECORATION_DONE,
    REEL_GO_BACK_DECORATION,
    REEL_SELECTED_DECORATION,
    REQUEST_REEL_SCENE_CONFIG,
    REQUEST_TO_PREVIEW,
    REQUEST_TO_REEL_PAGE,
    REQUEST_TO_SOCIAL_LOBBY_PAGE,
    REQUEST_TO_SPACE,
    REQUEST_TRACKING_STATE,
    RESET_RECORD,
    SELECT_TRACK,
    SET_CAMERA,
    SET_MIC,
    SET_MUSIC,
    START_AIGC,
    START_FILM,
    START_PREVIEW,
    START_RECORD,
    STOP_PREVIEW,
    STOP_RECORD,
    TOGGLE_TRACKING,
    UPLOAD_REEL
}

final flutterMessageTypeValues = EnumValues({
    "COCREATE_JOIN": FlutterMessageType.COCREATE_JOIN,
    "FLUTTER": FlutterMessageType.FLUTTER,
    "GET_TRACK_COUNT": FlutterMessageType.GET_TRACK_COUNT,
    "LOGIN_SUCCESS": FlutterMessageType.LOGIN_SUCCESS,
    "PLAY_MUSIC": FlutterMessageType.PLAY_MUSIC,
    "PREFS": FlutterMessageType.PREFS,
    "REEL_DECORATION_DONE": FlutterMessageType.REEL_DECORATION_DONE,
    "REEL_GO_BACK_DECORATION": FlutterMessageType.REEL_GO_BACK_DECORATION,
    "REEL_SELECTED_DECORATION": FlutterMessageType.REEL_SELECTED_DECORATION,
    "REQUEST_REEL_SCENE_CONFIG": FlutterMessageType.REQUEST_REEL_SCENE_CONFIG,
    "REQUEST_TO_PREVIEW": FlutterMessageType.REQUEST_TO_PREVIEW,
    "REQUEST_TO_REEL_PAGE": FlutterMessageType.REQUEST_TO_REEL_PAGE,
    "REQUEST_TO_SOCIAL_LOBBY_PAGE": FlutterMessageType.REQUEST_TO_SOCIAL_LOBBY_PAGE,
    "REQUEST_TO_SPACE": FlutterMessageType.REQUEST_TO_SPACE,
    "REQUEST_TRACKING_STATE": FlutterMessageType.REQUEST_TRACKING_STATE,
    "RESET_RECORD": FlutterMessageType.RESET_RECORD,
    "SELECT_TRACK": FlutterMessageType.SELECT_TRACK,
    "SET_CAMERA": FlutterMessageType.SET_CAMERA,
    "SET_MIC": FlutterMessageType.SET_MIC,
    "SET_MUSIC": FlutterMessageType.SET_MUSIC,
    "START_AIGC": FlutterMessageType.START_AIGC,
    "START_FILM": FlutterMessageType.START_FILM,
    "START_PREVIEW": FlutterMessageType.START_PREVIEW,
    "START_RECORD": FlutterMessageType.START_RECORD,
    "STOP_PREVIEW": FlutterMessageType.STOP_PREVIEW,
    "STOP_RECORD": FlutterMessageType.STOP_RECORD,
    "TOGGLE_TRACKING": FlutterMessageType.TOGGLE_TRACKING,
    "UPLOAD_REEL": FlutterMessageType.UPLOAD_REEL
});

@freezed
class MusicData with _$MusicData {
    const factory MusicData({
        required String aigcPath,
        required String singerName,
        required String songName,
        required String songPath,
        required String tag,
        required String thumbnailPath,
    }) = _MusicData;

    factory MusicData.fromJson(Map<String, dynamic> json) => _$MusicDataFromJson(json);
}

@freezed
class PrefsData with _$PrefsData {
    const factory PrefsData({
        required Map<String, Prefs> prefs,
    }) = _PrefsData;

    factory PrefsData.fromJson(Map<String, dynamic> json) => _$PrefsDataFromJson(json);
}

@freezed
class Prefs with _$Prefs {
    const factory Prefs({
        required String description,
        required String name,
        required dynamic value,
    }) = _Prefs;

    factory Prefs.fromJson(Map<String, dynamic> json) => _$PrefsFromJson(json);
}

@freezed
class ReelConfig with _$ReelConfig {
    const factory ReelConfig({
        required String bundleId,
        required ReelEntryType entry,
        required String sceneName,
    }) = _ReelConfig;

    factory ReelConfig.fromJson(Map<String, dynamic> json) => _$ReelConfigFromJson(json);
}


///type of entry reel scope
enum ReelEntryType {
    BROWSE,
    CREATE
}

final reelEntryTypeValues = EnumValues({
    "BROWSE": ReelEntryType.BROWSE,
    "CREATE": ReelEntryType.CREATE
});

@freezed
class ReelFilePath with _$ReelFilePath {
    const factory ReelFilePath({
        required String audio,
        required String thumbnail,
        required String video,
        required String xrs,
    }) = _ReelFilePath;

    factory ReelFilePath.fromJson(Map<String, dynamic> json) => _$ReelFilePathFromJson(json);
}

@freezed
class ReelSceneConfig with _$ReelSceneConfig {
    const factory ReelSceneConfig({
        required bool decorationActive,
        required List<ReelDecoration> decorations,
        required RecordStateType initState,
        required bool motionButtonActive,
    }) = _ReelSceneConfig;

    factory ReelSceneConfig.fromJson(Map<String, dynamic> json) => _$ReelSceneConfigFromJson(json);
}

@freezed
class ReelDecoration with _$ReelDecoration {
    const factory ReelDecoration({
        required String id,
        required String itemType,
        required String thumbnail,
    }) = _ReelDecoration;

    factory ReelDecoration.fromJson(Map<String, dynamic> json) => _$ReelDecorationFromJson(json);
}

enum RecordStateType {
    DONE,
    PRESET,
    PREVIEW,
    RECORDING,
    STANDBY,
    START_DECORATION,
    UPLOAD,
    WATCH
}

final recordStateTypeValues = EnumValues({
    "DONE": RecordStateType.DONE,
    "PRESET": RecordStateType.PRESET,
    "PREVIEW": RecordStateType.PREVIEW,
    "RECORDING": RecordStateType.RECORDING,
    "STANDBY": RecordStateType.STANDBY,
    "START_DECORATION": RecordStateType.START_DECORATION,
    "UPLOAD": RecordStateType.UPLOAD,
    "WATCH": RecordStateType.WATCH
});

@freezed
class RoomConfig with _$RoomConfig {
    const factory RoomConfig({
        required String sceneKey,
        required String spaceId,
    }) = _RoomConfig;

    factory RoomConfig.fromJson(Map<String, dynamic> json) => _$RoomConfigFromJson(json);
}

@freezed
class TrackingConfig with _$TrackingConfig {
    const factory TrackingConfig({
        required bool face,
        required bool fullBody,
        required bool upperBody,
    }) = _TrackingConfig;

    factory TrackingConfig.fromJson(Map<String, dynamic> json) => _$TrackingConfigFromJson(json);
}

@freezed
class TrackingState with _$TrackingState {
    const factory TrackingState({
        required TrackingStateType state,
        required TrackingFlag type,
    }) = _TrackingState;

    factory TrackingState.fromJson(Map<String, dynamic> json) => _$TrackingStateFromJson(json);
}

enum TrackingStateType {
    DETECTING,
    INITIATING,
    TRACKING
}

final trackingStateTypeValues = EnumValues({
    "DETECTING": TrackingStateType.DETECTING,
    "INITIATING": TrackingStateType.INITIATING,
    "TRACKING": TrackingStateType.TRACKING
});

enum TrackingFlag {
    FACE,
    FULL_BODY,
    UPPER_BODY
}

final trackingFlagValues = EnumValues({
    "FACE": TrackingFlag.FACE,
    "FULL_BODY": TrackingFlag.FULL_BODY,
    "UPPER_BODY": TrackingFlag.UPPER_BODY
});

@freezed
class UnityMessage with _$UnityMessage {
    const factory UnityMessage({
        required String data,
        required ErrorCode errorCode,
        required String errorMsg,
        required String sessionId,
        required UnityMessageType type,
    }) = _UnityMessage;

    factory UnityMessage.fromJson(Map<String, dynamic> json) => _$UnityMessageFromJson(json);
}

enum UnityMessageType {
    GENERAL_STATUS,
    GENERATED_AIGC,
    HIDE_LOADING,
    LOGIN_ONSHOW,
    RECORD_STATE,
    REQUEST_ACCESS_TOKEN,
    SHOW_LOADING,
    SHOW_TOAST,
    SWITCHED_TO_COCREATE_PAGE,
    SWITCHED_TO_REEL_PAGE,
    SWITCHED_TO_SOCIAL_LOBBY_PAGE,
    SWITCHED_TO_SPACE,
    TO_AVATAR_EDIT,
    TRACKING_STATE,
    UNITY
}

final unityMessageTypeValues = EnumValues({
    "GENERAL_STATUS": UnityMessageType.GENERAL_STATUS,
    "GENERATED_AIGC": UnityMessageType.GENERATED_AIGC,
    "HIDE_LOADING": UnityMessageType.HIDE_LOADING,
    "LOGIN_ONSHOW": UnityMessageType.LOGIN_ONSHOW,
    "RECORD_STATE": UnityMessageType.RECORD_STATE,
    "REQUEST_ACCESS_TOKEN": UnityMessageType.REQUEST_ACCESS_TOKEN,
    "SHOW_LOADING": UnityMessageType.SHOW_LOADING,
    "SHOW_TOAST": UnityMessageType.SHOW_TOAST,
    "SWITCHED_TO_COCREATE_PAGE": UnityMessageType.SWITCHED_TO_COCREATE_PAGE,
    "SWITCHED_TO_REEL_PAGE": UnityMessageType.SWITCHED_TO_REEL_PAGE,
    "SWITCHED_TO_SOCIAL_LOBBY_PAGE": UnityMessageType.SWITCHED_TO_SOCIAL_LOBBY_PAGE,
    "SWITCHED_TO_SPACE": UnityMessageType.SWITCHED_TO_SPACE,
    "TO_AVATAR_EDIT": UnityMessageType.TO_AVATAR_EDIT,
    "TRACKING_STATE": UnityMessageType.TRACKING_STATE,
    "UNITY": UnityMessageType.UNITY
});

class EnumValues<T> {
    Map<String, T> map;
    late Map<T, String> reverseMap;

    EnumValues(this.map);

    Map<T, String> get reverse {
        reverseMap = map.map((k, v) => MapEntry(v, k));
        return reverseMap;
    }
}
