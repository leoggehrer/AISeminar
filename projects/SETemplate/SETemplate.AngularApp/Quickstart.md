# SETemplate Angular – Quickstart

> Angular-Entwicklung beginnt **nach** der Backend-Code-Generierung.
> Für Backend-Schritte siehe `Quickstart.md` im Root-Verzeichnis.

---

## Voraussetzung

Backend-Code wurde generiert:

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

Generierte Dateien befinden sich in:

| Pfad | Inhalt |
|------|--------|
| `src/app/components/entities/` | Basis-Edit-Komponenten (generiert) |
| `src/app/pages/entities/` | Basis-List-Seiten (generiert) |
| `src/app/services/http/` | API-Services (generiert) |
| `src/app/models/entities/` | TypeScript-Interfaces (generiert) |

---

## Pro Entity: Workflow

### Schritt 1 – Vor-Template-Checkliste

**VOR jeder Template-Erstellung** das Interface analysieren:

- Model öffnen: `src/app/models/entities/**/i-entity-name.ts`
- Alle Properties notieren (Typ, Pflicht, optional)
- Fremdschlüssel identifizieren → Dropdown erforderlich
- Enum-Felder identifizieren → Dropdown mit Enum-Werten

→ Details: `.github/prompts/pre-template-checklist.prompt.md`

---

### Schritt 2 – List-Komponente erstellen

HTML-Template für: `src/app/pages/entities/[subfolder]/entity-name-list.component.html`

**Pflichtstruktur:**

```html
<!--@AiCode-->
<div class="list-container">
    <div class="container mt-4">
        <!-- Page Header: Titel + dezenter Zurück-Button -->
        <div class="d-flex justify-content-between align-items-center mb-4 p-3 page-header">
            <h3 class="mb-0">{{ 'ENTITYNAME_LIST.TITLE' | translate }}</h3>
            <button class="btn btn-link text-secondary p-1"
                    (click)="goBack()"
                    title="{{ 'COMMON.BACK' | translate }}">
                <i class="bi bi-arrow-left-circle-fill fs-5"></i>
            </button>
        </div>

        <!-- Toolbar: Hinzufügen + Suche + Aktualisieren – alle in einer Zeile -->
        <div class="list-toolbar">
            <div class="d-flex align-items-center gap-2">
                @if (canAdd) {
                <button class="btn btn-primary"
                        (click)="addCommand()"
                        title="{{ 'ENTITYNAME_LIST.ADD_ITEM' | translate }}">
                    <i class="bi bi-plus-circle me-1 me-sm-2"></i>
                    <span class="d-none d-sm-inline">{{ 'ENTITYNAME_LIST.ADD_ITEM' | translate }}</span>
                </button>
                }
                @if (canSearch) {
                <div class="flex-grow-1">
                    <input type="text" class="form-control"
                           [(ngModel)]="searchTerm"
                           [placeholder]="'ENTITYNAME_LIST.SEARCH_PLACEHOLDER' | translate"
                           title="{{ 'ENTITYNAME_LIST.SEARCH_PLACEHOLDER' | translate }}">
                </div>
                }
                @if (canRefresh) {
                <button class="btn btn-outline-secondary"
                        (click)="reloadData()"
                        title="{{ 'ENTITYNAME_LIST.REFRESH' | translate }}">
                    <i class="bi bi-arrow-clockwise"></i>
                </button>
                }
            </div>
        </div>

        <!-- List Items -->
        @for (item of dataItems; track item.id ?? $index) {
            <div class="modern-list-item">
                <!-- Felder anzeigen -->
            </div>
        }

        <!-- Empty State -->
        @if (dataItems.length === 0) {
        <div class="empty-state">
            <i class="bi bi-inbox empty-state-icon"></i>
            <p class="empty-state-text">{{ 'COMMON.NO_DATA' | translate }}</p>
        </div>
        }
    </div>
</div>
```

**Wichtigste Regeln:**
- `@if/@for` – NICHT `*ngIf/*ngFor`
- `track item.id ?? $index` bei `@for`
- CSS-Datei leer lassen (alle Styles kommen aus `styles.css`)
- Globale CSS-Klassen: `.list-container`, `.modern-list-item`, `.page-header`, `.empty-state`

→ Details: `.github/prompts/create-list-component.prompt.md`

---

### Schritt 3 – Edit-Formular erstellen

HTML-Template für: `src/app/components/entities/[subfolder]/entity-name-edit.component.html`

**Pflichtstruktur:**

```html
<!--@AiCode-->
@if (dataItem) {
<div class="modern-edit-container">
    <div class="modern-edit-card">
        <div class="modern-edit-header d-flex justify-content-between align-items-center">
            <h3>{{ title | translate }}</h3>
            <button type="button" class="btn-close-custom" (click)="dismiss()">&times;</button>
        </div>
        <div class="modern-edit-body">
            <div class="form-section">
                <!-- Textfelder, Dropdowns, Checkboxen -->
            </div>
        </div>
        <div class="modern-edit-footer">
            <button class="modern-btn-save" (click)="saveCommand()">{{ 'COMMON.SAVE' | translate }}</button>
            <button class="modern-btn-cancel" (click)="cancelCommand()">{{ 'COMMON.CANCEL' | translate }}</button>
        </div>
    </div>
</div>
}
```

**Fremdschlüssel → Dropdown:**

```html
<!-- Im TypeScript: categories: ICategory[] = []; laden via override ngOnInit() -->
<select class="modern-form-select" [(ngModel)]="dataItem.categoryId">
    <option [ngValue]="null">{{ 'COMMON.PLEASE_SELECT' | translate }}</option>
    @for (cat of categories; track cat.id) {
    <option [ngValue]="cat.id">{{ cat.name }}</option>
    }
</select>
```

**TypeScript für Dropdowns (`entity-name-edit.component.ts`):**

```typescript
//@AiCode
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { EntityNameBaseEditComponent } from '@app/components/entities/.../entity-name-base-edit.component';
import { IRelatedEntity } from '@app-models/entities/.../i-related-entity';
import { RelatedEntityService } from '@app-services/http/.../related-entity.service';

@Component({
  standalone: true,
  selector: 'app-entity-name-edit',
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './entity-name-edit.component.html',
  styleUrl: './entity-name-edit.component.css'
})
export class EntityNameEditComponent extends EntityNameBaseEditComponent {
  relatedEntities: IRelatedEntity[] = [];

  constructor(private relatedEntityService: RelatedEntityService) {
    super();
  }

  override ngOnInit(): void {
    super.ngOnInit();
    this.relatedEntityService.getAll().subscribe(r => this.relatedEntities = r.items ?? []);
  }

  override get title(): string {
    return this.editMode ? 'ENTITYNAME_EDIT.EDIT_TITLE' : 'ENTITYNAME_EDIT.CREATE_TITLE';
  }
}
```

→ Details: `.github/prompts/create-edit-component.prompt.md`

---

### Schritt 4 – Routing konfigurieren

Route in `src/app/app-routing.module.ts` eintragen.

→ Details: `.github/prompts/configure-routing.prompt.md`

---

### Schritt 5 – Dashboard-Cards ergänzen

Card-Link im Dashboard für jede neue Entity eintragen.

→ Details: `.github/prompts/add-dashboard-cards.prompt.md`

---

### Schritt 6 – i18n-Übersetzungen

Keys **gleichzeitig** in `src/assets/i18n/de.json` und `en.json` ergänzen.

**Naming-Konvention:**

| Bereich | Schlüssel-Muster |
|---------|-----------------|
| Listen-Titel | `ENTITYNAME_LIST.TITLE` |
| Hinzufügen-Button | `ENTITYNAME_LIST.ADD_ITEM` |
| Suchfeld | `ENTITYNAME_LIST.SEARCH_PLACEHOLDER` |
| Edit-Titel (bearbeiten) | `ENTITYNAME_EDIT.EDIT_TITLE` |
| Edit-Titel (neu) | `ENTITYNAME_EDIT.CREATE_TITLE` |
| Einzelne Felder | `ENTITYNAME_EDIT.FIELDNAME` |

→ Details: `.github/prompts/add-i18n-translations.prompt.md`

---

### Schritt 7 – Build & Test

```bash
cd SETemplate.AngularApp
npm run build
# Für Entwicklung:
npm start
```

---

## Optional: Master-Details

Wenn eine Entity Unterelemente hat (z.B. `ToDoList` → `ToDoItems`):

→ Details: `.github/prompts/create-master-details.prompt.md`

---

## Kurzreferenz – Wichtige Syntax

### Control Flow (VERPFLICHTEND)

```html
<!-- ✅ RICHTIG -->
@if (canAdd) { ... }
@for (item of dataItems; track item.id ?? $index) { ... }

<!-- ❌ FALSCH -->
<div *ngIf="canAdd">
<div *ngFor="let item of dataItems">
```

### Translate Pipe

```html
<!-- ✅ RICHTIG: Immer Pipe im Template -->
{{ 'KEY' | translate }}
{{ title | translate }}

<!-- ❌ FALSCH: Nur für MessageBox-Parameter erlaubt -->
translateService.instant('KEY')
```

### override-Keyword

```typescript
// ✅ IMMER override bei überschriebenen Methoden/Properties
override ngOnInit(): void { ... }
override get title(): string { ... }
override prepareQueryParams(queryParams: IQueryParams): void { ... }
```

---

## Code-Marker (Angular)

| Marker | Bedeutung |
|--------|-----------|
| `<!--@AiCode-->` | Von AI erstellt – nicht generiert |
| `<!--@GeneratedCode-->` | Generiert – **nicht bearbeiten** |
| `<!--@CustomCode-->` | Manuell angepasst – wird nie überschrieben |
| `//@AiCode` | TypeScript: von AI erstellt |
| `//@CustomCode` | TypeScript: geschützte Anpassung |

---

## Troubleshooting

| Problem | Lösung |
|---------|--------|
| `Missing override modifier` | `ngOnInit`, `title` etc. mit `override` deklarieren |
| Dropdown leer | `override ngOnInit()` mit `super.ngOnInit()` aufrufen |
| `*ngIf/*ngFor` Warnung | Auf `@if/@for` umstellen |
| Translate-Key nicht gefunden | Key in `de.json` **und** `en.json` ergänzen |
| Navigation Property undefined | Fremdschlüssel-Service in `ngOnInit()` laden |

→ Details: `.github/prompts/fix-common-errors.prompt.md`
