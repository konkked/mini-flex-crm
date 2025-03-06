export interface User {
  id: number;
  tenantId: number;
  username: string;
  email: string;
  name: string;
  role: string;
  enabled: boolean;
  attributes: Map<string, number | string | boolean | Map<string, number | string | boolean> | any>;
}
