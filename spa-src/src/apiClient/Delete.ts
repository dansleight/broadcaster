/* eslint-disable */
/* tslint:disable */
// @ts-nocheck
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

import { HttpClient, RequestParams } from "./http-client";

export class Delete<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Placeholder
   * @name PlaceholderDelete
   * @request DELETE:/delete/{placeholderId}
   * @secure
   */
  placeholderDelete = (placeholderId: number, params: RequestParams = {}) =>
    this.request<boolean, any>({
      path: `/delete/${placeholderId}`,
      method: "DELETE",
      secure: true,
      format: "json",
      ...params,
    });
}
