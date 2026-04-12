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

export enum UserRole {
  Admin = "Admin",
  Tech = "Tech",
  User = "User",
}

export interface BadRequestModel {
  message: string;
  userMessage: string | null;
}

export interface GlobalSettingsModel {
  applicationMode: string;
  msalSettings: MsalSettingsModel | null;
}

export interface GoodModel {
  /** @format int32 */
  id: number;
  name: string;
}

export interface MsalSettingsModel {
  clientId: string;
  authority: string;
  apiScope: string;
  provider: string | null;
}

export interface PlaceholderObject {
  /** @format int32 */
  placeholderId: number;
  unit: string | null;
  name: string;
}

export interface SetPlaceholderModel {
  /** @format int32 */
  placeholderId: number;
  /** @format int32 */
  audioTrackId: number;
}

export interface StreamStatusModel {
  status: string;
}

export interface UserObject {
  email: string;
  name: string;
  units: string[];
  roles: UserRole[];
}
