import { PivotedRelationships } from "./relationship";

export interface Customer {
  id: number;
  tenantId: number;
  name: string;
  attributes: Map<string,number|string|boolean| Map<string, number | string | boolean>>;
}

export interface FullCustomer extends Customer {
  relationships: PivotedRelationships;
}