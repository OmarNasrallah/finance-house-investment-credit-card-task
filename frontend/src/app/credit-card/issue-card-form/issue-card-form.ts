import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

import { ApiService, describeError } from '../../core/api.service';
import { Portfolio } from '../../core/models';

@Component({
  selector: 'app-issue-card-form',
  templateUrl: './issue-card-form.html',
  standalone: false
})
export class IssueCardForm {
  private api = inject(ApiService);
  private fb = inject(FormBuilder);

  @Input() customerId = '';
  @Input() portfolios: Portfolio[] = [];
  @Output() issued = new EventEmitter<void>();

  // Client-side checks cover input shape only; business rules are enforced (and explained) by the API.
  form = this.fb.nonNullable.group({
    portfolioId: ['', Validators.required],
    lienAmount: [null as number | null, [Validators.required, Validators.min(0.01)]]
  });

  submitting = false;
  error: { message: string; code?: string } | null = null;
  success = '';

  get selected(): Portfolio | undefined {
    return this.portfolios.find(p => p.id === this.form.controls.portfolioId.value);
  }

  /** Same formula as the backend: lien × advance rate, truncated to 2 decimals. */
  get previewLimit(): number | null {
    const lien = this.form.controls.lienAmount.value;
    const p = this.selected;
    return p && lien && lien > 0 ? Math.trunc(lien * p.advanceRate * 100) / 100 : null;
  }

  get exceedsAvailable(): boolean {
    const lien = this.form.controls.lienAmount.value;
    return !!this.selected && !!lien && lien > this.selected.availableValue;
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const { portfolioId, lienAmount } = this.form.getRawValue();

    this.submitting = true;
    this.error = null;
    this.success = '';
    this.api.issueCard(this.customerId, portfolioId, lienAmount!).subscribe({
      next: () => {
        this.submitting = false;
        this.success = 'Lien placed and card issued.';
        this.form.reset({ portfolioId: '', lienAmount: null });
        this.issued.emit();
      },
      error: e => { this.submitting = false; this.error = describeError(e); }
    });
  }
}
