class SwitchOption {
  bool _mode = false;
  bool get mode => _mode;

  SwitchOption({
    required bool mode,
  }) {
    _mode = mode;
  }

  SwitchOption.fromJson(dynamic json) {
    _mode = json['mode'];
  }

  SwitchOption copyWith({
    required bool mode,
  }) =>
      SwitchOption(
        mode: mode,
      );

  Map<String, dynamic> toJson() {
    final map = <String, dynamic>{};
    map['mode'] = _mode;
    return map;
  }
}
