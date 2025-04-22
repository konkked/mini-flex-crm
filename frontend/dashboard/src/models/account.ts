import { PivotedRelationships } from "./relationship";
import { Attributable } from "./attributable";
import { User } from "./user";

export interface Account extends Attributable {
  id: number;
  tenantId?: number;
  tenantName?: string;
  name: string;
  description?: string;
  accountManager?: User,
  salesRep?: User,
  presalesRep?: User
}

export interface FullAccount extends Account {
  relationships: PivotedRelationships;
}

export interface AccountFormData extends Attributable {
  id?: number;
  tenantId?: number;
  description?: string;
  accountManager?: User;
  salesRep?: User;
  presalesRep?: User;
  name?: string;
}

export interface FullAccountFormData extends Account {
  relationships: PivotedRelationships;
}