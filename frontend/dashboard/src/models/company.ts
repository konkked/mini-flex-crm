export interface Company {
    id: number;
    tenantId: number;
    name: string;
    attributes: Map<string, number | string | boolean | Map<string, number | string | boolean> | any>;
} 