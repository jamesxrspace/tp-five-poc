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
import type { Space } from './Space';
import { SpaceFromJSON, SpaceFromJSONTyped, SpaceToJSON } from './Space';

/**
 *
 * @export
 * @interface UpdateSpaceResponse
 */
export interface UpdateSpaceResponse extends BaseResponse {
  /**
   *
   * @type {Space}
   * @memberof UpdateSpaceResponse
   */
  data?: Space;
}

/**
 * Check if a given object implements the UpdateSpaceResponse interface.
 */
export function instanceOfUpdateSpaceResponse(value: object): boolean {
  let isInstance = true;

  return isInstance;
}

export function UpdateSpaceResponseFromJSON(json: any): UpdateSpaceResponse {
  return UpdateSpaceResponseFromJSONTyped(json, false);
}

export function UpdateSpaceResponseFromJSONTyped(
  json: any,
  ignoreDiscriminator: boolean,
): UpdateSpaceResponse {
  if (json === undefined || json === null) {
    return json;
  }
  return {
    ...BaseResponseFromJSONTyped(json, ignoreDiscriminator),
    data: !exists(json, 'data') ? undefined : SpaceFromJSON(json['data']),
  };
}

export function UpdateSpaceResponseToJSON(value?: UpdateSpaceResponse | null): any {
  if (value === undefined) {
    return undefined;
  }
  if (value === null) {
    return null;
  }
  return {
    ...BaseResponseToJSON(value),
    data: SpaceToJSON(value.data),
  };
}