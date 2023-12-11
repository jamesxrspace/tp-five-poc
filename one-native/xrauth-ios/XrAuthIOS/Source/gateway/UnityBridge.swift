//
//  UnityBridge.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

import Foundation

@objc public protocol UnityProtocol: AnyObject {
    func login(clientId: String, domain: String, redirectUri: String, callback: ResultListener)
    func logout(clientId: String, domain: String, redirectUri: String, callback: ResultListener)
    func saveInfo(key: String, value: String, callback: ResultListener)
    func readInfo(key: String, callback: ResultListener)
    func deleteInfo(key: String, callback: ResultListener)
}

@objc public class UnityCallback: NSObject, ResultListener {
    let TAG = "[XrAuth]UnityCallback %@"
    public func onSuccess(requestType _: Int, result: String) { // TODO: 取消 requesrType
        guard let unityCallback = XrAuth.xrAuthCallback else {
            NSLog(TAG, "Unity did not pass callbac to XrAuth to know request success")
            return
        }

        unityCallback(Int32(AuthError.None.code), result)
    }

    public func onFailure(requestType _: Int, errorCode: Int, message: String) { // TODO: 取消 requesrType
        guard let unityCallback = XrAuth.xrAuthCallback else {
            NSLog(TAG, "Unity did not pass callback to XrAuth to know request failed")
            return
        }

        unityCallback(Int32(errorCode), message)
    }
}
