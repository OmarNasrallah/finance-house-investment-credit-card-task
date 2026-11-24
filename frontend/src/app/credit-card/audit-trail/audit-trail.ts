import { Component, Input } from '@angular/core';
import { AuditEntry } from '../../core/models';

@Component({
  selector: 'app-audit-trail',
  templateUrl: './audit-trail.html',
  standalone: false
})
export class AuditTrail {
  @Input() entries: AuditEntry[] = [];
}
