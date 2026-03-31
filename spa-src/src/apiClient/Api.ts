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
   * @name BroadcastGetCurrentTask
   * @request GET:/api/Broadcast/current-task
   */
  broadcastGetCurrentTask = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/Broadcast/current-task`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastSetPlaceholder
   * @request GET:/api/Broadcast/set-placeholder/{usealt}
   */
  broadcastSetPlaceholder = (usealt: boolean, params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/Broadcast/set-placeholder/${usealt}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastSetLive
   * @request GET:/api/Broadcast/set-live
   */
  broadcastSetLive = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/Broadcast/set-live`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastStopAll
   * @request DELETE:/api/Broadcast/stop-all
   */
  broadcastStopAll = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/Broadcast/stop-all`,
      method: "DELETE",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Broadcast
   * @name BroadcastScheduleMeetings
   * @request GET:/api/Broadcast/schedule-dummy
   */
  broadcastScheduleMeetings = (params: RequestParams = {}) =>
    this.request<StreamStatusModel, any>({
      path: `/api/Broadcast/schedule-dummy`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Settings
   * @name SettingsGet
   * @summary Gets the global settings necessary for the SPA to start, including MSAL settings for Authentication
   * @request GET:/api/Settings
   */
  settingsGet = (params: RequestParams = {}) =>
    this.request<GlobalSettingsModel, any>({
      path: `/api/Settings`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Test
   * @name TestGet
   * @request GET:/api/Test/{id}
   * @secure
   */
  testGet = (id: number, params: RequestParams = {}) =>
    this.request<GoodModel, BadRequestModel>({
      path: `/api/Test/${id}`,
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
   * @request GET:/api/Widget
   * @secure
   */
  widgetGet = (params: RequestParams = {}) =>
    this.request<WidgetObject[], any>({
      path: `/api/Widget`,
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
   * @request POST:/api/Widget
   * @secure
   */
  widgetAdd = (data: AddWidgetModel, params: RequestParams = {}) =>
    this.request<WidgetObject, HttpValidationError>({
      path: `/api/Widget`,
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
   * @request GET:/api/Widget/{widgetId}
   * @originalName widgetGet
   * @duplicate
   * @secure
   */
  widgetGet2 = (widgetId: number, params: RequestParams = {}) =>
    this.request<WidgetObject, any>({
      path: `/api/Widget/${widgetId}`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
}
