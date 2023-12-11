import 'package:tpfive/feature/three_d_reels/presentations/three_d_reel_recording_page.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:tpfive/services/unity_message_service.dart';
import 'package:tpfive/test/common.dart';

void main() {
  testWidgets('ThreeDReelRecordingPage should create without exception',
      (tester) async {
    final UnityMessageService unityMessageService = UnityMessageService();
    await tester.pumpWidget(createTestWidget(
        const ThreeDReelRecordingPage(messageKey: 'foo'), unityMessageService));

    // No exception should be thrown.
  });
}
