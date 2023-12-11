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
import type { Space } from './Space';
import { SpaceFromJSON, SpaceFromJSONTyped, SpaceToJSON } from './Space';

/**
 *
 * @export
 * @interface GetSpaceListResponseAllOfData
 */
export interface GetSpaceListResponseAllOfData {
  /**
   *
   * @type {number}
   * @memberof GetSpaceListResponseAllOfData
   */
  total?: number;
  /**
   *
   * @type {Array<Space>}
   * @memberof GetSpaceListResponseAllOfData
   */
  items?: Array<Space>;
}

/**
 * Check if a given object implements the GetSpaceListResponseAllOfData interface.
 */
export function instanceOfGetSpaceListResponseAllOfData(value: object): boolean {
  let isInstance = true;

  return isInstance;
}

export function GetSpaceListResponseAllOfDataFromJSON(json: any): GetSpaceListResponseAllOfData {
  return GetSpaceListResponseAllOfDataFromJSONTyped(json, false);
}

export function GetSpaceListResponseAllOfDataFromJSONTyped(
  json: any,
  ignoreDiscriminator: boolean,
): GetSpaceListResponseAllOfData {
  if (json === undefined || json === null) {
    return json;
  }
  return {
    total: !exists(json, 'total') ? undefined : json['total'],
    items: !exists(json, 'items') ? undefined : (json['items'] as Array<any>).map(SpaceFromJSON),
  };
}

export function GetSpaceListResponseAllOfDataToJSON(
  value?: GetSpaceListResponseAllOfData | null,
): any {
  if (value === undefined) {
    return undefined;
  }
  if (value === null) {
    return null;
  }
  return {
    total: value.total,
    items: value.items === undefined ? undefined : (value.items as Array<any>).map(SpaceToJSON),
  };
}
