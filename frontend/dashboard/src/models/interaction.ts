import { Attributable } from "./attributable";

export enum InteractionType {
  // No specific enum defined in Swagger, so using a placeholder. Adjust if needed.
}

export interface Interaction extends Attributable {
  id: number;
  tenant: string;
  tenantId: number;
  type?: string;
  interactionDate: string; // date-time format
  notes?: string;
  accountId?: number | null;
  contactId?: number | null;
  leadId?: number | null;
}