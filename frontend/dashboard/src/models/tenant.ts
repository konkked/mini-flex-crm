import { Attributable } from "./attributable";

export interface Tenant extends Attributable {
  id: number;
  name: string;
  shortId: string;
  theme: string;
}

export interface TenantFormData extends Attributable {
  name?: string;
  shortId?: string;
  theme?: string;
}
