# GitHub Copilot Instructions für SETemplate Angular App

> **Version:** 3.0 | **Letzte Aktualisierung:** März 2026

## Projektübersicht

SETemplate Angular App ist das Frontend für das SETemplate-System:

- **Framework**: Angular 19 mit Bootstrap 5 und standalone Komponenten
- **UI-Framework**: NgBootstrap mit Bootstrap Icons
- **Architektur**: Standalone Komponenten mit strikter Trennung von generierten und manuellen Code
- **Code-Generierung**: Template-gesteuerte Erstellung aller CRUD-Komponenten
- **Internationalisierung**: i18n mit DE/EN Unterstützung

> Für Schritt-für-Schritt-Workflows siehe `.github/prompts/` (create-list-component, create-edit-component, configure-routing, etc.)

## Kernprinzipien

### 1. Code-Generierung First (KRITISCH!)

**⚠️ NIEMALS manuell CRUD-Komponenten oder Services erstellen!**

Alle CRUD-Komponenten, Services und Models werden automatisch vom Backend-Generator erstellt:
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

### 2. Code-Marker System

- `<!--@AiCode-->` - Von der AI Generierter HTML-Code
- `//@AiCode` - Von der AI Generierter TypeScript-Code
- `//@GeneratedCode` - **NICHT BEARBEITEN!** Wird bei Generierung überschrieben
- `//@CustomCode` - Manuell angepasster Code, wird **nicht** überschrieben

### 3. Standalone Komponenten

- **Alle Komponenten sind standalone** - keine Module verwenden
- **Immer separate HTML- und CSS-Dateien** verwenden
- **CommonModule** und andere Angular Modules direkt importieren

### 4. Angular Template-Syntax (NEU - VERPFLICHTEND)

- **Immer die neue Control-Flow-Syntax verwenden:** `@if`, `@for`, `@switch`
- **Legacy-Syntax vermeiden:** `*ngIf`, `*ngFor`, `*ngSwitch`
- Bei Schleifen immer `track` verwenden (z.B. `track item.id` oder `track $index`)

```html
<!-- ✅ RICHTIG -->
@if (canAdd) {
    <button class="btn btn-primary" (click)="addCommand()">Add</button>
}

@for (item of dataItems; track item.id ?? $index) {
    <div>{{ item.name }}</div>
}
```

```html
<!-- ❌ FALSCH -->
<button *ngIf="canAdd" class="btn btn-primary" (click)="addCommand()">Add</button>
<div *ngFor="let item of dataItems">{{ item.name }}</div>
```

## Projektstruktur

```
src/app/
├── components/          # Generierte Edit-Komponenten
│   └── entities/        # Entity-spezifische Edit-Komponenten
├── pages/               # Generierte List-Komponenten  
│   └── entities/        # Entity-spezifische List-Komponenten
├── services/            # Generierte API-Services
├── models/              # TypeScript-Interfaces für Entities
├── shared/              # Gemeinsame Komponenten und Services
└── assets/
    └── i18n/            # Übersetzungsdateien (de.json, en.json)
```

## Angular Komponenten

Der Code-Generator erstellt automatisch die Angular-Komponenten für die Entitäten basierend auf den definierten Entities im Backend.

Für die generierten Angular-Komponenten sind folgende Regeln zu beachten:

- Im Ordner `src/app/components/entities/` befinden sich die Basis-Komponenten für die Edit-Formulare (z.B.: employee-base-edit.component.ts).
- Im Ordner `src/app/components/entities/` befinden sich die Edit-Komponenten für die Edit-Formulare (z.B.: employee-edit.component.ts).
- Im Ordner `src/app/components/entities/` befinden sich die Basis-Komponenten für die Listen-Ansicht (z.B.: employee-base-list.component.ts).
- Im Ordner `src/app/pages/entities/` befinden sich die Seiten-Komponenten für die Listen-Ansicht (z.B.: employee-list.component.ts).

**Hinweis:** Wenn die Entitäten in Unterordnern liegen, werden die entsprechenden Komponenten und Seiten in entsprechenden Unterordnern erstellt.

### CSS-Regeln (KRITISCH!)

**Grundprinzip:** Alle Standard-Styles werden aus der **globalen `styles.css`** verwendet. Komponenten-CSS-Dateien bleiben **leer oder minimal**.

#### ✅ Globale Styles verwenden für:

- **Alle Formular-Layouts** (`.modern-form-group`, `.modern-form-control`, etc.)
- **Alle Listen-Layouts** (`.list-container`, `.modern-list-item`, etc.)
- **Alle Edit-Komponenten** (`.modern-edit-card`, `.modern-edit-header`, etc.)
- **Bootstrap-Klassen** (`btn`, `form-control`, `card`, etc.)
- **Gemeinsame Patterns** (`.page-header`, `.list-toolbar`, `.empty-state`, etc.)
- **Farbschemata und Themes**
- **Typografie und Schriftarten**
- **Responsive Breakpoints**

#### ❌ Nur komponentenspezifisches CSS für:

- **Einzigartige Animationen** die nur in dieser Komponente vorkommen
- **Spezielle Layout-Ausnahmen** die nicht in anderen Komponenten verwendet werden
- **Komponenten-spezifische Positionierungen** für Sonderfälle

**Beispiel für komponentenspezifisches CSS (Ausnahmefall):**

```css
/* entity-name-edit.component.css */
/* NUR für spezielle Anforderungen, die nicht global gelten */
.special-chart-container {
  position: relative;
  height: 400px;
}

@keyframes specialPulse {
  0% { opacity: 0.6; }
  50% { opacity: 1; }
  100% { opacity: 0.6; }
}
```

**Standard-Fall: CSS-Datei leer lassen!**

```css
/* entity-name-list.component.css */
/* Leer - Alle Styles kommen aus der globalen styles.css */
```

#### Wichtige globale CSS-Klassen (aus styles.css):

| Bereich                 | Klassen                                                                                                           |
| ----------------------- | ----------------------------------------------------------------------------------------------------------------- |
| **Listen**        | `.list-container`, `.modern-list-item`, `.list-item-header`, `.list-item-details`, `.list-item-actions` |
| **Formulare**     | `.modern-edit-container`, `.modern-edit-card`, `.modern-edit-header`, `.modern-edit-body`                 |
| **Form-Elemente** | `.modern-form-group`, `.modern-form-label`, `.modern-form-control`, `.modern-form-select`                 |
| **Buttons**       | `.modern-btn-save`, `.modern-btn-cancel`, `.btn-close-custom`                                               |
| **Sections**      | `.form-section`, `.form-section-title`                                                                        |
| **Zustände**     | `.empty-state`, `.empty-state-icon`, `.empty-state-text`                                                    |
| **Header**        | `.page-header`, `.list-toolbar`                                                                               |

### List Component Template

→ Vollständiges Template und Schritte: `.github/prompts/create-list-component.prompt.md`

**Regeln:**
- Dateiname: `entity-name-list.component.html` in `src/app/pages/entities/[subfolder]/`
- Struktur: Page-Header + Toolbar (Hinzufügen/Suche/Refresh) + List-Items + Empty-State
- IMMER `@if/@for` Syntax (kein `*ngIf/*ngFor`)
- CSS-Datei leer lassen
- Beide `de.json` und `en.json` gleichzeitig ergänzen

**Toolbar-Regeln (KRITISCH!):**
- Alle Toolbar-Elemente (Add-Button, Suchfeld, Refresh-Button) **immer in einer Zeile** mit `d-flex align-items-center gap-2`
- Jeder Button besitzt ein **Bootstrap-Icon** und ein `title`-Attribut als Tooltip
- Suchfeld nimmt mit `flex-grow-1` den verbleibenden Platz ein
- `row g-3` Raster **niemals** für die Toolbar verwenden

**Back-Button-Regel:**
- Zurück-Button im Page-Header: **dezenter Icon-only Button** (`btn btn-link text-secondary`)
- Verwende `(click)="goBack()"` und `title="{{ 'COMMON.BACK' | translate }}"` als Tooltip
- Symbol: `bi bi-arrow-left-circle-fill fs-5`


### Bearbeitungsansicht (Edit-Formular)

→ Vollständiges Template und Schritte: `.github/prompts/create-edit-component.prompt.md`

**Regeln:**
- Dateiname: `entity-name-edit.component.html` in `src/app/components/entities/[subfolder]/`
- Struktur: Bootstrap-Card + Form-Sections + Save/Cancel-Buttons
- Fremdschlüssel → Dropdown mit Service-Daten
- Enums → Dropdown mit `Object.values(EnumType).filter(v => typeof v === 'number')`
- `override ngOnInit()` für Dropdown-Daten (IMMER `override` keyword!)
- CSS-Datei leer lassen

### Master-Details

→ Vollständiges Template und Schritte: `.github/prompts/create-master-details.prompt.md`

**Regeln:**
- Neue standalone Komponente in `src/app/pages/entities/[subfolder]/`
- Dateiname: `masterEntity-detailsEntity.component.html` (z.B. `department-employees.component.html`)
- Master: readonly Bootstrap-Card, Details: bearbeitbare Liste darunter

### Dashboard Integration

- Alle Entity-Lists ins Dashboard eintragen
- Icons aus Bootstrap Icons verwenden
- Responsive Navigation berücksichtigen

→ Details: `.github/prompts/add-dashboard-cards.prompt.md`

---

## TypeScript Component Struktur

### List Component TypeScript

```typescript
//@CustomCode
import { IdType, IdDefault } from '@app/models/i-key-model';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IQueryParams } from '@app/models/base/i-query-params';
import { IEntityName } from '@app-models/entities/app/i-entity-name';
import { EntityNameBaseListComponent } from '@app/components/entities/app/entity-name-base-list.component';
import { EntityNameEditComponent } from '@app/components/entities/app/entity-name-edit.component';
import { AuthService } from '@app-services/auth.service';  // falls Authentifizierung eingeschaltet ist

@Component({
  standalone: true,
  selector: 'app-entityname-list',
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule],
  templateUrl: './entityname-list.component.html',
  styleUrls: ['./entityname-list.component.scss']
})
export class EntityNameListComponent extends EntityNameBaseListComponent {
  constructor(private authService: AuthService) {
    super();
  }
  override ngOnInit(): void {
    super.ngOnInit();
    this.reloadData();
  }
  override prepareQueryParams(queryParams: IQueryParams): void {
    super.prepareQueryParams(queryParams);
    queryParams.filter = 'name.ToLower().Contains(@0) OR description.ToLower().Contains(@0)';
  }
  protected override getItemKey(item: IEntityName): IdType {
    return item?.id || IdDefault;
  }
  override get pageTitle(): string {
    return 'EntityName';
  }
  override getEditComponent() {
    return EntityNameEditComponent;
  }
}
```

### Edit Component TypeScript

```typescript
//@CustomCode
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { EntityNameBaseEditComponent } from '@app/components/entities/app/entity-name-base-edit.component';

@Component({
  standalone: true,
  selector: 'app-entity-name-edit',
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './entity-name-edit.component.html',
  styleUrl: './entity-name-edit.component.css'
})
export class EntityNameEditComponent extends EntityNameBaseEditComponent {
  override ngOnInit(): void {
    super.ngOnInit();
    // Zusätzliche Initialisierungen, z.B. Dropdown-Daten laden
  }
  override get title(): string {
    return this.editMode ? 'ENTITYNAME_EDIT.EDIT_TITLE' : 'ENTITYNAME_EDIT.CREATE_TITLE';
  }
}
```

---

## Responsive Design Regeln

- **Mobile**: `btn-lg`, `col-12`, Inline-Text mit `d-none d-sm-inline`
- **Tablet**: `col-md-6`, Standard `btn`
- **Desktop**: `col-lg-4`, `btn-sm`
- Button-Sichtbarkeit: `d-none d-sm-inline` (Text) + `d-inline d-sm-none` (Icon)

---

## Internationalisierung (i18n)

### ⚠️ WICHTIG: Translate Pipe Regel

**Im Template IMMER die `translate` Pipe verwenden — NIEMALS `translateService.instant()` für Template-Ausgaben!**

```typescript
// ✅ RICHTIG: Komponente gibt Schlüssel zurück
override get title(): string {
  return this.editMode ? 'ENTITYNAME_EDIT.EDIT_TITLE' : 'ENTITYNAME_EDIT.CREATE_TITLE';
}
```

```html
<!-- ✅ RICHTIG: Template übersetzt -->
<h3>{{ title | translate }}</h3>
```

`translateService.instant()` NUR für `MessageBoxService.confirm()` Parameter.

→ Naming-Konventionen und JSON-Struktur: `.github/prompts/add-i18n-translations.prompt.md`

---

## Services und Models

- Services werden automatisch generiert (kein manuelles Erstellen!)
- Models/Interfaces werden generiert in `src/app/models/entities/[subfolder]/`
- Entity-Interface Dateiname: `i-entity-name.ts`
- Model-Interface Pattern: `export interface IEntityName { id: IdType; ... }`

---

## Routing

→ Details: `.github/prompts/configure-routing.prompt.md`

---

## Entwicklungs-Workflow

| Schritt | Aufgabe | Prompt |
|---------|---------|--------|
| 1 | Vor-Template-Checkliste | `pre-template-checklist.prompt.md` |
| 2 | List-Komponenten erstellen | `create-list-component.prompt.md` |
| 3 | Edit-Formulare erstellen | `create-edit-component.prompt.md` |
| 4 | Master-Details (optional) | `create-master-details.prompt.md` |
| 5 | Routing konfigurieren | `configure-routing.prompt.md` |
| 6 | Dashboard-Cards ergänzen | `add-dashboard-cards.prompt.md` |
| 7 | i18n-Übersetzungen | `add-i18n-translations.prompt.md` |
| 8 | Häufige Fehler beheben | `fix-common-errors.prompt.md` |

---

## Konventionen

### Naming

- Komponenten: `kebab-case` für Dateien, `PascalCase` für Klassen
- Services: `camelCase`
- i18n-Keys: `ENTITY_LIST.KEY` / `ENTITY_EDIT.KEY` (GROSSBUCHSTABEN)

### Code-Marker

| Marker | Bedeutung |
|--------|-----------|
| `<!--@AiCode-->` | Von AI erstellt, nicht generiert |
| `<!--@GeneratedCode-->` | Automatisch generiert, nicht bearbeiten |
| `<!--@CustomCode-->` | Manuell angepasst, wird nicht überschrieben |
| `//@AiCode` | TypeScript: Von AI erstellt |
| `//@CustomCode` | TypeScript: Geschützte Anpassung |

### Troubleshooting

| Problem | Lösung |
|---------|--------|
| `Missing override modifier` | `ngOnInit`, `title` etc. mit `override` versehen |
| Dropdown leer | `override ngOnInit()` mit `super.ngOnInit()` aufrufen |
| `*ngIf/*ngFor` Warnung | `@if/@for` verwenden |
| Translate nicht gefunden | Key in `de.json` + `en.json` ergänzen |

→ Details: `.github/prompts/fix-common-errors.prompt.md`
