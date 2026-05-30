import { UserObject } from "../apiClient/data-contracts";

export const InRole = (user: UserObject, role: string) => {
  if (!user.roles) return false;
  return user.roles.some((r) => r.toLowerCase() === role.toLowerCase());
};
