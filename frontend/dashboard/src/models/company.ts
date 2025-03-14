import { Attributable } from "./attributable";

export interface Company extends Attributable {
    id: number;
    tenantId: number;
    name: string;
} 