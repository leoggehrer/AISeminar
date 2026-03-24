---
description: "Erstellt eine Angular Split-View-Komponente: Master-Liste mit Filterleiste links, Detail-Ansicht mit CRUD rechts – alles in einer Seite ohne Navigation"
name: "Master-Liste + Details (Split-View) erstellen"
argument-hint: "Master-Entity und Detail-Entity (z.B. Student + Enrollments)"
agent: "agent"
---

# Angular Master-Liste + Details (Split-View) erstellen

Erstelle eine **neue standalone Komponente** für eine **zweispaltige Split-View** von `$input`.

> Links: scrollbare Master-Liste mit Suchfeld. Rechts: Details (readonly Info + bearbeitbare Detail-Liste) des selektierten Master-Eintrags.
> Diese Ansicht hat **keine generierte Basis-Komponente** – sie wird komplett neu erstellt.

## Pflichtschritte – in dieser Reihenfolge

### 1. Interfaces prüfen
Öffne beide Interfaces und notiere alle Properties:
- `src/app/models/entities/**/i-master-entity.ts` (links angezeigt + readonly Info rechts)
- `src/app/models/entities/**/i-detail-entity.ts` (CRUD-Liste rechts)

### 2. Neue Komponente anlegen
**Pfad:** `src/app/pages/entities/[subfolder]/master-detailsName.component.ts`
**Dateinamen-Muster:** `masterentity-detailsentity.component.*`
(z.B. `student-enrollments.component.ts`)

**TypeScript-Struktur:**
```typescript
//@AiCode
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { finalize } from 'rxjs';
import { IMasterEntity } from '@app-models/entities/[subfolder]/i-master-entity';
import { IDetailEntity } from '@app-models/entities/[subfolder]/i-detail-entity';
import { MasterEntityService } from '@app/services/http/entities/[subfolder]/master-entity-service';
import { DetailEntityService } from '@app/services/http/entities/[subfolder]/detail-entity-service';
import { DetailEntityEditComponent } from '@app/components/entities/[subfolder]/detail-entity-edit.component';
import { MessageBoxService } from '@app/services/message-box-service.service';
import { ErrorHandlerService } from '@app/services/error-handler.service';

@Component({
  standalone: true,
  selector: 'app-master-detailsname',
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule],
  templateUrl: './master-detailsname.component.html',
  styleUrl: './master-detailsname.component.css'
})
export class MasterDetailsnameComponent implements OnInit {
  public masters: IMasterEntity[] = [];
  public selectedMaster: IMasterEntity | null = null;
  public details: IDetailEntity[] = [];
  public searchTerm = '';
  public saveData = false;

  public canAdd = true;
  public canEdit = true;
  public canDelete = true;

  private masterService = inject(MasterEntityService);
  private detailService = inject(DetailEntityService);
  private modalService = inject(NgbModal);
  private messageBoxService = inject(MessageBoxService);
  private translateService = inject(TranslateService);
  private errorHandlerService = inject(ErrorHandlerService);

  ngOnInit(): void {
    this.loadMasters();
  }

  get filteredMasters(): IMasterEntity[] {
    if (!this.searchTerm.trim()) return this.masters;
    const term = this.searchTerm.toLowerCase();
    // Anpassen: Felder die durchsucht werden sollen
    return this.masters.filter(m =>
      m.[hauptfeld]?.toLowerCase().includes(term)
    );
  }

  loadMasters(): void {
    this.masterService.getAll().subscribe({
      next: (data) => this.masters = data,
      error: (err) => this.errorHandlerService.handleError(err)
    });
  }

  selectMaster(master: IMasterEntity): void {
    this.selectedMaster = master;
    this.loadDetails(master.id!);
  }

  private loadDetails(masterId: number): void {
    this.detailService.query({
      filter: 'masterId == @0',       // Fremdschlüsselfeld anpassen
      values: [masterId.toString()],
      includes: [],                    // Navigation-Properties ergänzen falls nötig
      sortBy: ''
    }).subscribe({
      next: (data) => this.details = data,
      error: (err) => this.errorHandlerService.handleError(err)
    });
  }

  addCommand(): void {
    this.detailService.getTemplate().subscribe({
      next: (template) => {
        template.masterId = this.selectedMaster!.id!;  // Fremdschlüsselfeld anpassen
        const modalRef = this.modalService.open(DetailEntityEditComponent, { size: 'lg', centered: true });
        const comp = modalRef.componentInstance;
        comp.dataItem = template;
        comp.save.subscribe((item: IDetailEntity) => {
          comp.saveData = true;
          this.detailService.create(item)
            .pipe(finalize(() => comp.saveData = false))
            .subscribe({
              next: () => { comp.close(); this.loadDetails(this.selectedMaster!.id!); },
              error: (err) => this.errorHandlerService.handleError(err)
            });
        });
      },
      error: (err) => this.errorHandlerService.handleError(err)
    });
  }

  editCommand(item: IDetailEntity): void {
    this.detailService.getById(item.id!).subscribe({
      next: (loaded) => {
        const modalRef = this.modalService.open(DetailEntityEditComponent, { size: 'lg', centered: true });
        const comp = modalRef.componentInstance;
        comp.dataItem = { ...loaded };
        comp.save.subscribe((updated: IDetailEntity) => {
          comp.saveData = true;
          this.detailService.update(updated)
            .pipe(finalize(() => comp.saveData = false))
            .subscribe({
              next: () => { comp.close(); this.loadDetails(this.selectedMaster!.id!); },
              error: (err) => this.errorHandlerService.handleError(err)
            });
        });
      },
      error: (err) => this.errorHandlerService.handleError(err)
    });
  }

  async deleteCommand(item: IDetailEntity): Promise<void> {
    const confirmed = await this.messageBoxService.confirm(
      this.translateService.instant('COMMON.DELETE_CONFIRM', { item: item.[hauptfeld] ?? item.id }),
      this.translateService.instant('COMMON.DELETE_TITLE')
    );
    if (confirmed) {
      this.detailService.delete(item).subscribe({
        next: () => this.loadDetails(this.selectedMaster!.id!),
        error: (err) => this.errorHandlerService.handleError(err)
      });
    }
  }
}
```

### 3. HTML-Template erstellen
```html
<!--@AiCode-->
<div class="list-container">
    <div class="container-fluid mt-4">

        <!-- Page Header mit Zurück-Button -->
        <div class="d-flex justify-content-between align-items-center mb-3 p-3 page-header">
            <h3 class="mb-0 flex-grow-1">
                <i class="bi bi-[master-icon] me-2"></i>
                {{ 'MASTERDETAILS.TITLE' | translate }}
            </h3>
            <button class="btn btn-link text-secondary p-1"
                    (click)="goBack()"
                    title="{{ 'COMMON.BACK' | translate }}">
                <i class="bi bi-arrow-left-circle-fill fs-5"></i>
            </button>
        </div>

        <div class="row g-3">

            <!-- ===== LINKE SPALTE: Master-Liste ===== -->
            <div class="col-12 col-md-4 col-xl-3">
                <div class="card h-100 shadow-sm">

                    <!-- Filterleiste -->
                    <div class="card-header py-2">
                        <div class="input-group input-group-sm">
                            <span class="input-group-text bg-transparent border-end-0">
                                <i class="bi bi-search text-muted"></i>
                            </span>
                            <input type="text"
                                   class="form-control border-start-0 ps-0"
                                   [(ngModel)]="searchTerm"
                                   [placeholder]="'MASTERDETAILS.SEARCH_PLACEHOLDER' | translate"
                                   title="{{ 'MASTERDETAILS.SEARCH_PLACEHOLDER' | translate }}">
                            @if (searchTerm) {
                            <button class="btn btn-outline-secondary border-start-0"
                                    (click)="searchTerm = ''"
                                    title="{{ 'COMMON.CLEAR' | translate }}">
                                <i class="bi bi-x-lg"></i>
                            </button>
                            }
                        </div>
                    </div>

                    <!-- Master-Einträge -->
                    <div class="card-body p-0 overflow-auto" style="max-height: calc(100vh - 220px);">
                        <div class="list-group list-group-flush">
                            @for (master of filteredMasters; track master.id ?? $index) {
                            <button type="button"
                                    class="list-group-item list-group-item-action px-3 py-2"
                                    [class.active]="selectedMaster?.id === master.id"
                                    (click)="selectMaster(master)">
                                <div class="d-flex align-items-center gap-2">
                                    <i class="bi bi-[master-icon] flex-shrink-0"></i>
                                    <div class="overflow-hidden">
                                        <div class="fw-semibold text-truncate">{{ master.[hauptfeld] }}</div>
                                        <!-- Optional: zweite Zeile mit Zusatzinfo -->
                                        <small class="text-muted text-truncate d-block">{{ master.[zusatzfeld] ?? '' }}</small>
                                    </div>
                                </div>
                            </button>
                            }

                            @if (filteredMasters.length === 0) {
                            <div class="empty-state py-4">
                                <div class="empty-state-icon"><i class="bi bi-search"></i></div>
                                <p class="empty-state-text small">{{ 'MASTERDETAILS.NO_MASTERS_FOUND' | translate }}</p>
                            </div>
                            }
                        </div>
                    </div>

                    <!-- Footer: Anzahl -->
                    <div class="card-footer py-1 text-muted small text-end">
                        {{ filteredMasters.length }} / {{ masters.length }}
                    </div>

                </div>
            </div>

            <!-- ===== RECHTE SPALTE: Detail-Ansicht ===== -->
            <div class="col-12 col-md-8 col-xl-9">

                @if (!selectedMaster) {
                <!-- Platzhalter wenn nichts gewählt -->
                <div class="card h-100 shadow-sm">
                    <div class="card-body d-flex flex-column justify-content-center align-items-center text-muted py-5">
                        <i class="bi bi-arrow-left-circle fs-1 mb-3 opacity-25"></i>
                        <p class="mb-0">{{ 'MASTERDETAILS.SELECT_MASTER_HINT' | translate }}</p>
                    </div>
                </div>
                }

                @if (selectedMaster) {
                <!-- Master-Info Card (readonly) -->
                <div class="modern-edit-card mb-3">
                    <div class="modern-edit-header d-flex justify-content-between align-items-center">
                        <h5 class="mb-0">
                            <i class="bi bi-[master-icon] me-2"></i>
                            {{ selectedMaster.[hauptfeld] }}
                        </h5>
                        <!-- Optional: Link zur Edit-Seite des Master -->
                        <!-- <a [routerLink]="['/master-route', selectedMaster.id, 'edit']"
                              class="btn btn-sm btn-outline-secondary"
                              title="{{ 'COMMON.EDIT' | translate }}">
                            <i class="bi bi-pencil"></i>
                        </a> -->
                    </div>
                    <div class="modern-edit-body">
                        <div class="list-item-details">
                            <!-- detail-row pro sichtbarer Eigenschaft des Master -->
                            <div class="detail-row">
                                <span class="detail-label">
                                    <i class="bi bi-[feld-icon] me-1"></i>
                                    {{ 'MASTER_EDIT.[FELD]' | translate }}
                                </span>
                                <span class="detail-value">{{ selectedMaster.[feld] }}</span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Detail-Toolbar -->
                <div class="d-flex justify-content-between align-items-center mb-3 p-3 page-header">
                    <h6 class="mb-0">
                        <i class="bi bi-[detail-icon] me-2"></i>
                        {{ 'MASTERDETAILS.DETAIL_TITLE' | translate }}
                        <span class="badge bg-primary ms-2">{{ details.length }}</span>
                    </h6>
                    @if (canAdd) {
                    <button class="btn btn-primary btn-sm"
                            (click)="addCommand()"
                            title="{{ 'MASTERDETAILS.ADD_DETAIL' | translate }}">
                        <i class="bi bi-plus-circle me-1 me-sm-2"></i>
                        <span class="d-none d-sm-inline">{{ 'MASTERDETAILS.ADD_DETAIL' | translate }}</span>
                    </button>
                    }
                </div>

                <!-- Detail-Liste -->
                <div class="list-items">
                    @for (item of details; track item.id ?? $index) {
                    <div class="modern-list-item">
                        <div class="list-item-header">
                            <div class="list-item-icon">
                                <i class="bi bi-[detail-icon]"></i>
                            </div>
                            <div class="flex-grow-1">
                                <h5 class="list-item-title">{{ item.[hauptfeld] }}</h5>
                                <!-- Optional: Zusatzinfo oder Badge -->
                            </div>
                        </div>
                        <div class="list-item-details">
                            <!-- detail-row pro Eigenschaft des Detail-Eintrags -->
                            <div class="detail-row">
                                <span class="detail-label">
                                    <i class="bi bi-[feld-icon] me-1"></i>
                                    {{ 'DETAIL_EDIT.[FELD]' | translate }}
                                </span>
                                <span class="detail-value">{{ item.[feld] }}</span>
                            </div>
                        </div>
                        <div class="list-item-actions">
                            @if (canEdit) {
                            <button class="btn btn-outline-secondary"
                                    (click)="editCommand(item)"
                                    title="{{ 'COMMON.EDIT' | translate }}">
                                <i class="bi bi-pencil me-2"></i>{{ 'COMMON.EDIT' | translate }}
                            </button>
                            }
                            @if (canDelete) {
                            <button class="btn btn-outline-danger"
                                    (click)="deleteCommand(item)"
                                    title="{{ 'COMMON.DELETE' | translate }}">
                                <i class="bi bi-trash me-2"></i>{{ 'COMMON.DELETE' | translate }}
                            </button>
                            }
                        </div>
                    </div>
                    }

                    @if (details.length === 0) {
                    <div class="empty-state">
                        <div class="empty-state-icon"><i class="bi bi-inbox"></i></div>
                        <p class="empty-state-text">{{ 'MASTERDETAILS.NO_DETAILS' | translate }}</p>
                    </div>
                    }
                </div>
                }

            </div>
        </div><!-- /row -->

    </div>
</div>
```

### 4. CSS-Datei
Die Komponenten-CSS-Datei leer lassen – alle Styles kommen aus der globalen `styles.css`.

```css
/* master-detailsname.component.css */
/* Leer – Alle Styles kommen aus der globalen styles.css */
```

### 5. Route hinzufügen
In `app-routing.module.ts`:
```typescript
{
  path: 'master-detailsname',
  component: MasterDetailsnameComponent,
  canActivate: [AuthGuard],
  title: 'MasterDetailsname'
}
```

### 6. i18n-Übersetzungen ergänzen (beide Sprachen gleichzeitig!)
```json
// de.json
"MASTERDETAILS": {
  "TITLE": "Master – Details",
  "SEARCH_PLACEHOLDER": "Suchen...",
  "NO_MASTERS_FOUND": "Keine Einträge gefunden",
  "SELECT_MASTER_HINT": "Bitte einen Eintrag aus der Liste wählen",
  "DETAIL_TITLE": "Details",
  "ADD_DETAIL": "Hinzufügen",
  "NO_DETAILS": "Keine Einträge vorhanden"
}

// en.json
"MASTERDETAILS": {
  "TITLE": "Master – Details",
  "SEARCH_PLACEHOLDER": "Search...",
  "NO_MASTERS_FOUND": "No entries found",
  "SELECT_MASTER_HINT": "Please select an entry from the list",
  "DETAIL_TITLE": "Details",
  "ADD_DETAIL": "Add",
  "NO_DETAILS": "No entries available"
}
```

---

## Wichtige Anpassungen je Entity

### Suchlogenik anpassen (`filteredMasters` Getter)
Filtere nach den relevanten Textfeldern der Master-Entity:
```typescript
// Beispiel: Suche über mehrere Felder
return this.masters.filter(m =>
  m.name?.toLowerCase().includes(term) ||
  m.description?.toLowerCase().includes(term)
);
```

### Fremdschlüsselfilter anpassen (`loadDetails`)
Das `filter`-Feld muss dem tatsächlichen Fremdschlüssel-Property entsprechen:
```typescript
// Beispiel: studentId, departmentId, courseId
filter: 'studentId == @0'
```

### Navigation-Properties in `includes` ergänzen
Falls Detail-Items Navigation-Properties (z.B. `course`, `category`) benötigen:
```typescript
includes: ['Course', 'Category'],
```

---

## Responsive Verhalten

| Breakpoint | Layout |
|------------|--------|
| `< md` (Mobile) | Master-Liste und Detail-Panel untereinander (Vollbreite) |
| `≥ md` (Tablet) | Links 4 Spalten (Master), Rechts 8 Spalten (Details) |
| `≥ xl` (Desktop) | Links 3 Spalten (Master), Rechts 9 Spalten (Details) |

Die Master-Liste scrollt intern (`overflow-auto`, `max-height: calc(100vh - 220px)`), sodass die Seite auf großen Bildschirmen nicht scrollt.

---

## Unterschied zu `create-master-details.prompt.md`

| | `create-master-details` | **`create-masterlist-details`** |
|--|--|--|
| Layout | Vertikal: Master-Card oben, Details unten | **Horizontal: Master-Liste links, Details rechts** |
| Navigation | Via Route-Parameter (`/:id`) | **Klick in Liste → Selection-State** |
| Master | Ein einzelner Master (per URL) | **Scrollbare Liste aller Master** |
| Filter | Keiner | **Suchfeld über Master-Liste** |
| Verwendung | Direktlink aus einer anderen Liste | **Standalone Split-View-Seite** |

---

## Nach der Erstellung – Checkliste

- [ ] `filteredMasters` Getter: Suchfelder der Master-Entity angepasst
- [ ] `loadDetails`: Fremdschlüsselfeld korrekt gesetzt
- [ ] `loadDetails`: benötigte `includes` eingetragen
- [ ] HTML: Alle `[hauptfeld]`, `[zusatzfeld]`, `[feld]`, `[icon]` Platzhalter ersetzt
- [ ] `detail-row` Blöcke für alle relevanten Properties ergänzt
- [ ] Route in `app-routing.module.ts` eingetragen
- [ ] Dashboard-Card ergänzt (→ `add-dashboard-cards.prompt.md`)
- [ ] i18n-Keys in `de.json` **und** `en.json` ergänzt
- [ ] `npm run build` ausführen und alle Fehler beheben
