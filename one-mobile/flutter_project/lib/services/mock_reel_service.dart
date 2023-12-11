import 'dart:async';
import 'dart:convert';

import 'package:flutter/services.dart';

class MockReelService {
  static const String TAG = 'MockService';
  static const _jsonDir = 'assets/json/';
  static const _jsonExtension = '.json';

  MockReelService();
  Future onRequest(RequestOptions options) async {
    final resourcePath = _jsonDir + options.path + _jsonExtension;
    final data = await rootBundle.load(resourcePath);
    final map = json.decode(
      utf8.decode(
        data.buffer.asUint8List(data.offsetInBytes, data.lengthInBytes),
      ),
    );

    return Response(
      data: map,
      statusCode: 200,
    );
  }
}

class RequestOptions {
  String path = '';

  RequestOptions({required this.path});
}

class Response {
  dynamic data;
  int statusCode = 200;

  Response({required this.data, required this.statusCode});
}
