//
//  XrAuth.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

import Foundation
import SimpleKeychain

@objc public class XrAuth: NSObject {
    /// THE callback object that define in XrAuthIOS.h
    @objc
    public static var xrAuthCallback: RequestXrAuthCallback?

    @objc
    public static let instance: XrAuth = {
        let this = XrAuth()
        return this
    }()

    private let TAG = "[XrAuth]XrAuth %@"
    private let storage = XrStorage(SimpleKeychain())
    private let authService = WebAuth()
}

@objc extension XrAuth: UnityProtocol {
    public func login(clientId: String, domain: String, redirectUri: String, callback: ResultListener) {
        NSLog(TAG, "GO login: \(clientId), \(domain), \(redirectUri)")
        let dispatcher = Dispatcher(requestType: RequestType.LOGIN, listener: callback)

        guard let window = getUIWindow() else {
            let errorMessage = "Cannot obtain key window to open to let user sign in."
            NSLog(TAG, errorMessage)
            return dispatcher.onError(error: AuthError.RequestFailed, extra: errorMessage)
        }

        let idp = AuthIDP(clientId: clientId, domain: domain, redirectUri: redirectUri)
        authService.signIn(in: window, idp: idp, dispatcher: dispatcher)
    }

    public func logout(clientId: String, domain: String, redirectUri: String, callback: ResultListener) {
        NSLog(TAG, "GO logout")
        let dispatcher = Dispatcher(requestType: RequestType.LOGOUT, listener: callback)

        guard let window = getUIWindow() else {
            let errorMessage = "Cannot obtain key window to open to let user sign out."
            NSLog(TAG, errorMessage)
            return dispatcher.onError(error: AuthError.RequestFailed, extra: errorMessage)
        }

        let idp = AuthIDP(clientId: clientId, domain: domain, redirectUri: redirectUri)
        authService.signOut(in: window, idp: idp, dispatcher: dispatcher)
    }

    public func saveInfo(key: String, value: String, callback: ResultListener) {
        let dispatcher = Dispatcher(requestType: RequestType.SAVE_INFO, listener: callback)

        guard let _ = try? storage.store(key: key, value: value) else {
            return dispatcher.onError(error: AuthError.SaveDataFailed)
        }

        dispatcher.onSuccess(result: "saved")
    }

    public func readInfo(key: String, callback: ResultListener) {
        let dispatcher = Dispatcher(requestType: RequestType.READ_INFO, listener: callback)

        guard let hasItem = try? storage.exist(key),
              hasItem == true,
              let result = try? storage.retrieveString(key)
        else {
            return dispatcher.onError(error: AuthError.ReadDataFailed)
        }

        dispatcher.onSuccess(result: result)
    }

    public func deleteInfo(key: String, callback: ResultListener) {
        let dispatcher = Dispatcher(requestType: RequestType.DELETE_INFO, listener: callback)

        guard let hasItem = try? storage.exist(key),
              hasItem == true,
              let result = try? storage.remove(key)
        else {
            return dispatcher.onSuccess(result: "not exist")
        }

        dispatcher.onSuccess(result: "\(result)")
    }
}

extension XrAuth {
    func getUIWindow() -> UIWindow? {
        UIApplication.shared.connectedScenes
            .compactMap { $0 as? UIWindowScene }
            .first(where: { $0.activationState == .foregroundActive })?
            .windows
            .first(where: { $0.isKeyWindow })
    }
}
