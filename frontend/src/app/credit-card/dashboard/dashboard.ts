import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { ApiService, describeError } from '../../core/api.service';
import { AuditEntry, CreditCard, Customer, Portfolio } from '../../core/models';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  standalone: false
})
export class Dashboard implements OnInit {
  private api = inject(ApiService);
  private destroyRef = inject(DestroyRef);

  customers: Customer[] = [];
  customerCtrl = new FormControl<string>('', { nonNullable: true });

  portfolios: Portfolio[] = [];
  cards: CreditCard[] = [];
  audit: AuditEntry[] = [];

  loadError = '';
  jobMessage = '';
  jobRunning = false;

  ngOnInit(): void {
    this.api.getCustomers().subscribe({
      next: list => {
        this.customers = list;
        if (list.length) this.customerCtrl.setValue(list[0].id);
      },
      error: e => (this.loadError = describeError(e).message)
    });

    this.customerCtrl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => { this.jobMessage = ''; this.refresh(); });
  }

  get customerId(): string { return this.customerCtrl.value; }

  /** Re-runs all queries for the selected customer. Called after every command. */
  refresh(): void {
    if (!this.customerId) return;
    forkJoin({
      portfolios: this.api.getPortfolios(this.customerId),
      cards: this.api.getCards(this.customerId),
      audit: this.api.getAudit(this.customerId)
    }).subscribe({
      next: r => {
        this.portfolios = r.portfolios;
        this.cards = r.cards;
        this.audit = r.audit;
        this.loadError = '';
      },
      error: e => (this.loadError = describeError(e).message)
    });
  }

  runDelinquencyCheck(): void {
    this.jobRunning = true;
    this.jobMessage = '';
    this.api.triggerDelinquencyCheck().subscribe({
      next: r => {
        this.jobMessage = `Hangfire job #${r.jobId} enqueued. Refreshing in a moment…`;
        // The job runs asynchronously on the Hangfire server; give it a moment, then re-query.
        setTimeout(() => { this.jobRunning = false; this.refresh(); }, 2000);
      },
      error: e => { this.jobRunning = false; this.jobMessage = describeError(e).message; }
    });
  }
}
