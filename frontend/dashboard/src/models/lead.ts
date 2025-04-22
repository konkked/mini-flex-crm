import { Attributable } from "./attributable";

export enum LeadStatusType {
  Dead  = 'Dead',
  Raw = 'Raw',
  Bronze = 'Bronze',
  Silver = 'Silver',
  Gold = 'Gold',
  Qualified = 'Qualified'
}

export enum LeadDataOriginType {
  Scraped = 'Scraped',
  Manual = 'Manual',
  Import = 'Import',
  BulkEntry = 'BulkEntry',
}


export interface Lead extends Attributable {
  id: number;
  tenant: string;
  tenantId: number;
  status: LeadStatusType;
  leadDataOrigin: LeadDataOriginType;
  name: string;
  companyName?: string;
  industry?: string;
  approximateCompanySize?: number | null;
  approximateRevenue?: number | null;
  email?: string;
  phone?: string;
}


export interface LeadFormData extends Attributable {
    tenant?: string;
    tenantId?: number;
    status?: LeadStatusType;
    leadDataOrigin?: LeadDataOriginType;
    name?: string;
    companyName?: string;
    industry?: string;
    approximateCompanySize?: number | null;
    approximateRevenue?: number | null;
    email?: string;
    phone?: string;
  }