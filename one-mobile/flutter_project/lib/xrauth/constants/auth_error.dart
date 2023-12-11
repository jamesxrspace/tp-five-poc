class AuthError {
  final int code;
  final String message;

  AuthError(this.code, this.message);

  @override
  String toString() {
    return 'AuthError{code: $code, message: $message}';
  }

  // Also means success
  static final AuthError none = AuthError(0, 'None');

  // System error
  static final AuthError functionNotSupported =
      AuthError(2001, 'Function not supported');
  static final AuthError functionNotImplemented =
      AuthError(2002, 'Function not implemented');

  // Data cache error
  static final AuthError saveDataFailed = AuthError(2011, 'Save data failed');
  static final AuthError readDataFailed = AuthError(2012, 'Read data failed');
  static final AuthError deleteDataFailed =
      AuthError(2013, 'Delete data failed');

  // State error
  static final AuthError notSignedIn = AuthError(2021, 'Not signed in');
  static final AuthError signInAlready = AuthError(2022, 'Already signed in');

  // Auth error
  static final AuthError invalidParameter =
      AuthError(2101, 'Invalid parameter');
  static final AuthError invalidCredentials =
      AuthError(2102, 'Invalid credentials');
  static final AuthError invalidAccessToken =
      AuthError(2103, 'Invalid access token');
  static final AuthError invalidRefreshToken =
      AuthError(2104, 'Invalid refresh token');
  static final AuthError accessTokenExpired =
      AuthError(2105, 'Access token expired');
  static final AuthError refreshTokenExpired =
      AuthError(2106, 'Refresh token expired');
  static final AuthError userCancelLogin = AuthError(2111, 'User cancel login');
  static final AuthError authCodeFailed =
      AuthError(2112, 'Failed to get authorization code during login');
  static final AuthError authTokenFailed = AuthError(
      2113, 'Failed to exchange access_token with authorization code');

  // User instance error
  static final AuthError usernameAlreadyExists =
      AuthError(2201, 'Username already exists');
  static final AuthError emailAlreadyExists =
      AuthError(2202, 'Email already exists');
  static final AuthError usernameMalformed =
      AuthError(2203, 'Username malformed');
  static final AuthError emailMalformed = AuthError(2204, 'Email malformed');
  static final AuthError userNotFound = AuthError(2205, 'User not found');
  static final AuthError renewAccessTokenFailed =
      AuthError(2206, 'Renew access token failed');

  // Http error
  static final AuthError network = AuthError(2301, 'Network error');
  static final AuthError requestTimeout = AuthError(2302, 'Request timeout');
  static final AuthError requestFailed = AuthError(2303, 'Request failed');
  static final AuthError emptyResponse = AuthError(2304, 'Empty response');
  static final AuthError invalidResponse = AuthError(2305, 'Invalid response');
  static final AuthError malformedResponse =
      AuthError(2306, 'Malformed response');

  // Device auth error
  static final AuthError authorizationPending =
      AuthError(2501, 'Authorization pending');
  static final AuthError slowdown = AuthError(2502, 'Slowdown');
  static final AuthError cannotGetDeviceCode =
      AuthError(2503, 'Cannot get device code');
  static final AuthError deviceCodeExpired =
      AuthError(2504, 'Device code expired');

  // Guest auth error
  static final AuthError guestNotExist = AuthError(2601, 'Guest not exist');
  static final AuthError createGuestFailed =
      AuthError(2602, 'Create guest failed');

  static final AuthError unknown = AuthError(2999, 'Unknown error');
}
