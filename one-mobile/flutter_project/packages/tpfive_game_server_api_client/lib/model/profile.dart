//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.12

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class Profile {
  /// Returns a new [Profile] instance.
  Profile({
    this.xrId,
    this.email,
    this.issuerResourceOwnerIds = const {},
    this.username,
    this.nickname,
    this.isEmailVerified,
  });

  /// xrspace user id
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? xrId;

  /// email
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? email;

  Map<String, String> issuerResourceOwnerIds;

  /// username
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? username;

  /// nickname
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? nickname;

  /// is email verified
  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  bool? isEmailVerified;

  @override
  bool operator ==(Object other) => identical(this, other) || other is Profile &&
     other.xrId == xrId &&
     other.email == email &&
     other.issuerResourceOwnerIds == issuerResourceOwnerIds &&
     other.username == username &&
     other.nickname == nickname &&
     other.isEmailVerified == isEmailVerified;

  @override
  int get hashCode =>
    // ignore: unnecessary_parenthesis
    (xrId == null ? 0 : xrId!.hashCode) +
    (email == null ? 0 : email!.hashCode) +
    (issuerResourceOwnerIds.hashCode) +
    (username == null ? 0 : username!.hashCode) +
    (nickname == null ? 0 : nickname!.hashCode) +
    (isEmailVerified == null ? 0 : isEmailVerified!.hashCode);

  @override
  String toString() => 'Profile[xrId=$xrId, email=$email, issuerResourceOwnerIds=$issuerResourceOwnerIds, username=$username, nickname=$nickname, isEmailVerified=$isEmailVerified]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.xrId != null) {
      json[r'xr_id'] = this.xrId;
    } else {
      json[r'xr_id'] = null;
    }
    if (this.email != null) {
      json[r'email'] = this.email;
    } else {
      json[r'email'] = null;
    }
      json[r'issuer_resource_owner_ids'] = this.issuerResourceOwnerIds;
    if (this.username != null) {
      json[r'username'] = this.username;
    } else {
      json[r'username'] = null;
    }
    if (this.nickname != null) {
      json[r'nickname'] = this.nickname;
    } else {
      json[r'nickname'] = null;
    }
    if (this.isEmailVerified != null) {
      json[r'is_email_verified'] = this.isEmailVerified;
    } else {
      json[r'is_email_verified'] = null;
    }
    return json;
  }

  /// Returns a new [Profile] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static Profile? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key), 'Required key "Profile[$key]" is missing from JSON.');
          assert(json[key] != null, 'Required key "Profile[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return Profile(
        xrId: mapValueOfType<String>(json, r'xr_id'),
        email: mapValueOfType<String>(json, r'email'),
        issuerResourceOwnerIds: mapCastOfType<String, String>(json, r'issuer_resource_owner_ids') ?? const {},
        username: mapValueOfType<String>(json, r'username'),
        nickname: mapValueOfType<String>(json, r'nickname'),
        isEmailVerified: mapValueOfType<bool>(json, r'is_email_verified'),
      );
    }
    return null;
  }

  static List<Profile> listFromJson(dynamic json, {bool growable = false,}) {
    final result = <Profile>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = Profile.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, Profile> mapFromJson(dynamic json) {
    final map = <String, Profile>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = Profile.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of Profile-objects as value to a dart map
  static Map<String, List<Profile>> mapListFromJson(dynamic json, {bool growable = false,}) {
    final map = <String, List<Profile>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = Profile.listFromJson(entry.value, growable: growable,);
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
  };
}

