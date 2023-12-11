import { GameApiBase } from './api-base.js';

/**
 * The request body for creating a new room.
 * @typedef {Object} RoomRequestBody
 * @property {string} space_id - The ID of the space the room belongs to.
 * @property {string} room_id - The ID of the room.
 * @property {string} user_id - The ID of the user creating the room.
 */

export class RoomAPI {
  constructor(token) {
    this.ApiBase = GameApiBase(token);
    this.baseUrl = '/api/v1/room';
  }

  /**
   *
   * @param {RoomRequestBody} request
   */
  join(request) {
    return this.ApiBase.post(`${this.baseUrl}/join`, request);
  }

  /**
   *
   * @param {RoomRequestBody} request
   */
  leave(request) {
    return this.ApiBase.post(`${this.baseUrl}/leave`, request);
  }

  list() {
    return this.ApiBase.get(`${this.baseUrl}/list`);
  }
  get(roomId) {
    return this.ApiBase.get(`${this.baseUrl}/${roomId}`);
  }

  all() {
    return this.ApiBase.get(`${this.baseUrl}/all`);
  }
}

// export const RoomAPI = new roomApi();
