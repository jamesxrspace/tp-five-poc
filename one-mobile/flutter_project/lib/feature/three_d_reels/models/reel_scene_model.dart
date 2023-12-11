class ReelSceneModel {
  String? sceneName;
  String? bundleId;
  String? thumbNail;
  String? participantLimit;
  String? description;

  ReelSceneModel(
      {this.sceneName,
      this.bundleId,
      this.thumbNail,
      this.participantLimit,
      this.description});

  ReelSceneModel.fromJson(Map<String, dynamic> json) {
    sceneName = json['sceneName'];
    bundleId = json['bundleId'];
    thumbNail = json['thumbNail'];
    participantLimit = json['participantLimit'];
    description = json['description'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = <String, dynamic>{};
    data['sceneName'] = sceneName;
    data['bundleId'] = bundleId;
    data['thumbNail'] = thumbNail;
    data['participantLimit'] = participantLimit;
    data['description'] = description;
    return data;
  }
}
