export interface Customer {
  id: number;
  tenantId: string;
  name: string;
  attributes: Map<string,number|string|boolean| Map<string, number | string | boolean>>;
}
