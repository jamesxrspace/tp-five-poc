import 'dart:convert';

class Guest {
  String email;
  String password;
  String username;
  String nickname;
  String userId;

  Guest({
    required this.email,
    required this.password,
    required this.username,
    required this.nickname,
    required this.userId,
  });

  static Guest parse(String str) => Guest.fromJson(json.decode(str));

  static String toJsonString(Guest data) => json.encode(data.toJson());

  factory Guest.fromJson(Map<String, dynamic> json) => Guest(
        email: json['email'],
        password: json['password'],
        username: json['username'],
        nickname: json['nickname'],
        userId: json['user_id'],
      );

  Map<String, dynamic> toJson() => {
        'email': email,
        'password': password,
        'username': username,
        'nickname': nickname,
        'user_id': userId,
      };
}
