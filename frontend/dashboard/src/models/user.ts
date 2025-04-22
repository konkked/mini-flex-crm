import { Attributable } from "./attributable";

export interface User extends Attributable {
  id: number;
  tenantId: number;
  tenantName :string;
  username: string;
  email: string;
  name: string;
  role: string;
  enabled: boolean;
  profileImage?: string;
}

export interface UserFormData extends Attributable {
  tenantId?: number;
  username?: string;
  password?: string;
  email?: string;
  name?: string;
  role?: string;
  enabled?: boolean;
  profileImage?: string;
}
