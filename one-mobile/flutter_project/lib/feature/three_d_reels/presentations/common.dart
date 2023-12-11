import 'package:flutter/material.dart';
import 'package:tpfive/constants/style.dart';
import 'package:tpfive/generated/freezed/generate_model.dart';

Color computeTrackingIconColor(bool enable, TrackingStateType state) {
  if (!enable) {
    return Colors.white;
  }

  return switch (state) {
    TrackingStateType.INITIATING => Colors.white.withOpacity(0.5),
    TrackingStateType.TRACKING => ThemeColors.mainColor,
    TrackingStateType.DETECTING => Colors.red.withOpacity(0.6),
  };
}
