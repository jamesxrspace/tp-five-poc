//
//  RequestType.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

import Foundation

public enum RequestType {
    case LOGIN
    case LOGOUT
    case SAVE_INFO
    case READ_INFO
    case DELETE_INFO
    case UNKNOWN

    public var code: Int {
        switch self {
        case .LOGIN: return 1
        case .LOGOUT: return 2
        case .SAVE_INFO: return 3
        case .READ_INFO: return 4
        case .DELETE_INFO: return 5
        case .UNKNOWN: return 99
        }
    }

    public static func lookup(code: Int) -> RequestType {
        switch code {
        case 1: return .LOGIN
        case 2: return .LOGOUT
        case 3: return .SAVE_INFO
        case 4: return .READ_INFO
        case 5: return .DELETE_INFO
        default: return .UNKNOWN
        }
    }
}
