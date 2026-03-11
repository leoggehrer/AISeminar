---
description: "Erstellt eine Angular Master-Details-Komponente (neue standalone Seite) mit readonly Master-Ansicht und bearbeitbarer Detail-Liste"
name: "Master-Details-Komponente erstellen"
argument-hint: "Master-Entity und Detail-Entity (z.B. Department + Employees)"
agent: "agent"
---

# Angular Master-Details-Komponente erstellen

Erstelle eine **neue standalone Komponente** für die Master-Details-Ansicht von `$input`.

> Diese Ansicht hat **keine generierte Basis-Komponente** – sie wird komplett neu erstellt.

## Pflichtschritte – in dieser Reihenfolge

### 1. Interfaces prüfen
Öffne beide Interfaces und notiere alle Properties:
- `src/app/models/entities/**/i-master-entity.ts` (readonly angezeigt)
- `src/app/models/entities/**/i-detail-entity.ts` (CRUD mit Aktionsbuttons)

### 2. Neue Komponente anlegen
**Pfad:** `src/app/pages/entities/[subfolder]/master-detailsName.component.ts`  
**Dateinamen-Muster:** `masterentity-detailsentity.component.*`  
(z.B. `department-employees.component.ts`)

**TypeScript-Struktur:**
```typescript
//@CustomCode
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IMasterEntity } from '@app-models/entities/[subfolder]/i-master-entity';
import { IDetailEntity } from '@app-models/entities/[subfolder]/i-detail-entity';
import { MasterEntityService } from '@app-services/entities/[subfolder]/master-entity.service';
import { DetailEntityService } from '@app-services/entities/[subfolder]/detail-entity.service';
import { DetailEntityEditComponent } from '@app/components/entities/[subfolder]/detail-entity-edit.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MessageBoxServiceService } from '@app-services/message-box-service.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  standalone: true,
  selector: 'app-master-details',
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule],
  templateUrl: './master-detailsName.component.html',
  styleUrl: './master-detailsName.component.css'
})
export class MasterDetailsComponent implements OnInit {
  public master: IMasterEntity | null = null;
  public details: IDetailEntity[] = [];

  public canAdd = true;
  public canEdit = true;
  public canDelete = true;

  private masterService = inject(MasterEntityService);
  private detailService = inject(DetailEntityService);
  private route = inject(ActivatedRoute);
  private modalService = inject(NgbModal);
  private messageBoxService = inject(MessageBoxServiceService);
  private translateService = inject(TranslateService);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadMaster(id);
    this.loadDetails(id);
  }

  private loadMaster(id: number): void {
    this.masterService.getById(id).subscribe({
      next: (data) => this.master = data,
      error: (err) => console.error('Error loading master', err)
    });
  }

  private loadDetails(masterId: number): void {
    // Filter nach masterId anpassen
    this.detailService.getAll({ masterId }).subscribe({
      next: (data) => this.details = data,
      error: (err) => console.error('Error loading details', err)
    });
  }

  addCommand(): void {
    const modalRef = this.modalService.open(DetailEntityEditComponent, { size: 'lg' });
    modalRef.componentInstance.dataItem = { masterId: this.master?.id };
    modalRef.result.then(() => this.loadDetails(this.master!.id), () => {});
  }

  editCommand(item: IDetailEntity): void {
    const modalRef = this.modalService.open(DetailEntityEditComponent, { size: 'lg' });
    modalRef.componentInstance.dataItem = { ...item };
    modalRef.result.then(() => this.loadDetails(this.master!.id), () => {});
  }

  deleteCommand(item: IDetailEntity): void {
    const title = this.translateService.instant('COMMON.DELETE_TITLE');
    const message = this.translateService.instant('COMMON.DELETE_CONFIRM');
    this.messageBoxService.confirm(title, message).then(confirmed => {
      if (confirmed) {
        this.detailService.delete(item.id).subscribe(() => this.loadDetails(this.master!.id));
      }
    });
  }
}
```

### 3. HTML-Template erstellen
```html
<!--@AiCode-->
<div class="list-container">
    <div class="container mt-4" *ngIf="master">
        <!-- Master: Readonly-Ansicht -->
        <div class="modern-edit-card mb-4">
            <div class="modern-edit-header d-flex justify-content-between align-items-center">
                <h4 class="mb-0">
                    <i class="bi bi-[master-icon] me-2"></i>
                    {{ master.[hauptfeld] }}
                </h4>
                <a routerLink="/[master-route]" class="btn btn-light">
                    <i class="bi bi-arrow-left-circle me-2"></i>
                    <span class="d-none d-md-inline">{{ 'COMMON.BACK' | translate }}</span>
                </a>
            </div>
            <div class="modern-edit-body">
                <div class="list-item-details">
                    <!-- detail-row pro sichtbarer Eigenschaft -->
                </div>
            </div>
        </div>

        <!-- Details: Liste mit CRUD -->
        <div class="d-flex justify-content-between align-items-center mb-3 p-3 page-header">
            <h5 class="mb-0">
                <i class="bi bi-[detail-icon] me-2"></i>
                {{ 'MASTER_DETAILS.DETAIL_TITLE' | translate }}
                <span class="badge bg-primary ms-2">{{ details.length }}</span>
            </h5>
            <button *ngIf="canAdd" class="btn btn-primary" (click)="addCommand()">
                <i class="bi bi-plus-circle me-2"></i>
                <span class="d-none d-sm-inline">{{ 'MASTER_DETAILS.ADD_DETAIL' | translate }}</span>
            </button>
        </div>

        <div class="list-items">
            <div *ngFor="let item of details" class="modern-list-item">
                <div class="list-item-header">
                    <div class="list-item-icon"><i class="bi bi-[detail-icon]"></i></div>
                    <div class="flex-grow-1">
                        <h5 class="list-item-title">{{ item.[hauptfeld] }}</h5>
                    </div>
                </div>
                <div class="list-item-details">
                    <!-- detail-rows -->
                </div>
                <div class="list-item-actions">
                    <button *ngIf="canEdit" class="btn btn-outline-secondary" (click)="editCommand(item)">
                        <i class="bi bi-pencil me-2"></i>{{ 'COMMON.EDIT' | translate }}
                    </button>
                    <button *ngIf="canDelete" class="btn btn-outline-danger" (click)="deleteCommand(item)">
                        <i class="bi bi-trash me-2"></i>{{ 'COMMON.DELETE' | translate }}
                    </button>
                </div>
            </div>
            <div *ngIf="details.length === 0" class="empty-state">
                <div class="empty-state-icon"><i class="bi bi-inbox"></i></div>
                <p class="empty-state-text">{{ 'MASTER_DETAILS.NO_DETAILS' | translate }}</p>
            </div>
        </div>
    </div>
</div>
```

### 4. Route hinzufügen
In `app-routing.module.ts`:
```typescript
{ path: 'master/:id/details', component: MasterDetailsComponent, canActivate: [AuthGuard], title: 'Details' }
```

### 5. i18n-Übersetzungen ergänzen (beide Sprachen!)
```json
"MASTER_DETAILS": {
  "DETAIL_TITLE": "Details",
  "ADD_DETAIL": "Hinzufügen",
  "NO_DETAILS": "Keine Einträge vorhanden"
}
```

## Nach der Erstellung
- [ ] Route in `app-routing.module.ts` eintragen
- [ ] Link in Master-List-Ansicht ergänzen (z.B. `routerLink="/master/{{item.id}}/details"`)
- [ ] `npm run build` ausführen und alle Fehler beheben
