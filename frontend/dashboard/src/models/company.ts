import { Attributable } from "./attributable";

export interface Company extends Attributable {
    id: number;
    tenantId: number;
    tenantName: string;
    name: string;
} 

export interface CompanyFormData extends Attributable {
    tenantId?: number;
    name?: string;
} 