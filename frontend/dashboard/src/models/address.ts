import { Attributable } from "./attributable";

export interface Address extends Attributable{
  id?: number;
  tenant?: string;
  tenantId?: number;
  entityName?: string;
  entityId?: number | null;
  significanceOrdinal?: number | null;
  content?: string;
  lat?: number | null;
  lng?: number | null;
}