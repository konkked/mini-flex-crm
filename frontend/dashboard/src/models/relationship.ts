import { Company } from "./company";
import { User } from "./user";

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
  user?: User[];
}
