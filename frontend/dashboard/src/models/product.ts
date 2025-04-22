import { Attributable } from "./attributable";

export interface Product extends Attributable{
  id?: number;
  tenant?: string;
  tenantId?: number;
  name?: string;
  suggestedPrice: number;
  termMonths?: number | null;
}