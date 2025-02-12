import { Company } from "./company";
import { User } from "./user";

export interface Relation {
  id: number;
  entityId: number;
  entityName: string;
  customerId: number;
  customerName: string;
  tenantId: number;
}

export interface Relationships {
  company: Company[];
  user: User[];
}
