import Flutter
import UIKit
import flutter_unity_widget

@UIApplicationMain
@objc class AppDelegate: FlutterAppDelegate {
    override func application(
        _ application: UIApplication,
        didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
    ) -> Bool {
        InitUnityIntegrationWithOptions(argc: CommandLine.argc, argv: CommandLine.unsafeArgv, launchOptions)
        handleAuthMethodCall()
        GeneratedPluginRegistrant.register(with: self)
        return super.application(application, didFinishLaunchingWithOptions: launchOptions)
    }

    /// Creating MethodChannel in XrAuthPlugin.swift will cause a circular reference issue,
    /// so we create it in AppDelegate
    func handleAuthMethodCall() {
        guard let controller = window?.rootViewController as? FlutterViewController
        else {
            NSLog("[XrAuth] No root view controller to create method channel")
            return
        }

        let channel = FlutterMethodChannel(name: XrAuthPlugin.METHOD_CHANNEL_NAME,
                                           binaryMessenger: controller.binaryMessenger)
        channel.setMethodCallHandler(XrAuthPlugin().handler)
    }
}
