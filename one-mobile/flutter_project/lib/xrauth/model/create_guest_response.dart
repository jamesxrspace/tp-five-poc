import 'dart:convert';
import 'package:tpfive/xrauth/model/guest.dart';

class CreateGuestResponse {
  String status;
  Guest guestInfo;

  CreateGuestResponse({
    required this.status,
    required this.guestInfo,
  });

  static CreateGuestResponse parse(String str) =>
      CreateGuestResponse.fromJson(json.decode(str));

  static String toJsonString(CreateGuestResponse data) =>
      json.encode(data.toJson());

  factory CreateGuestResponse.fromJson(Map<String, dynamic> json) =>
      CreateGuestResponse(
          status: json['status'], guestInfo: Guest.fromJson(json['message']));

  Map<String, dynamic> toJson() => {
        'status': status,
        'message': guestInfo.toJson(),
      };
}
