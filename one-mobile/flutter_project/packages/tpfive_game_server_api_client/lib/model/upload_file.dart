//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class UploadFile {
  /// Returns a new [UploadFile] instance.
  UploadFile({
    required this.fileId,
    required this.contentType,
    required this.contentLength,
    required this.checksum,
  });

  /// unique file id(file name)
  String fileId;

  /// file content type
  String contentType;

  /// file content length(bytes)
  int contentLength;

  /// file checksum(sha256 base64 encoded)
  String checksum;

  @override
  bool operator ==(Object other) => identical(this, other) || other is UploadFile &&
     other.fileId == fileId &&
     other.contentType == contentType &&
     other.contentLength == contentLength &&
     other.checksum == checksum;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (fileId.hashCode) +
    (contentType.hashCode) +
    (contentLength.hashCode) +
    (checksum.hashCode);

  @override
  String toString() => 'UploadFile[fileId=$fileId, contentType=$contentType, contentLength=$contentLength, checksum=$checksum]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
      json[r'file_id'] = this.fileId;
      json[r'content_type'] = this.contentType;
      json[r'content_length'] = this.contentLength;
      json[r'checksum'] = this.checksum;
    return json;
  }

  /// Returns a new [UploadFile] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static UploadFile? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "UploadFile[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "UploadFile[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return UploadFile(
        fileId: mapValueOfType<String>(json, r'file_id')!,
        contentType: mapValueOfType<String>(json, r'content_type')!,
        contentLength: mapValueOfType<int>(json, r'content_length')!,
        checksum: mapValueOfType<String>(json, r'checksum')!,
      );
    }
    return null;
  }

  static List<UploadFile> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <UploadFile>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = UploadFile.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, UploadFile> mapFromJson(dynamic json) {
    final map = <String, UploadFile>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = UploadFile.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of UploadFile-objects as value to a dart map
  static Map<String, List<UploadFile>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<UploadFile>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = UploadFile.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'file_id',
    'content_type',
    'content_length',
    'checksum',
  };
}

