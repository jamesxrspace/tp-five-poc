import 'dart:convert';

import 'package:tpfive/feature/mock_unity_widget/unity_widget.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/services/unity_message_service.dart';
import 'package:tpfive/test/common.dart';

void main() {
  testWidgets('Mock UnityWidget should post message without exception',
      (tester) async {
    final UnityMessageService unityMessageService = UnityMessageService();

    await tester.pumpWidget(createTestWidget(
        UnityWidget(
          onUnityCreated: (controller) {},
          onUnityMessage: (message) {
            UnityMessage.fromJson(jsonDecode(message));
          },
          useAndroidViewSurface: false,
          hideStatus: false,
          fullscreen: false,
        ),
        unityMessageService));

    UnityWidgetState state = tester.state(find.byType(UnityWidget));

    for (var value in FlutterMessageType.values) {
      state.postMessage('', '', '''{
        "type": "${value.name}",
        "data": "",
        "sessionId": "",
        "errorCode": "${ErrorCode.SUCCESS.name}",
        "errorMsg": ""
        }''');
    }

    // No exception should be thrown.
    await tester.pumpAndSettle();
  });
}
