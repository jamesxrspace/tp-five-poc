import 'dart:convert';

import 'package:flutter/services.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:tpfive/flavors.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';

class PrefsService {
  static const String prefsKey = 'settings';

  late final SharedPreferences sharePrefs;
  late PrefsData prefsData;

  Future<void> init() async {
    var [defaultPrefs, userDefinedPrefs] =
        await Future.wait([_loadFromConfigureFile(), _loadFromSharedPrefs()]);

    prefsData = PrefsData(prefs: {
      ...defaultPrefs.prefs,
      ...Map.from(userDefinedPrefs.prefs)
        ..removeWhere((key, value) => !defaultPrefs.prefs.containsKey(key)),
    });
  }

  Future<PrefsData> _loadFromConfigureFile() async {
    final bytes = await rootBundle.load(F.prefsFilePath);
    return prefsDataFromJson(
      utf8.decode(
        bytes.buffer.asUint8List(bytes.offsetInBytes, bytes.lengthInBytes),
      ),
    );
  }

  Future<PrefsData> _loadFromSharedPrefs() async {
    sharePrefs = await SharedPreferences.getInstance();
    var json = sharePrefs.getString(prefsKey);
    return json != null && json.isNotEmpty
        ? prefsDataFromJson(json)
        : const PrefsData(prefs: {});
  }

  void set(String key, dynamic value) {
    Map<String, Prefs> prefsMap = Map.from(prefsData.prefs);
    prefsMap[key] = Prefs(
        description: prefsMap[key]!.description,
        name: prefsMap[key]!.name,
        value: value);
    prefsData = PrefsData(prefs: prefsMap);

    sharePrefs.setString(prefsKey, prefsDataToJson(prefsData));
  }
}
