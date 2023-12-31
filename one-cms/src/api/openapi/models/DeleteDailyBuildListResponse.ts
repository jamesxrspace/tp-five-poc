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

/**
 *
 * @export
 * @interface DeleteDailyBuildListResponse
 */
export interface DeleteDailyBuildListResponse extends BaseResponse {
  /**
   *
   * @type {boolean}
   * @memberof DeleteDailyBuildListResponse
   */
  success?: boolean;
  /**
   *
   * @type {string}
   * @memberof DeleteDailyBuildListResponse
   */
  message?: string;
}

/**
 * Check if a given object implements the DeleteDailyBuildListResponse interface.
 */
export function instanceOfDeleteDailyBuildListResponse(value: object): boolean {
  let isInstance = true;

  return isInstance;
}

export function DeleteDailyBuildListResponseFromJSON(json: any): DeleteDailyBuildListResponse {
  return DeleteDailyBuildListResponseFromJSONTyped(json, false);
}

export function DeleteDailyBuildListResponseFromJSONTyped(
  json: any,
  ignoreDiscriminator: boolean,
): DeleteDailyBuildListResponse {
  if (json === undefined || json === null) {
    return json;
  }
  return {
    ...BaseResponseFromJSONTyped(json, ignoreDiscriminator),
    success: !exists(json, 'Success') ? undefined : json['Success'],
    message: !exists(json, 'Message') ? undefined : json['Message'],
  };
}

export function DeleteDailyBuildListResponseToJSON(
  value?: DeleteDailyBuildListResponse | null,
): any {
  if (value === undefined) {
    return undefined;
  }
  if (value === null) {
    return null;
  }
  return {
    ...BaseResponseToJSON(value),
    Success: value.success,
    Message: value.message,
  };
}
