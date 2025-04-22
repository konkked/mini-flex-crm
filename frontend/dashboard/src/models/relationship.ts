import { Company } from "./company";
import { User } from "./user";
import { Deal } from "./deal";
import { Interaction } from "./interaction";
import { Sale } from "./sale";

export interface Relationship {
  id: number;
  entityId: number;
  entityName: string;
  customerId: number;
  customerName: string;
  tenantId: number;
}

export interface PivotedRelationships {
  company?: Company[];
  interaction?: Interaction[];
  sale: Sale[];
  deal?: Deal[];
  user?: User[];
}
