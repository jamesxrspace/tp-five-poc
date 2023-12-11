/* tslint:disable */
/* eslint-disable */
/**
 * Server API - Login
 * The Restful APIs of Login.
 *
 * The version of the OpenAPI document: 1.0.0
 *
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { exists, mapValues } from '../runtime';
import type { BaseResponse } from './BaseResponse';
import {
  BaseResponseFromJSON,
  BaseResponseFromJSONTyped,
  BaseResponseToJSON,
} from './BaseResponse';
import type { SpaceGroup } from './SpaceGroup';
import { SpaceGroupFromJSON, SpaceGroupFromJSONTyped, SpaceGroupToJSON } from './SpaceGroup';

/**
 *
 * @export
 * @interface UpdateSpaceGroupResponse
 */
export interface UpdateSpaceGroupResponse extends BaseResponse {
  /**
   *
   * @type {SpaceGroup}
   * @memberof UpdateSpaceGroupResponse
   */
  data?: SpaceGroup;
}

/**
 * Check if a given object implements the UpdateSpaceGroupResponse interface.
 */
export function instanceOfUpdateSpaceGroupResponse(value: object): boolean {
  let isInstance = true;

  return isInstance;
}

export function UpdateSpaceGroupResponseFromJSON(json: any): UpdateSpaceGroupResponse {
  return UpdateSpaceGroupResponseFromJSONTyped(json, false);
}

export function UpdateSpaceGroupResponseFromJSONTyped(
  json: any,
  ignoreDiscriminator: boolean,
): UpdateSpaceGroupResponse {
  if (json === undefined || json === null) {
    return json;
  }
  return {
    ...BaseResponseFromJSONTyped(json, ignoreDiscriminator),
    data: !exists(json, 'data') ? undefined : SpaceGroupFromJSON(json['data']),
  };
}

export function UpdateSpaceGroupResponseToJSON(value?: UpdateSpaceGroupResponse | null): any {
  if (value === undefined) {
    return undefined;
  }
  if (value === null) {
    return null;
  }
  return {
    ...BaseResponseToJSON(value),
    data: SpaceGroupToJSON(value.data),
  };
}