export interface Tenant {
  id: number;
  name: string;
  attributes: Map<string, number | string | boolean | Map<string, number | string | boolean> | any>;
}
