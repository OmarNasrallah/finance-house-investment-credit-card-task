import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { ApiService, describeError } from '../../core/api.service';
import { CreditCard } from '../../core/models';

@Component({
  selector: 'app-card-list',
  templateUrl: './card-list.html',
  standalone: false
})
export class CardList {
  private api = inject(ApiService);

  @Input() cards: CreditCard[] = [];
  @Output() changed = new EventEmitter<void>();

  /** One amount control per card, created lazily. */
  private amountCtrls = new Map<string, FormControl<number | null>>();
  errors: Record<string, { message: string; code?: string }> = {};
  busy: Record<string, boolean> = {};

  amountCtrl(cardId: string): FormControl<number | null> {
    let ctrl = this.amountCtrls.get(cardId);
    if (!ctrl) {
      ctrl = new FormControl<number | null>(null, [Validators.required, Validators.min(0.01)]);
      this.amountCtrls.set(cardId, ctrl);
    }
    return ctrl;
  }

  utilizationPct(c: CreditCard): number {
    return c.creditLimit > 0 ? Math.min(100, (c.utilizedAmount / c.creditLimit) * 100) : 0;
  }

  charge(card: CreditCard): void {
    const ctrl = this.amountCtrl(card.id);
    if (ctrl.invalid) { ctrl.markAsTouched(); return; }
    this.run(card.id, this.api.charge(card.id, ctrl.value!), true);
  }

  repay(card: CreditCard): void {
    const ctrl = this.amountCtrl(card.id);
    if (ctrl.invalid) { ctrl.markAsTouched(); return; }
    this.run(card.id, this.api.repay(card.id, ctrl.value!), true);
  }

  close(card: CreditCard): void {
    this.run(card.id, this.api.closeCard(card.id), false);
  }

  private run(cardId: string, call: Observable<void>, resetAmount: boolean): void {
    this.busy[cardId] = true;
    delete this.errors[cardId];
    call.subscribe({
      next: () => {
        this.busy[cardId] = false;
        if (resetAmount) this.amountCtrl(cardId).reset(null);
        this.changed.emit();
      },
      error: e => { this.busy[cardId] = false; this.errors[cardId] = describeError(e); }
    });
  }
}
