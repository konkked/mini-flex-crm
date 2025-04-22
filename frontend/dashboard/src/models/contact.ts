export interface Contact {
  id?: number;
  tenant?: string;
  tenantId?: number;
  entityName?: string;
  entityId?: number | null;
  significanceOrdinal?: number | null;
  name?: string;
  title?: string;
  email?: string;
  emailVerified?: boolean | null;
  phone?: string;
  phoneVerified?: boolean | null;
  canText: boolean;
  canCall: boolean;
  canEmail: boolean;
  attributes?: any;
}