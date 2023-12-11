import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:get_it/get_it.dart';
import 'package:tpfive/feature/unity_holder/unity_message_mixin.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';
import 'package:tpfive/services/prefs_service.dart';

class PrefsViewModel with UnityMessageMixin {
  final WidgetRef _ref;

  late PrefsService _prefsService;

  PrefsViewModel(WidgetRef ref) : _ref = ref {
    initUnityMessageProvider(_ref);
    init();
  }

  void init() {
    _prefsService = GetIt.I<PrefsService>();
  }

  Map<String, Prefs> getPrefs() {
    return _prefsService.prefsData.prefs;
  }

  void set(String key, dynamic value) {
    if (_prefsService.prefsData.prefs[key]!.value == value) {
      return;
    }

    _prefsService.set(key, value);
    postMessageToUnity(
        FlutterMessageType.PREFS, prefsDataToJson(_prefsService.prefsData));
  }
}
