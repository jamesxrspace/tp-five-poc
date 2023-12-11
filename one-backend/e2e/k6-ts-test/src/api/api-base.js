// import for jsdoc
import http, * as k6 from 'k6/http'; // eslint-disable-line
import { config } from '../config.js';

/**
 * A base class for making HTTP requests using the k6 `http` module.
 */
export class ApiBase {
  /**
   * Creates a new instance of the `ApiBase` class.
   * @param {string} baseUrl - The base URL for the API.
   * @param {boolean} [enableAuth] - Default false, An optional flag indicating whether to enable authentication.
   * @param {string} [token]- The token to use for authentication.
   */
  constructor(baseUrl, enableAuth, token) {
    this.baseUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
    this.enableAuth = enableAuth || false;
    this.token = token || undefined;
  }

  /**
   * Attaches the authentication token to the request parameters, if present.
   * @private
   * @template RT - The response type.
   * @param {k6.RefinedParams<RT> | null | undefined} [params] - The request parameters.
   * @returns {k6.RefinedParams<RT> | null} The request parameters with the authentication token attached.
   */

  attachDefaultHeader(params) {
    if (this.enableAuth) {
      if (this.token === undefined) {
        throw new Error('token is undefined');
      }
      const defaultParams = {
        auth: 'bearer',
        headers: {},
      };
      defaultParams.headers['Authorization'] = `Bearer ${this.token}`;
      defaultParams.headers['Content-Type'] = 'application/json';
      params = Object.assign(defaultParams, params);
    }

    return params;
  }

  /**
   * Makes a GET request to the specified URL.
   * @template RT - The response type.
   * @param {string} url - The URL to request.
   * @param {k6.RefinedParams<RT> | null | undefined} [params] - Optional request parameters.
   * @returns {k6.RefinedResponse<RT>} The response from the server.
   */
  get(url, params) {
    return http.get(this.url(url), this.attachDefaultHeader(params));
  }

  /**
   * Makes a POST request to the specified URL with the specified body.
   * @template RT - The response type.
   * @param {string} url - The URL to request.
   * @param {k6.RequestBody | null | undefined} [body] - Optional request body.
   * @param {k6.RefinedParams<RT> | null | undefined} [params] - Optional request parameters.
   * @returns {k6.RefinedResponse<RT>} The response from the server.
   */
  post(url, body, params) {
    return http.post(this.url(url), JSON.stringify(body), this.attachDefaultHeader(params));
  }

  postForm(url, body, params) {
    return http.post(this.url(url), body, this.attachDefaultHeader(params));
  }

  /**
   * Makes a PUT request to the specified URL with the specified body.
   * @template RT - The response type.
   * @param {string} url - The URL to request.
   * @param {k6.RequestBody | null | undefined} [body] - Optional request body.
   * @param {k6.RefinedParams<RT> | null | undefined} [params] - Optional request parameters.
   * @returns {k6.RefinedResponse<RT>} The response from the server.
   */
  put(url, body, params) {
    return http.put(this.url(url), body, this.attachDefaultHeader(params));
  }

  /**
   * Makes a DELETE request to the specified URL with the specified body.
   * @template RT - The response type.
   * @param {string} url - The URL to request.
   * @param {k6.RequestBody | null | undefined} [body] - Optional request body.
   * @param {k6.RefinedParams<RT> | null | undefined} [params] - Optional request parameters.
   * @returns {k6.RefinedResponse<RT>} The response from the server.
   */
  del(url, body, params) {
    return http.del(this.url(url), body, this.attachDefaultHeader(params));
  }

  /**
   * Makes a PATCH request to the specified URL with the specified body.
   * @template RT - The response type.
   * @param {string} url - The URL to request.
   * @param {k6.RequestBody | null | undefined} [body] - Optional request body.
   * @param {k6.RefinedParams<RT> | null | undefined} [params] - Optional request parameters.
   * @returns {k6.RefinedResponse<RT>} The response from the server.
   */
  patch(url, body, params) {
    return http.patch(this.url(url), body, this.attachDefaultHeader(params));
  }

  /**
   * Returns the full URL for the specified path.
   * @param {string} path - The path to append to the base URL.
   * @returns {string} The full URL.
   */
  url(path) {
    const url = this.baseUrl + path;

    // Validate the URL
    const urlRegex = /^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$/i;
    if (!urlRegex.test(url)) {
      throw new Error(`Invalid URL: ${url}`);
    }

    return url;
  }
}

/**
 * create a new instance of the `ApiBase` class for game service.
 *
 * @param {string} token - The token to use for authentication.
 * @returns {ApiBase}
 * @constructor
 */
const GameApiBase = (token) => {
  return new ApiBase(config.GAME_SERVICE, true, token);
};

const AccountApiBase = () => {
  return new ApiBase(config.AUTH_SERVICE, false);
};

export { GameApiBase, AccountApiBase };
