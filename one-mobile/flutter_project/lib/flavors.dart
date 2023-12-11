enum Flavor {
  dev,
  prod,
  qa,
}

class F {
  static Flavor? appFlavor;

  static String get title {
    switch (appFlavor) {
      case Flavor.dev:
        return 'TPFive(Dev)';
      case Flavor.prod:
        return 'TPFive';
      case Flavor.qa:
        return 'TPFive(qa)';
      default:
        throw Exception();
    }
  }

  static String get prefsFilePath {
    switch (appFlavor) {
      case Flavor.dev:
        return 'assets/json/prefs_dev.json';
      case Flavor.prod:
        return 'assets/json/prefs_prod.json';
      case Flavor.qa:
        return 'assets/json/prefs_qa.json';
      default:
        throw Exception();
    }
  }
}
