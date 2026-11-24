import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { Dashboard } from './dashboard/dashboard';
import { PortfolioList } from './portfolio-list/portfolio-list';
import { IssueCardForm } from './issue-card-form/issue-card-form';
import { CardList } from './card-list/card-list';
import { RedeemForm } from './redeem-form/redeem-form';
import { AuditTrail } from './audit-trail/audit-trail';

const routes: Routes = [{ path: '', component: Dashboard }];

@NgModule({
  declarations: [Dashboard, PortfolioList, IssueCardForm, CardList, RedeemForm, AuditTrail],
  imports: [CommonModule, ReactiveFormsModule, RouterModule.forChild(routes)]
})
export class CreditCardModule { }
