import { describe, expect } from 'https://jslib.k6.io/k6chaijs/4.3.4.3/index.js';
import { RoomAPI } from '../api/room-api.js';
import { User } from '../api/user.js';
import { config } from '../config.js';

export const options = config.K6_OPTIONS;

const uniqueId = () => Math.random().toString(36).substring(7);
const createUniqRoomInfo = (roomId, userId) => {
  return {
    room_id: roomId || uniqueId(),
    user_id: userId || 'user_id',
    space_id: 'space_id',
  };
};

let token = undefined;

export function setup() {
  if (token === undefined) {
    let user = new User(config.USERNAME, config.PASSWORD);
    token = user.token();
  }

  return token;
}

export default function testSuite(token) {
  let api = new RoomAPI(token);
  const joinRoom = (roomId, userId) => api.join(createUniqRoomInfo(roomId, userId));
  const leaveRoom = (roomId, userId) => api.leave(createUniqRoomInfo(roomId, userId));
  const getRoom = (roomId) => api.get(roomId);

  describe('GivenAUser', () => {
    describe('When join room Should have one user in room', () => {
      // arrange
      let roomId = uniqueId();

      // act
      const res = joinRoom(roomId);
      const room = getRoom(roomId);

      // assert
      expect(res.status, 'status is 200').to.equal(200);
      expect(room.status, 'status is 200').to.equal(200);
      expect(JSON.parse(room.body).data.user_count, 'player count should be 1').to.equal(1);
    });
    describe('When join room and leave Should have no user in room', () => {
      // arrange
      let roomId = uniqueId();
      const resJoin = joinRoom(roomId);
      const listBeforeLeave = getRoom(roomId);

      // act
      const resLeave = leaveRoom(roomId);
      const listAfterLeave = getRoom(roomId);

      // assert
      expect(resJoin.status, 'resJoin status is 200').to.equal(200);
      expect(resLeave.status, 'resLeave status is 200').to.equal(200);
      expect(listBeforeLeave.status, 'listBeforeLeave status is 200').to.equal(200);
      expect(listAfterLeave.status, 'listAfterLeave status is 200').to.equal(200);
      expect(
        JSON.parse(listBeforeLeave.body).data.user_count,
        'Before leave, player count should be 1',
      ).to.equal(1);
      expect(
        JSON.parse(listAfterLeave.body).data.user_count,
        'After leave, player count should be 0',
      ).to.equal(0);
    });
    describe('When another user is join Should have 2 users in room', () => {
      // arrange
      let roomId = uniqueId();
      const resJoin = joinRoom(roomId);

      // act
      const resJoin2 = joinRoom(roomId, 'user 2');
      const room = getRoom(roomId);

      // assert
      expect(resJoin.status, 'resJoin status is 200').to.equal(200);
      expect(resJoin2.status, 'resJoin2 status is 200').to.equal(200);
      expect(room.status, 'list status is 200').to.equal(200);
      expect(JSON.parse(room.body).data.user_count, 'player count should be 2').to.equal(2);
    });
    describe('When another user is join diff room Should have one user in each room', () => {
      // arrange
      let roomId1 = uniqueId();
      const resJoin = joinRoom(roomId1);
      const room1 = getRoom(roomId1);

      // act
      let roomId2 = uniqueId();
      const resJoin2 = joinRoom(roomId2);
      const room2 = getRoom(roomId2);

      // assert
      expect(resJoin.status, 'resJoin status is 200').to.equal(200);
      expect(resJoin2.status, 'resJoin2 status is 200').to.equal(200);
      expect(room1.status, 'list1 status is 200').to.equal(200);
      expect(room2.status, 'list2 status is 200').to.equal(200);
      expect(
        JSON.parse(room1.body).data.user_count,
        'player count should be 1 in first room',
      ).to.equal(1);
      expect(
        JSON.parse(room1.body).data.user_count,
        'player count should be 1 in second room',
      ).to.equal(1);
    });

    // Alternative to the above test
    describe('When join room again Should have one user in room', () => {
      // arrange
      let roomId = uniqueId();
      let userId = 'user_id';
      const resJoin = joinRoom(roomId, userId);

      // act
      const resJoin2 = joinRoom(roomId, userId);
      const room = getRoom(roomId);

      // assert
      expect(resJoin.status, 'resJoin status is 200').to.equal(200);
      expect(resJoin2.status, 'resJoin2 status is 400').to.equal(400);
      expect(room.status, 'list status is 200').to.equal(200);
      expect(JSON.parse(room.body).data.user_count, 'player count should be 1').to.equal(1);
    });
    describe('When leave with wrong room id Should Error', () => {
      // arrange
      const roomInfo = createUniqRoomInfo();
      const wrongRoomInfo = createUniqRoomInfo();

      const resJoin = api.join(roomInfo);

      // act
      const resLeave = api.leave(wrongRoomInfo);

      // assert
      expect(resJoin.status, 'resJoin status is 200').to.equal(200);
      expect(resLeave.status, 'resLeave status is 400').to.equal(400);
    });
  });

  describe('Given 2 users', () => {
    describe('When one user leave Should 1 user in room', () => {
      const roomInfo = createUniqRoomInfo();
      const roomInfo2 = createUniqRoomInfo();
      roomInfo2.room_id = roomInfo.room_id;
      roomInfo2.user_id = 'user_id2';

      const resJoin = api.join(roomInfo);
      const resJoin2 = api.join(roomInfo2);
      const room1 = api.get(roomInfo.room_id);

      // act
      const resLeave = api.leave(roomInfo);
      const room = api.get(roomInfo.room_id);

      // assert
      expect(resJoin.status, 'resJoin status is 200').to.equal(200);
      expect(resJoin2.status, 'resJoin2 status is 200').to.equal(200);
      expect(resLeave.status, 'resLeave status is 200').to.equal(200);
      expect(room.status, 'list status is 200').to.equal(200);
      expect(JSON.parse(room1.body).data.user_count, 'player count should be 2').to.equal(2);
      expect(JSON.parse(room.body).data.user_count, 'player count should be 1').to.equal(1);
    });
  });

  describe('Given 2 users in different room', () => {
    describe('When one user is leave Should have 1 users in room', () => {
      const roomInfo = createUniqRoomInfo();
      const roomInfo2 = createUniqRoomInfo();

      const resJoin = api.join(roomInfo);
      const resJoin2 = api.join(roomInfo2);

      // act
      const resLeave = api.leave(roomInfo);
      const room = api.get(roomInfo.room_id);
      const room2 = api.get(roomInfo2.room_id);

      // assert
      expect(resJoin.status, 'resJoin status is 200').to.equal(200);
      expect(resJoin2.status, 'resJoin2 status is 200').to.equal(200);
      expect(resLeave.status, 'resLeave status is 200').to.equal(200);
      expect(room.status, 'list status is 200').to.equal(200);

      expect(JSON.parse(room.body).data.user_count, 'player count should be 1').to.equal(0);
      expect(JSON.parse(room2.body).data.user_count, 'player count should be 1').to.equal(1);
    });
  });

  describe('Given no user', () => {
    describe('When leave Should Error', () => {
      // arrange
      const roomInfo = createUniqRoomInfo();

      // act
      const resLeave = api.leave(roomInfo);

      // assert
      expect(resLeave.status, 'resLeave status is 400').to.equal(400);
    });
  });
}

export function handleSummary(data) {
  return config.K6_SUMMARY(data);
}
