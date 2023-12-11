//
//  RestfulAPI.swift
//  XrAuthIOS
//
//  Created by XRSpace on 2023/7/28.
//

import Foundation

public class RestfulAPI {
    public static let CONTENT_TYPE_JSON = "application/json"
    public static let CONTENT_TYPE_X_FORM = "application/x-www-form-urlencoded"

    private let TAG: String = "[XrAuth]RestfulAPI %@"
    private var request: URLRequest?
    private var formBody: Data?
    private var bearerToken: String?
    private var httpMethod = HttpMethod.POST
    private var domain: String?
    private var path: String?
    private var queries: [String: Any] = [:]
    private var contentType: String?
    private var timeoutAfterSeconds = 20.0
    private var currentRetryCount: Int = 0
    private var maxRetryTimes = 0

    func domain(_ domain: String) -> Self {
        self.domain = domain
        return self
    }

    func path(_ path: String) -> Self {
        self.path = path
        return self
    }

    func query(key: String, value: Any) -> Self {
        queries[key] = value
        return self
    }

    func queries(queries: [String: Any]) -> Self {
        for (key, value) in queries {
            self.queries[key] = value
        }
        return self
    }

    func formBody(_ formBody: Data) -> Self {
        self.formBody = formBody
        return self
    }

    func bearer(token: String) -> Self {
        bearerToken = token
        return self
    }

    func httpMethod(_ method: HttpMethod) -> Self {
        httpMethod = method
        return self
    }

    func contentType(_ contentType: String) -> Self {
        self.contentType = contentType
        return self
    }

    func timeout(afterSeconds: Double) -> Self {
        timeoutAfterSeconds = afterSeconds
        return self
    }

    func retry(times: Int) -> Self {
        maxRetryTimes = times
        return self
    }

    /// Make url request by the given parameters and return the origin response from server by callback
    func submit(_ callback: @escaping (Data?, URLResponse?, Error?) -> Void) {
        guard let request = makeRequest() else {
            NSLog(TAG, "Try to submit request but fail to make request url")
            callback(nil, nil, AuthError.RequestFailed)
            return
        }

        URLSession.shared.dataTask(with: request as URLRequest, completionHandler: { data, response, error in
            callback(data, response, error)
        }).resume()
    }

    /// Make url request by given parameters. It will filter the basic request/response error and return result directly.
    @available(iOS 15.0, *)
    func execute() async -> Callback<Data, AuthError> {
        guard let request = makeRequest() else {
            NSLog(TAG, "Try to execute request but fail to make request url")
            return .failure(AuthError.RequestFailed)
        }

        do {
            let (data, response) = try await URLSession.shared.data(for: request)
            guard let response = response as? HTTPURLResponse else {
                NSLog(TAG, "Response from API is not regular HTTP response")
                return .failure(AuthError.RequestFailed)
            }

            guard response.statusCode >= 200, response.statusCode < 300 else {
                NSLog(TAG, "Response is not success. Received status code: \(response.statusCode)")
                logData(data: data)
                return .failure(AuthError.RequestFailed)
            }

            return .success(data)
        } catch let urlError as URLError {
            if shouldRetry() || urlError.code.rawValue == NSURLErrorTimedOut {
                NSLog(TAG, "Request timeout. go retry...")
                return await execute()
            } else {
                NSLog(TAG, "Request failed with error: \(urlError)")
                return .failure(AuthError.RequestFailed)
            }
        } catch {
            NSLog(TAG, "Request failed with unexpected error: \(error)")
            return .failure(AuthError.Network)
        }
    }

    /// Make url request by given parameters. It will filter the basic request/response error and return result by callback.
    func enqueue(_ callback: @escaping (Callback<Data?, AuthError>) -> Void) {
        guard let request = makeRequest() else {
            NSLog(TAG, "Try to enqueue request but failed to make request url")
            callback(.failure(AuthError.RequestFailed))
            return
        }

        URLSession.shared.dataTask(with: request as URLRequest, completionHandler: { data, response, error in
            guard error == nil else {
                guard let urlError = error as? URLError else {
                    NSLog(self.TAG, "Request failed with unexpected error: \(error!)")
                    self.logData(data: data)
                    callback(.failure(AuthError.RequestFailed))
                    return
                }

                if urlError.code.rawValue == NSURLErrorTimedOut, self.shouldRetry() {
                    self.enqueue(callback)
                } else {
                    NSLog(self.TAG, "Request failed with error: \(urlError)")
                    self.logData(data: data)
                    callback(.failure(AuthError.RequestFailed))
                }
                return
            }

            guard let response = response as? HTTPURLResponse else {
                NSLog(self.TAG, "Response from API is not regular HTTP response")
                self.logData(data: data)
                callback(.failure(AuthError.RequestFailed))
                return
            }

            guard response.statusCode >= 200, response.statusCode < 300 else {
                NSLog(self.TAG, "Response is not success. Received status code: \(response.statusCode)")
                self.logData(data: data)
                callback(.failure(AuthError.RequestFailed))
                return
            }

            callback(.success(data)) // It's ok if data is empty
        })
        .resume()
    }

    /// Print data to console if it is not nil
    private func logData(data: Data?) {
        if let data = data {
            NSLog(TAG, "Response Data: \(String(data: data, encoding: .utf8) ?? "nil")")
        }
    }

    private func shouldRetry() -> Bool {
        currentRetryCount += 1
        let needRetry = currentRetryCount < maxRetryTimes

        if needRetry {
            NSLog(TAG, "Start to retry request...")
        }

        return needRetry
    }

    private func makeRequest() -> URLRequest? {
        if let request = self.request {
            return request
        }

        guard let requestURL = makeRequestURL() else {
            NSLog(TAG, "Request URL is not valid.")
            return nil
        }

        var request = URLRequest(url: requestURL,
                                 cachePolicy: .useProtocolCachePolicy,
                                 timeoutInterval: timeoutAfterSeconds)

        request.httpMethod = httpMethod.rawValue

        var headers: [String: String] = [
            "content-type": RestfulAPI.CONTENT_TYPE_JSON,
        ]

        if let contentType = contentType, !contentType.isEmpty {
            headers["content-type"] = contentType
        }

        if let token = bearerToken {
            headers["Authorization"] = "Bearer \(token)"
        }

        request.allHTTPHeaderFields = headers
        request.httpBody = formBody

        if let data = formBody,
           contentType == RestfulAPI.CONTENT_TYPE_X_FORM,
           let dataString = String(data: data, encoding: .utf8)?.urlEncoded()
        {
            request.httpBody = Data(dataString.utf8)
        }

        return request
    }

    private func makeRequestURL() -> URL? {
        guard let domain = domain, let path = path else {
            NSLog(TAG, "Invalid domain or path.")
            return nil
        }

        let div = path.hasPrefix("/") ? "" : "/"
        let queryString = queries.isEmpty ? "" : "?" + queries.map { "\($0)=\($1)" }.joined(separator: "&")
        let urlString = "\(domain)\(div)\(path)\(queryString)"

        return URL(string: urlString)
    }
}

extension String {
    func matches(_ regex: String) -> Bool {
        range(of: regex, options: .regularExpression, range: nil, locale: nil) != nil
    }

    func urlEncoded() -> String {
        let encodeUrlString = addingPercentEncoding(withAllowedCharacters: CharacterSet.urlHostAllowed)?
            .replacingOccurrences(of: "+", with: "%2B")
        return encodeUrlString ?? self
    }

    func urlDecoded() -> String {
        removingPercentEncoding ?? self
    }
}

public extension Data {
    var prettyPrintedJSONString: NSString { /// NSString gives us a nice sanitized debugDescription
        guard let object = try? JSONSerialization.jsonObject(with: self, options: []),
              let data = try? JSONSerialization.data(withJSONObject: object, options: [.prettyPrinted]),
              let prettyPrintedString = NSString(data: data, encoding: String.Encoding.utf8.rawValue) else { return "" }

        return prettyPrintedString
    }

    var jsonString: NSString { /// NSString gives us a nice sanitized debugDescription
        guard let object = try? JSONSerialization.jsonObject(with: self, options: []),
              let data = try? JSONSerialization.data(withJSONObject: object, options: [.sortedKeys]),
              let result = NSString(data: data, encoding: String.Encoding.utf8.rawValue) else { return "" }
        return result
    }
}

public enum HttpMethod: String {
    case GET
    case PUT
    case POST
    case PATCH
    case DELETE
}
