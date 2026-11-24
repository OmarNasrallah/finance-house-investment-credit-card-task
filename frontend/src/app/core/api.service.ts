import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuditEntry, CreditCard, Customer, Portfolio } from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private base = '/api';

  // Queries
  getCustomers(): Observable<Customer[]> { return this.http.get<Customer[]>(`${this.base}/customers`); }
  getPortfolios(customerId: string): Observable<Portfolio[]> {
    return this.http.get<Portfolio[]>(`${this.base}/customers/${customerId}/portfolios`);
  }
  getCards(customerId: string): Observable<CreditCard[]> {
    return this.http.get<CreditCard[]>(`${this.base}/customers/${customerId}/cards`);
  }
  getAudit(customerId: string): Observable<AuditEntry[]> {
    return this.http.get<AuditEntry[]>(`${this.base}/customers/${customerId}/audit`);
  }

  // Commands
  issueCard(customerId: string, portfolioId: string, lienAmount: number): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.base}/cards`, { customerId, portfolioId, lienAmount });
  }
  charge(cardId: string, amount: number): Observable<void> {
    return this.http.post<void>(`${this.base}/cards/${cardId}/charge`, { amount });
  }
  repay(cardId: string, amount: number): Observable<void> {
    return this.http.post<void>(`${this.base}/cards/${cardId}/repay`, { amount });
  }
  closeCard(cardId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/cards/${cardId}/close`, {});
  }
  redeem(portfolioId: string, amount: number): Observable<void> {
    return this.http.post<void>(`${this.base}/portfolios/${portfolioId}/redeem`, { amount });
  }
  triggerDelinquencyCheck(): Observable<{ jobId: string }> {
    return this.http.post<{ jobId: string }>(`${this.base}/jobs/delinquency-check`, {});
  }
}

/** Turns an API error into a readable message, keeping the business-rule code when present. */
export function describeError(err: unknown): { message: string; code?: string } {
  if (err instanceof HttpErrorResponse) {
    if (err.status === 0) return { message: 'Cannot reach the API. Is the backend running on port 5080?' };
    const body = err.error ?? {};
    return { message: body.detail ?? body.title ?? err.message, code: body.code };
  }
  return { message: String(err) };
}
