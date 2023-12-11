import 'package:tpfive/feature/launch_unity/launch_unity.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:tpfive/services/unity_message_service.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/test/common.dart';

void main() {
  testWidgets('LaunchUnity should go to avatar edit without exception',
      (tester) async {
    final UnityMessageService unityMessageService = UnityMessageService();
    await tester.pumpWidget(createTestWidget(
        const LaunchUnity(messageKey: 'foo'), unityMessageService));

    var unityMessage = const UnityMessage(
        data: 'foo',
        sessionId: '',
        type: UnityMessageType.TO_AVATAR_EDIT,
        errorCode: ErrorCode.SUCCESS,
        errorMsg: '');
    unityMessageService.callback('foo', unityMessage);

    // No exception should be thrown.
    await tester.pumpAndSettle();
  });
}
