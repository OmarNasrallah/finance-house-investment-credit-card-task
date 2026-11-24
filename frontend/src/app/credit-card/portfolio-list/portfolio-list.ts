import { Component, Input } from '@angular/core';
import { Portfolio } from '../../core/models';

@Component({
  selector: 'app-portfolio-list',
  templateUrl: './portfolio-list.html',
  standalone: false
})
export class PortfolioList {
  @Input() portfolios: Portfolio[] = [];
}
