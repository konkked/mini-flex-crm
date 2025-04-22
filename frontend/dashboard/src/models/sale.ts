import { Attributable } from "./attributable";

export interface Sale extends Attributable {
  id?: number;
  tenant?: string;
  tenantId?: number;
  title?: string;
  description?: string;
  value: number;
  termMonths?: number | null;
  dealId?: number | null;
}