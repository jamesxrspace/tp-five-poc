import { config } from '../config.js';
import { ApiBase } from './api-base.js';

export class User {
  /**
   *
   * @param {string} username
   * @param {string} password
   */
  constructor(username, password) {
    this.username = username;
    this.password = password;
    this._token = undefined;
  }

  token() {
    if (this._token === undefined) {
      const params = {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
      };
      const body = {
        grant_type: 'password',
        email: this.username,
        password: this.password,
        expire_secs: '600',
      };

      const api = new ApiBase(config.AUTH_SERVICE);
      const url = '/oidc/token';
      const res = api.postForm(url, body, params);
      if (res.status !== 200) {
        throw new Error('Login error', res);
      }
      this._token = JSON.parse(res.body).access_token;
    }
    return this._token;
  }
}
