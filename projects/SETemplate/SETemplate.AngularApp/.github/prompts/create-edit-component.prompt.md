---
description: "Erstellt eine Angular Edit-Komponente (HTML-Template + TypeScript) für eine Entity mit Bootstrap-Card-Layout, Dropdowns für Fremdschlüssel/Enums und i18n-Übersetzungen"
name: "Edit-Komponente erstellen"
argument-hint: "Entity-Name (z.B. ToDoItem, Department)"
agent: "agent"
---

# Angular Edit-Komponente erstellen

Erstelle **HTML-Template und TypeScript** für die Edit-Komponente `$input` (bereits generiert unter `src/app/components/entities/`).

## Pflichtschritte – in dieser Reihenfolge

### 1. Interface ZUERST prüfen (KRITISCH!)
Öffne `src/app/models/entities/**/i-$input.ts` und notiere:
- Alle Properties und deren Typen
- Fremdschlüssel (z.B. `toDoListId: number`, `departmentId: number | null`)
- Navigation-Properties (z.B. `toDoList: IToDoList | null`)
- Enum-Felder (z.B. `priority: Priority`, `state: ToDoState`)
- Pflichtfelder (`required`) vs. optionale Felder

**Nur Properties aus dem Interface verwenden!** Keine Annahmen über Feldnamen.

### 2. HTML-Template erstellen
Dateiname: `[entity-name]-edit.component.html` in `src/app/components/entities/[subfolder]/`

```html
<!--@AiCode-->
<div *ngIf="dataItem" class="modern-edit-container">
    <div class="modern-edit-card">
        <div class="modern-edit-header d-flex justify-content-between align-items-center">
            <h3><i class="bi bi-[icon] me-2"></i>{{ title | translate }}</h3>
            <button type="button" class="btn-close-custom" (click)="dismiss()">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>
        <div class="modern-edit-body">
            <form (ngSubmit)="submitForm()" #editForm="ngForm">

                <!-- Form-Section pro Gruppe -->
                <div class="form-section">
                    <h4 class="form-section-title">
                        <i class="bi bi-info-circle"></i>
                        {{ 'ENTITYNAME_EDIT.BASIC_INFO' | translate }}
                    </h4>

                    <!-- Textfeld -->
                    <div class="modern-form-group">
                        <label for="fieldName" class="modern-form-label">
                            <i class="bi bi-pencil"></i>
                            {{ 'ENTITYNAME_EDIT.FIELDNAME' | translate }}
                        </label>
                        <input id="fieldName" class="form-control modern-form-control"
                               [(ngModel)]="dataItem.fieldName" name="fieldName" required />
                    </div>

                    <!-- Dropdown für Fremdschlüssel -->
                    <div class="modern-form-group">
                        <label for="relatedEntityId" class="modern-form-label">
                            <i class="bi bi-link"></i>
                            {{ 'ENTITYNAME_EDIT.RELATED_ENTITY' | translate }}
                        </label>
                        <select id="relatedEntityId" class="form-select modern-form-select"
                                [(ngModel)]="dataItem.relatedEntityId" name="relatedEntityId">
                            <option [ngValue]="null">{{ 'ENTITYNAME_EDIT.NO_SELECTION' | translate }}</option>
                            @for (e of relatedEntities; track e.id) {
                            <option [ngValue]="e.id">{{ e.name }}</option>
                            }
                        </select>
                    </div>

                    <!-- Dropdown für Enum -->
                    <div class="modern-form-group">
                        <label for="priority" class="modern-form-label">
                            <i class="bi bi-flag"></i>
                            {{ 'ENTITYNAME_EDIT.PRIORITY' | translate }}
                        </label>
                        <select id="priority" class="form-select modern-form-select"
                                [(ngModel)]="dataItem.priority" name="priority">
                            @for (p of priorities; track $index) {
                            <option [ngValue]="p">{{ p }}</option>
                            }
                        </select>
                    </div>

                    <!-- Datumsfeld -->
                    <div class="modern-form-group">
                        <label for="dueDate" class="modern-form-label">
                            <i class="bi bi-calendar"></i>
                            {{ 'ENTITYNAME_EDIT.DUE_DATE' | translate }}
                        </label>
                        <input type="date" id="dueDate" class="form-control modern-form-control"
                               [(ngModel)]="dataItem.dueDate" name="dueDate" />
                    </div>

                    <!-- Checkbox -->
                    <div class="modern-form-check">
                        <input type="checkbox" class="form-check-input modern-form-check-input"
                               id="isActive" [(ngModel)]="dataItem.isActive" name="isActive" />
                        <label class="modern-form-check-label" for="isActive">
                            {{ 'ENTITYNAME_EDIT.IS_ACTIVE' | translate }}
                        </label>
                    </div>
                </div>

                <!-- Aktions-Buttons -->
                <div class="modern-form-actions">
                    <button class="btn modern-btn-save" type="submit" [disabled]="!editForm.valid">
                        <i class="bi bi-check-lg me-2"></i>{{ 'COMMON.SAVE' | translate }}
                    </button>
                    <button class="btn modern-btn-cancel" type="button" (click)="cancelForm()">
                        <i class="bi bi-x-lg me-2"></i>{{ 'COMMON.CANCEL' | translate }}
                    </button>
                </div>
            </form>
        </div>
    </div>
</div>
```

### 3. TypeScript-Komponente erweitern
Dateiname: `[entity-name]-edit.component.ts`

```typescript
//@CustomCode
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { EntityNameBaseEditComponent } from '@app/components/entities/[subfolder]/entity-name-base-edit.component';
// Imports für Dropdown-Services und Enum-Typen:
// import { RelatedEntityService } from '@app-services/entities/[subfolder]/related-entity.service';
// import { Priority } from '@app/enums/priority';

@Component({
  standalone: true,
  selector: 'app-entity-name-edit',
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './entity-name-edit.component.html',
  styleUrl: './entity-name-edit.component.css'
})
export class EntityNameEditComponent extends EntityNameBaseEditComponent {

  // Arrays für Dropdowns
  // public relatedEntities: IRelatedEntity[] = [];
  // public priorities = Object.values(Priority).filter(v => typeof v === 'number') as number[];

  // Services injizieren
  // private relatedEntityService = inject(RelatedEntityService);

  public override get title(): string {
    return this.editMode ? 'ENTITYNAME_EDIT.EDIT_TITLE' : 'ENTITYNAME_EDIT.CREATE_TITLE';
  }

  // WICHTIG: override keyword verwenden!
  override ngOnInit(): void {
    super.ngOnInit();
    // this.loadRelatedEntities();
  }

  // private loadRelatedEntities(): void {
  //   this.relatedEntityService.getAll().subscribe({
  //     next: (data) => this.relatedEntities = data,
  //     error: (err) => console.error('Error loading related entities', err)
  //   });
  // }
}
```

**Regeln für TypeScript:**
- Bei Fremdschlüsseln: entsprechenden Service mit `inject()` laden
- Bei Enums: `Object.values(EnumType).filter(v => typeof v === 'number') as number[]`
- Lifecycle: IMMER `override ngOnInit()` mit `super.ngOnInit()` aufrufen
- Title-Property: gibt i18n-Key zurück (wird im Template mit `| translate` übersetzt)

### 4. i18n-Übersetzungen ergänzen
Ergänze **beide** Dateien gleichzeitig:
- `src/assets/i18n/de.json`
- `src/assets/i18n/en.json`

```json
"ENTITYNAME_EDIT": {
  "CREATE_TITLE": "Neues Entity erstellen",
  "EDIT_TITLE": "Entity bearbeiten",
  "BASIC_INFO": "Grundinformationen",
  "FIELDNAME": "Feldname",
  "IS_ACTIVE": "Aktiv"
}
```

## CSS-Regel
CSS-Datei der Komponente **leer lassen** – alle Styles kommen aus der globalen `styles.css`.

## Nach der Erstellung
- [ ] `npm run build` ausführen und alle Fehler beheben
