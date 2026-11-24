import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

import { ApiService, describeError } from '../../core/api.service';
import { Portfolio } from '../../core/models';

@Component({
  selector: 'app-redeem-form',
  templateUrl: './redeem-form.html',
  standalone: false
})
export class RedeemForm {
  private api = inject(ApiService);
  private fb = inject(FormBuilder);

  @Input() portfolios: Portfolio[] = [];
  @Output() redeemed = new EventEmitter<void>();

  // No max validator on purpose: the lien rule lives in the Domain, and the API explains violations.
  form = this.fb.nonNullable.group({
    portfolioId: ['', Validators.required],
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]]
  });

  submitting = false;
  error: { message: string; code?: string } | null = null;
  success = '';

  get selected(): Portfolio | undefined {
    return this.portfolios.find(p => p.id === this.form.controls.portfolioId.value);
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const { portfolioId, amount } = this.form.getRawValue();

    this.submitting = true;
    this.error = null;
    this.success = '';
    this.api.redeem(portfolioId, amount!).subscribe({
      next: () => {
        this.submitting = false;
        this.success = `Redeemed ${amount!.toFixed(2)}.`;
        this.form.controls.amount.reset(null);
        this.redeemed.emit();
      },
      error: e => { this.submitting = false; this.error = describeError(e); }
    });
  }
}
