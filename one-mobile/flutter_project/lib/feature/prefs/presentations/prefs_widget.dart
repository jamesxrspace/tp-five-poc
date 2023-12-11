import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:tpfive/constants/tp_five_icons.dart';
import 'package:tpfive/feature/prefs/models/pref_view_model.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';

class PrefsWidget extends ConsumerStatefulWidget {
  const PrefsWidget({super.key});

  static const routeName = '/setting';
  static const tag = 'Setting';

  @override
  PrefsState createState() => PrefsState();
}

class PrefsState extends ConsumerState<PrefsWidget> {
  final ScrollController _listViewController = ScrollController();
  late PrefsViewModel prefsViewModel;

  @override
  void initState() {
    super.initState();
    prefsViewModel = PrefsViewModel(ref);
  }

  @override
  Widget build(BuildContext context) {
    final prefsData = prefsViewModel.getPrefs();

    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
          child: Column(children: [
        const SizedBox(height: 20),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            ElevatedButton(
              onPressed: () {
                Navigator.pop(context);
              },
              style: ElevatedButton.styleFrom(
                shape: const CircleBorder(),
                padding: const EdgeInsets.all(5),
                backgroundColor: Colors.transparent,
                foregroundColor: Colors.transparent,
                shadowColor: Colors.transparent,
              ),
              child: const Icon(
                TPFiveIcon.arrowLeft,
                color: Colors.black,
                size: 20,
              ),
            ),
            Expanded(
              child: Center(
                child: Text(
                  'Prefs',
                  style: GoogleFonts.poppins(
                    textStyle: const TextStyle(
                      fontSize: 23,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                ),
              ),
            ),
            const SizedBox(width: 64)
          ],
        ),
        const SizedBox(height: 15),
        Expanded(
          child: ListView.builder(
            controller: _listViewController,
            itemCount: prefsData.length,
            itemBuilder: (context, index) {
              var key = prefsData.keys.elementAt(index);
              return Padding(
                padding: const EdgeInsets.all(10.0),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(key),
                    const Spacer(),
                    setPrefsUI(key, prefsData[key]!),
                  ],
                ),
              );
            },
          ),
        )
      ])),
    );
  }

  Widget setPrefsUI(String key, Prefs prefs) {
    if (prefs.value is bool) {
      return getSwitch(key, prefs.value);
    } else if (prefs.value is String) {
      var controller = TextEditingController();
      controller.text = prefs.value;
      return getInputText(key, prefs.value, controller);
    }

    throw Exception();
  }

  Widget getSwitch(String key, bool value) {
    return Switch(
      value: value,
      onChanged: (value) {
        setState(() {
          prefsViewModel.set(key, value);
        });
      },
    );
  }

  Widget getInputText(
      String key, String value, TextEditingController controller) {
    return FocusScope(
      onFocusChange: (value) {
        if (value) {
          prefsViewModel.set(key, controller.text);
        }
      },
      child: SizedBox(
        width: 150,
        child: TextField(
          controller: controller,
          decoration: const InputDecoration(
            border: OutlineInputBorder(),
            hintText: 'Enter prefs text',
          ),
        ),
      ),
    );
  }
}
