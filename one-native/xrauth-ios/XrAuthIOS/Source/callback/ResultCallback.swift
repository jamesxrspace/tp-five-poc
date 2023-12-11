//
//  ResultCallback.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

import Foundation

@objc public protocol ResultListener {
    func onSuccess(requestType: Int, result: String)
    func onFailure(requestType: Int, errorCode: Int, message: String)
}

public class ResultCallback: ResultListener {
    let callback: (Callback<String, AuthError>) -> Void

    public init(_ callback: @escaping (Callback<String, AuthError>) -> Void) {
        self.callback = callback
    }

    public func onSuccess(requestType _: Int, result: String) {
        callback(.success(result))
    }

    public func onFailure(requestType _: Int, errorCode: Int, message _: String) {
        callback(.failure(AuthError.lookup(code: errorCode)))
    }
}

@objc public class Dispatcher: NSObject {
    private var listener: ResultListener
    private var requestType: Int

    public init(typeCode: Int, listener: ResultListener) {
        requestType = typeCode
        self.listener = listener
    }

    public convenience init(requestType: RequestType, listener: ResultListener) {
        self.init(typeCode: requestType.code, listener: listener)
    }

    public func onSuccess(result: String) {
        listener.onSuccess(requestType: requestType, result: result)
    }

    public func onFailure(errorCode: Int, message: String) {
        listener.onFailure(requestType: requestType, errorCode: errorCode, message: message)
    }

    public func onError(error: AuthError, extra: String = "") {
        if extra.isEmpty {
            onFailure(errorCode: error.code, message: error.desc)
        } else {
            onFailure(errorCode: error.code, message: extra)
        }
    }
}
