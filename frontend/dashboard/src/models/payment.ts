import { Attributable } from "./attributable";

export enum PaymentType {
  Cash = 0,
  CreditCard = 1,
  BankTransfer = 2,
}

export interface Payment extends Attributable {
  id?: number;
  tenant?: string;
  tenantId?: number;
  type?: PaymentType;
  title?: string;
  value: number;
  saleId: number;
  attributes?: any;
}