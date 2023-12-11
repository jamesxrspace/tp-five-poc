import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';

late final StateProviderRef recordStateRef;
final recordStateType = StateProvider<RecordStateType>((ref) {
  recordStateRef = ref;
  return RecordStateType.PRESET;
});

void setRecordType(RecordStateType type) {
  recordStateRef.read(recordStateType.notifier).state = type;
}
