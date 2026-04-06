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
  AddWidgetModel,
  BadRequestModel,
  GlobalSettingsModel,
  GoodModel,
  HttpValidationError,
  StreamStatusModel,
  WidgetObject,
} from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Api<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastTest
   * @request GET:/api/broadcast/test
   */
  broadcastTest = (params: RequestParams = {}) =>
    this.request<boolean, any>({
      path: `/api/broadcast/test`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastAudioTest
   * @request GET:/api/broadcast/audio-test
   */
  broadcastAudioTest = (params: RequestParams = {}) =>
    this.request<boolean, any>({
      path: `/api/broadcast/audio-test`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastGetCurrentTask
   * @request GET:/api/broadcast/current-task
   */
  broadcastGetCurrentTask = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/broadcast/current-task`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastSetPlaceholder
   * @request GET:/api/broadcast/set-placeholder/{usealt}
   */
  broadcastSetPlaceholder = (usealt: boolean, params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/broadcast/set-placeholder/${usealt}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastSetLive
   * @request GET:/api/broadcast/set-live
   */
  broadcastSetLive = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/broadcast/set-live`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastStopAll
   * @request DELETE:/api/broadcast/stop-all
   */
  broadcastStopAll = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/broadcast/stop-all`,
      method: "DELETE",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastScheduleMeetings
   * @request GET:/api/broadcast/schedule-dummy
   */
  broadcastScheduleMeetings = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/broadcast/schedule-dummy`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Preview
   * @name PreviewPing
   * @request POST:/api/preview/ping
   */
  previewPing = (params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/api/preview/ping`,
      method: "POST",
      ...params,
    });
  /**
   * No description
   *
   * @tags Preview
   * @name PreviewGetMjpeg
   * @request GET:/api/preview/mjpeg
   */
  previewGetMjpeg = (params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/api/preview/mjpeg`,
      method: "GET",
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
   * @tags Widget
   * @name WidgetGet
   * @request GET:/api/widget
   * @secure
   */
  widgetGet = (params: RequestParams = {}) =>
    this.request<WidgetObject[], any>({
      path: `/api/widget`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Widget
   * @name WidgetAdd
   * @request POST:/api/widget
   * @secure
   */
  widgetAdd = (data: AddWidgetModel, params: RequestParams = {}) =>
    this.request<WidgetObject, HttpValidationError>({
      path: `/api/widget`,
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
   * @tags Widget
   * @name WidgetGet2
   * @request GET:/api/widget/{widgetId}
   * @originalName widgetGet
   * @duplicate
   * @secure
   */
  widgetGet2 = (widgetId: number, params: RequestParams = {}) =>
    this.request<WidgetObject, any>({
      path: `/api/widget/${widgetId}`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
}
