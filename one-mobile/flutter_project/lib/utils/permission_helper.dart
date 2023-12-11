import 'package:permission_handler/permission_handler.dart';

class PermissionHelper {
  static Future<bool> checkPermission(Permission permission) async {
    PermissionStatus status = await permission.status;
    switch (status) {
      case PermissionStatus.granted:
        return true;
      case PermissionStatus.permanentlyDenied:
      case PermissionStatus.restricted:
        await openAppSettings();
        break;
      case PermissionStatus.denied:
      case PermissionStatus.limited:
      default:
        await _requestPermission(permission);
        break;
    }
    return await permission.status == PermissionStatus.granted;
  }

  static Future _requestPermission(Permission permission) async {
    PermissionStatus status = await permission.request();
    if (status.isPermanentlyDenied) {
      await openAppSettings();
    }
  }
}
