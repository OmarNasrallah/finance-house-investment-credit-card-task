export interface Customer { id: string; fullName: string; }

export interface Portfolio {
  id: string;
  type: 'Basic' | 'Plus' | 'Premium';
  totalValue: number;
  lienAmount: number;
  availableValue: number;
  isFrozen: boolean;
  advanceRate: number;
}

export type CardStatus = 'Active' | 'Blocked' | 'Delinquent' | 'Closed';

export interface CreditCard {
  id: string;
  portfolioId: string;
  portfolioType: string;
  maskedCardNumber: string;
  creditLimit: number;
  utilizedAmount: number;
  availableCredit: number;
  status: CardStatus;
  isAtRisk: boolean;
  fullyUtilizedSinceUtc: string | null;
  issuedAtUtc: string;
}

export interface AuditEntry {
  timestampUtc: string;
  entityType: string;
  entityId: string;
  action: string;
  amount: number | null;
  details: string;
}

/** RFC 7807 body returned by the API's global exception handler. */
export interface ApiProblem { title?: string; detail?: string; code?: string; status?: number; }
