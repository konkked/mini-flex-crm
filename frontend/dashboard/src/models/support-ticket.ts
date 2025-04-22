import { Attributable } from "./attributable";

export enum SupportTicketStatusType {
  Open = 0,
  InProgress = 1,
  Resolved = 2,
  Closed = 3,
  Escalated = 4,
}

export interface SupportTicketModel extends Attributable {
  id?: number;
  tenant?: string;
  tenantId?: number;
  issue?: string;
  status?: SupportTicketStatusType;
  userId: number;
}