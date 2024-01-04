import type { AccessToken } from "./accessToken";

export type ResourceCredential = {
  clientId: string;
  accessToken: AccessToken;
};
