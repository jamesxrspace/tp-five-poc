//
//  XrStorage.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

import Foundation
import SimpleKeychain

public class XrStorage {
    private var keychain: SimpleKeychain

    public init(_ keychain: SimpleKeychain) {
        self.keychain = keychain
        newInstallResetKeychain()
    }

    public func store(key: String, value: String) throws {
        try keychain.set(value, forKey: key)
    }

    public func retrieveString(_ key: String) throws -> String {
        try keychain.string(forKey: key)
    }

    public func remove(_ key: String) throws {
        try keychain.deleteItem(forKey: key)
    }

    public func exist(_ key: String) throws -> Bool {
        try keychain.hasItem(forKey: key)
    }

    private func clear() {
        try? keychain.deleteAll()
    }

    private func newInstallResetKeychain() {
        let key = "installed"
        let userDefaults = UserDefaults.standard
        let isInstalled = userDefaults.bool(forKey: key)
        if !isInstalled {
            try? keychain.deleteAll()
            userDefaults.set(true, forKey: key)
        }
    }
}
