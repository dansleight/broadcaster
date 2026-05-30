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

import {
  AuthTokenResponse,
  BadRequestModel,
  FullStreamState,
  GlobalSettingsModel,
  GoodModel,
  GoogleCallbackRequest,
  PlaceholderObject,
  SetPlaceholderModel,
  StreamStatusModel,
  TokenModel,
  UserObject,
} from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Api<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastGetStreamState
   * @request GET:/api/broadcast/current-task
   * @secure
   */
  broadcastGetStreamState = (params: RequestParams = {}) =>
    this.request<FullStreamState, any>({
      path: `/api/broadcast/current-task`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastSetPlaceholder
   * @request POST:/api/broadcast/set-placeholder
   * @secure
   */
  broadcastSetPlaceholder = (
    data: SetPlaceholderModel,
    params: RequestParams = {},
  ) =>
    this.request<StreamStatusModel, any>({
      path: `/api/broadcast/set-placeholder`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastSetLive
   * @request GET:/api/broadcast/set-live
   * @secure
   */
  broadcastSetLive = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/broadcast/set-live`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastStopAll
   * @request DELETE:/api/broadcast/stop-all
   * @secure
   */
  broadcastStopAll = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/broadcast/stop-all`,
      method: "DELETE",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastScheduleMeetings
   * @request GET:/api/broadcast/schedule-dummy
   * @secure
   */
  broadcastScheduleMeetings = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/broadcast/schedule-dummy`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags GoogleAuth
   * @name GoogleAuthCallback
   * @request POST:/api/auth/google/callback
   */
  googleAuthCallback = (
    data: GoogleCallbackRequest,
    params: RequestParams = {},
  ) =>
    this.request<AuthTokenResponse, any>({
      path: `/api/auth/google/callback`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags GoogleAuth
   * @name GoogleAuthRefresh
   * @request GET:/api/auth/google/refresh
   */
  googleAuthRefresh = (params: RequestParams = {}) =>
    this.request<AuthTokenResponse, any>({
      path: `/api/auth/google/refresh`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Image
   * @name ImageImage
   * @request GET:/api/image/{imageName}
   */
  imageImage = (imageName: string, params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/api/image/${imageName}`,
      method: "GET",
      ...params,
    });
  /**
   * No description
   *
   * @tags Info
   * @name InfoGetUser
   * @request GET:/api/info/User
   * @secure
   */
  infoGetUser = (params: RequestParams = {}) =>
    this.request<UserObject, any>({
      path: `/api/info/User`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Info
   * @name InfoGetUnits
   * @request GET:/api/info/Units
   * @secure
   */
  infoGetUnits = (params: RequestParams = {}) =>
    this.request<string[], any>({
      path: `/api/info/Units`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Info
   * @name InfoGetPlaceholders
   * @request GET:/api/info/Placeholders
   * @secure
   */
  infoGetPlaceholders = (params: RequestParams = {}) =>
    this.request<PlaceholderObject[], any>({
      path: `/api/info/Placeholders`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Info
   * @name InfoGetUnitPlaceholders
   * @request GET:/api/info/Placeholders/{unit}
   * @secure
   */
  infoGetUnitPlaceholders = (unit: string, params: RequestParams = {}) =>
    this.request<PlaceholderObject[], any>({
      path: `/api/info/Placeholders/${unit}`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Placeholder
   * @name PlaceholderGet
   * @request GET:/api/placeholder
   * @secure
   */
  placeholderGet = (params: RequestParams = {}) =>
    this.request<PlaceholderObject[], any>({
      path: `/api/placeholder`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Placeholder
   * @name PlaceholderAdd
   * @request POST:/api/placeholder
   * @secure
   */
  placeholderAdd = (
    query: {
      Unit: string;
      Name: string;
    },
    data: {
      /** @format binary */
      File: File;
    },
    params: RequestParams = {},
  ) =>
    this.request<PlaceholderObject[], any>({
      path: `/api/placeholder`,
      method: "POST",
      query: query,
      body: data,
      secure: true,
      type: ContentType.FormData,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Placeholder
   * @name PlaceholderGetForUnit
   * @request GET:/api/placeholder/For/{unit}
   * @secure
   */
  placeholderGetForUnit = (unit: string, params: RequestParams = {}) =>
    this.request<PlaceholderObject[], any>({
      path: `/api/placeholder/For/${unit}`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Settings
   * @name SettingsGet
   * @summary Gets the global settings necessary for the SPA to start, including MSAL settings for Authentication
   * @request GET:/api/settings
   */
  settingsGet = (params: RequestParams = {}) =>
    this.request<GlobalSettingsModel, any>({
      path: `/api/settings`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Status
   * @name StatusGet
   * @request GET:/api/status
   */
  statusGet = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/status`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Status
   * @name StatusGetRefreshToken
   * @request GET:/api/status/refresh-token
   */
  statusGetRefreshToken = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/status/refresh-token`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Status
   * @name StatusValidateRefreshToken
   * @request POST:/api/status/validate-token
   */
  statusValidateRefreshToken = (data: TokenModel, params: RequestParams = {}) =>
    this.request<boolean, any>({
      path: `/api/status/validate-token`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Status
   * @name StatusDbPath
   * @request GET:/api/status/db-path
   */
  statusDbPath = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/status/db-path`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Test
   * @name TestGet
   * @request GET:/api/test/{id}
   * @secure
   */
  testGet = (id: number, params: RequestParams = {}) =>
    this.request<GoodModel, BadRequestModel>({
      path: `/api/test/${id}`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Test
   * @name TestNotify
   * @request GET:/api/test/notify/{message}
   * @secure
   */
  testNotify = (message: string, params: RequestParams = {}) =>
    this.request<boolean, any>({
      path: `/api/test/notify/${message}`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
}
