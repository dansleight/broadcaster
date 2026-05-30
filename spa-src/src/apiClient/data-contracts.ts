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

export enum StreamState {
  Idle = "Idle",
  Live = "Live",
  Placeholder = "Placeholder",
}

export enum PlaceholderType {
  Pre = "Pre",
  Sacrament = "Sacrament",
  Post = "Post",
}

export enum MusicType {
  Default = "Default",
  Sacrament = "Sacrament",
}

export interface AuthTokenResponse {
  accessToken: string;
  user: UserObject;
}

export interface BadRequestModel {
  message: string;
  userMessage: string | null;
}

export interface FullStreamState {
  streamState: StreamState;
  placeholderImage: string | null;
  placeholderMusic: string | null;
}

export interface GlobalSettingsModel {
  applicationMode: string;
  googleClientId: string;
  redirectUri: string;
}

export interface GoodModel {
  /** @format int32 */
  id: number;
  name: string;
}

export interface GoogleCallbackRequest {
  /** @minLength 1 */
  code: string;
}

export interface PlaceholderObject {
  /** @format int32 */
  placeholderId: number;
  unit: string | null;
  name: string;
}

export interface SetPlaceholderModel {
  placeholderType: PlaceholderType;
  musicType: MusicType | null;
}

export interface StreamStatusModel {
  status: string;
}

export interface TokenModel {
  token: string;
}

export interface UserObject {
  googleSub: string;
  email: string;
  name: string;
  givenName: string | null;
  familyName: string | null;
  picture: string | null;
  units: string[];
  roles: UserRole[];
}
