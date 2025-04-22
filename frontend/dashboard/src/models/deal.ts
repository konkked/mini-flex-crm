import { Attributable } from "./attributable";
import { LeadDataOriginType } from "./lead";

export enum DealStatusType {
  Abandoned = 'Abandoned',
  Qualified = 'Qualified',
  Outreach = 'Outreach',
  Nurture = 'Nurture',
  Closing = 'Closing',
  Closed = 'Closed',
}

export interface Deal extends Attributable {
  id: number;
  leadId?: number;
  tenant: string;
  tenantId: number;
  status: DealStatusType;
  leadDataOrigin: LeadDataOriginType;
  name: string;
  companyName?: string;
  industry?: string;
  approximateCompanySize?: number | null;
  approximateRevenue?: number | null;
  email?: string;
  phone?: string;
  ownerId?: number;
}


export interface DealFormData extends Attributable {
    tenant?: string;
    tenantId?: number;
    status?: DealStatusType;
    leadDataOrigin?: LeadDataOriginType;
    name?: string;
    companyName?: string;
    industry?: string;
    approximateCompanySize?: number | null;
    approximateRevenue?: number | null;
    email?: string;
    phone?: string;
  }