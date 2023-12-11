//
//  Callback.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

import Foundation

/// It's a common callback object.
public enum Callback<Success, Failure> {
    case success(Success)
    case failure(Failure)
}
