import { PivotedRelationships } from "./relationship";
import { Attributable } from "./attributable";

export interface Customer extends Attributable {
  id: number;
  tenantId: number;
  name: string;
}

export interface FullCustomer extends Customer {
  relationships: PivotedRelationships;
}

export interface CustomerFormData extends Attributable {
  tenantId?: number;
  name?: string;
}

export interface FullCustomerFormData extends Customer {
  relationships: PivotedRelationships;
}