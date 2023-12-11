//
//  AuthError.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

import Foundation

public enum AuthError: Error, CaseIterable {
    case None
    case FunctionNotSupported
    case FunctionNotImplemented
    case SaveDataFailed
    case ReadDataFailed
    case DeleteDataFailed
    case UserCancel
    case AuthCodeFailed
    case AuthTokenFailed
    case Network
    case RequestTimeout
    case RequestFailed
    case UnknownError

    public var code: Int {
        switch self {
        case .None:
            return 0
        case .FunctionNotSupported:
            return 2001
        case .FunctionNotImplemented:
            return 2002
        case .SaveDataFailed:
            return 2011
        case .ReadDataFailed:
            return 2012
        case .DeleteDataFailed:
            return 2013
        case .UserCancel:
            return 2111
        case .AuthCodeFailed:
            return 2112
        case .AuthTokenFailed:
            return 2113
        case .Network:
            return 2301
        case .RequestTimeout:
            return 2302
        case .RequestFailed:
            return 2303
        case .UnknownError:
            return 2999
        }
    }

    public var desc: String {
        switch self {
        case .None:
            return "None"
        case .FunctionNotSupported:
            return "Function Not Supported"
        case .FunctionNotImplemented:
            return "Function Not implemented"
        case .SaveDataFailed:
            return "Save data failed"
        case .ReadDataFailed:
            return "Read data failed"
        case .DeleteDataFailed:
            return "Delete data failed"
        case .UserCancel:
            return "User cancel login"
        case .AuthCodeFailed:
            return "Failed to get auth code during login"
        case .AuthTokenFailed:
            return "Fail to exchange access_token with authorization code"
        case .Network:
            return "Network error"
        case .RequestTimeout:
            return "Request timeout"
        case .RequestFailed:
            return "Request failed"
        case .UnknownError:
            return "Unknown error"
        }
    }

    public static func lookup(code: Int) -> AuthError {
        for error in AuthError.allCases {
            if error.code == code {
                return error
            }
        }
        return UnknownError
    }
}
