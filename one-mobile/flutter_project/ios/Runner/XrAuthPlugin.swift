//
//  XrAuthPlugin.swift
//  Runner
//
//  Created by XRSpace on 2023/10/4.
//

import Flutter
import Foundation
import XrAuthIOS

public class XrAuthPlugin: NSObject {
    static let METHOD_CHANNEL_NAME = "com.xrspace.xrauth.plugin"
    let TAG = "[XrAuth]XrAuthPlugin %@"
    let xrAuth = XrAuth.instance

    /// A method call handler for XrAuthPlugin
    lazy var handler: FlutterMethodCallHandler = { call, result in
        self.handle(call, result)
    }

    /// Handle method call from Flutter
    func handle(_ call: FlutterMethodCall, _ callback: @escaping FlutterResult) {
        NSLog(TAG, "method \(call.method) called")
        guard let arguments = call.arguments as? [String: Any] else {
            NSLog(TAG, "Failed to \(call.method) with empty arguments")
            return callback(AuthError.RequestFailed.asFlutterError())
        }

        switch call.method {
        case "login":
            guard let clientId = arguments["clientId"] as? String,
                  let domain = arguments["domain"] as? String,
                  let redirectUri = arguments["redirectUri"] as? String
            else {
                NSLog(TAG, "Login cannot be performed due to invalid parameter(s)")
                return callback(AuthError.RequestFailed.asFlutterError())
            }

            xrAuth.login(clientId: clientId, domain: domain, redirectUri: redirectUri, callback: ResultCallback { result in
                switch result {
                case let .success(token):
                    NSLog(self.TAG, "login success")
                    callback(token)
                case let .failure(error):
                    NSLog(self.TAG, "login failed: (\(error.code)) \(error.desc)")
                    callback(error.asFlutterError())
                }
            })
        case "logout":
            guard let clientId = arguments["clientId"] as? String,
                  let domain = arguments["domain"] as? String,
                  let redirectUri = arguments["redirectUri"] as? String
            else {
                NSLog(TAG, "Logout is not possible due to invalid parameter(s)")
                return callback(AuthError.RequestFailed.asFlutterError())
            }

            xrAuth.logout(clientId: clientId, domain: domain, redirectUri: redirectUri, callback: ResultCallback { result in
                switch result {
                case let .success(success):
                    NSLog(self.TAG, "logout success")
                    callback("logout success? \(success)")
                case let .failure(error):
                    NSLog(self.TAG, "logout failed with error: \(error)")
                    callback(error.asFlutterError())
                }
            })
        case "saveData":
            guard let key = arguments["key"] as? String,
                  let value = arguments["value"] as? String
            else {
                NSLog(TAG, "Saving failed due to invalid parameter(s)")
                return callback(AuthError.RequestFailed.asFlutterError())
            }

            xrAuth.saveInfo(key: key, value: value, callback: ResultCallback { result in
                switch result {
                case .success:
                    callback(true)
                case let .failure(error):
                    NSLog(self.TAG, "Failed to save: (\(error.code)) \(error.desc)")
                    callback(error.asFlutterError())
                }
            })
        case "readData":
            guard let key = arguments["key"] as? String
            else {
                NSLog(TAG, "Reading failed due to invalid parameter(s)")
                return callback(AuthError.RequestFailed.asFlutterError())
            }

            xrAuth.readInfo(key: key, callback: ResultCallback { result in
                switch result {
                case let .success(value):
                    callback(value)
                case let .failure(error):
                    NSLog(self.TAG, "Failed to read: (\(error.code)) \(error.desc)")
                    callback(error.asFlutterError())
                }
            })
        case "deleteData":
            guard let key = arguments["key"] as? String
            else {
                NSLog(TAG, "Deleting failed due to invalid parameter(s)")
                return callback(AuthError.RequestFailed.asFlutterError())
            }

            xrAuth.deleteInfo(key: key, callback: ResultCallback { result in
                switch result {
                case .success:
                    callback(true)
                case let .failure(error):
                    NSLog(self.TAG, "Failed to delete: (\(error.code)) \(error.desc)")
                    callback(error.asFlutterError())
                }
            })
        default:
            NSLog(TAG, "The method call [\(call.method)] is not supported")
            callback(AuthError.FunctionNotSupported.asFlutterError())
        }
    }
}

/// An extend of  AuthError. Create a new method, which can convert itself to FlutterError
extension AuthError {
    func asFlutterError() -> FlutterError {
        FlutterError(code: String(code), message: desc, details: nil)
    }
}
