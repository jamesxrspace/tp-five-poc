import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tpfive/main.dart';

class _LoadingState {
  final bool display;
  final _Config config;

  _LoadingState(this.display, this.config);
}

class _Config {
  final double alpha;
  final Alignment position;

  _Config(this.alpha, this.position);
}

class LoadingConfigNotifier extends StateNotifier<_LoadingState> {
  LoadingConfigNotifier()
      : super(_LoadingState(false, _Config(0.5, Alignment.center)));

  static var defaultConfig = _Config(0.5, Alignment.center);

  void show(
      {double alpha = 0.5,
      String comment = '',
      Alignment position = Alignment.center}) {
    globalProviderContainer.read(unityGeneralMessageProvider.notifier).state =
        comment;
    state = _LoadingState(true, _Config(alpha, position));
  }

  void hide() {
    state = _LoadingState(false, defaultConfig);
  }
}

final loadingStateProvider =
    StateNotifierProvider<LoadingConfigNotifier, _LoadingState>((ref) {
  return LoadingConfigNotifier();
});

final unityGeneralMessageProvider = StateProvider<String>((ref) => '');
