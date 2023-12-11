//
//  WebAuth.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/27.
//

import Foundation
import UIKit

public class WebAuth: NSObject {
    let TAG = "[XrAuth]AuthService %@"
    private let AUTH_ENDPOINT = "/oidc/auth"
    private let TOKEN_ENDPOINT = "/oidc/token"
    private let LOGOUT_ENDPOINT = "/login/profile/logout"
    private let SCOPE = "openid offline_access profile email username"
    private let CALLBACK_URL_SCHEME = "xrspace"

    private var session: ASWebAuthenticationSession?

    func signIn(in window: UIWindow, idp: AuthIDP, dispatcher: Dispatcher) {
        NSLog(TAG, "Sign in")
        retrieveAuthCode(in: window, idp: idp) { codeResult in
            switch codeResult {
            case let .failure(e):
                dispatcher.onError(error: e)
            case let .success(code):
                self.codeToToken(code: code, idp: idp) { tokenResult in
                    switch tokenResult {
                    case let .failure(tokenError):
                        dispatcher.onError(error: tokenError)
                    case let .success(resultSring):
                        dispatcher.onSuccess(result: resultSring)
                    }
                }
            }
        }
    }

    func signOut(in window: UIWindow, idp: AuthIDP, dispatcher: Dispatcher) {
        NSLog(TAG, "Sign out")
        let requestURL = "\(idp.domain)\(LOGOUT_ENDPOINT)"
        var urlComponents = URLComponents(string: requestURL)!
        urlComponents.queryItems = [
            URLQueryItem(name: "redirect_uri", value: idp.redirectUri),
        ]

        let url = urlComponents.url!
        session = ASWebAuthenticationSession(url: url, callbackURLScheme: CALLBACK_URL_SCHEME) { [weak self] _, error in
            self?.session = nil
            NSLog("Sign out web page is closed. error=\(error.debugDescription)")
        }
        session?.prefersEphemeralWebBrowserSession = true // No cookies
        session?.presentationContextProvider = window
        session?.start()

        dispatcher.onSuccess(result: "Sign out success")
    }
}

private extension WebAuth {
    /// To open web page and retrieve authorization_code after user logged in
    func retrieveAuthCode(in window: UIWindow, idp: AuthIDP,
                          callback: @escaping (Callback<String, AuthError>) -> Void)
    {
        NSLog(TAG, "Opening login page...")
        var urlComponents = URLComponents(string: idp.domain)
        urlComponents?.path = AUTH_ENDPOINT
        urlComponents?.queryItems = [
            URLQueryItem(name: "protocol", value: "oidc"),
            URLQueryItem(name: "response_type", value: "code"),
            URLQueryItem(name: "client_id", value: idp.clientId),
            URLQueryItem(name: "redirect_uri", value: idp.redirectUri),
            URLQueryItem(name: "scope", value: SCOPE),
        ]

        guard let requestURL = urlComponents?.url else {
            NSLog(TAG, "URL of sign in web page is not valid")
            return callback(.failure(AuthError.AuthCodeFailed))
        }

        session = ASWebAuthenticationSession(url: requestURL, callbackURLScheme: CALLBACK_URL_SCHEME) { [weak self] responseURL, error in
            self?.session = nil
            let tag = "[XrAuth]Webpage %@"
            guard error == nil else {
                NSLog(tag, "Failed to retrieve authorization code. Error: \(error.debugDescription)")

                // DON'T callback immediatilly. Wait for half second for view controller ready.
                DispatchQueue.main.asyncAfter(deadline: .now() + 0.5) {
                    callback(.failure(AuthError.UserCancel))
                }
                return
            }

            guard let url = responseURL else {
                NSLog(tag, "Responsed url error.")
                return callback(.failure(AuthError.AuthCodeFailed))
            }

            guard let queryItems = URLComponents(url: url, resolvingAgainstBaseURL: false)?.queryItems else {
                NSLog(tag, "Responsed query item error.")
                return callback(.failure(AuthError.AuthCodeFailed))
            }

            // get code from response url parameter
            guard let code = queryItems.first(where: { $0.name == "code" })?.value else {
                NSLog(tag, "Responsed code error. ")
                return callback(.failure(AuthError.AuthCodeFailed))
            }

            callback(.success(code))
        }
        // https://developer.apple.com/documentation/authenticationservices/aswebauthenticationsession
        // Use temp session and it won't keep credentials in cookies.
        session?.prefersEphemeralWebBrowserSession = true
        session?.presentationContextProvider = window
        session?.start()
    }

    /// Take auth code to retrieve credentials
    func codeToToken(code: String, idp: AuthIDP,
                     callback: @escaping (Callback<String, AuthError>) -> Void)
    {
        var params = URLComponents()
        params.queryItems = [
            URLQueryItem(name: "code", value: code),
            URLQueryItem(name: "client_id", value: idp.clientId),
            URLQueryItem(name: "grant_type", value: "authorization_code"),
            URLQueryItem(name: "response_type", value: "token id_token"),
            URLQueryItem(name: "redirect_uri", value: idp.redirectUri),
        ]

        guard let postData = params.query?.data(using: .utf8) else {
            NSLog(TAG, "Generate form body fail.")
            return callback(.failure(AuthError.AuthTokenFailed))
        }

        RestfulAPI()
            .domain(idp.domain)
            .path(TOKEN_ENDPOINT)
            .formBody(postData)
            .httpMethod(HttpMethod.POST)
            .contentType(RestfulAPI.CONTENT_TYPE_X_FORM)
            .retry(times: 1)
            .enqueue { result in
                switch result {
                case let .success(data):
                    guard let data = data,
                          let resultString = String(data: data, encoding: .utf8)
                    else {
                        NSLog(self.TAG, "Failed to parse token result.")
                        return callback(.failure(AuthError.AuthTokenFailed))
                    }

                    callback(.success(resultString))
                case let .failure(error):
                    NSLog(self.TAG, "Exchange code to credentials fail: (\(error.code)) \(error.desc)")
                    callback(.failure(error))
                }
            }
    }
}

@objc extension UIWindow: ASWebAuthenticationPresentationContextProviding {
    public func presentationAnchor(for _: ASWebAuthenticationSession) -> ASPresentationAnchor {
        self
    }
}
