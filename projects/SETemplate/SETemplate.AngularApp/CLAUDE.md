# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with the **Angular Frontend** of the SETemplate project.

> For backend instructions, see the root `CLAUDE.md` (used when opening the main project folder in a separate VSCode session).

## Project Overview

SETemplate Angular App is the frontend for the SETemplate system:

- **Framework**: Angular 19 with Bootstrap 5 and standalone components
- **UI**: NgBootstrap with Bootstrap Icons
- **Architecture**: Standalone components with strict separation of generated and manual code
- **Code generation**: Template-driven creation of all CRUD components from the backend
- **i18n**: DE/EN support via `@ngx-translate`

**Language note:** All code must be in English — class names, method names, property names, variable names, comments, and all other identifiers.

### Core Concepts

- **Entity-First**: Entities are defined in the backend (`SETemplate.Logic/Entities/`), the generator creates Angular components from them
- **Code-Markers**: `//@GeneratedCode` vs `//@CustomCode` vs `//@AiCode` control overwrite behavior
- **Standalone Components**: All components are standalone — no modules

### Fundamental Rules

- **NEVER** manually create CRUD components or services — they are generated and will be overwritten
- **ALWAYS** use new Angular control-flow syntax (`@if`, `@for`, `@switch`)
- **ALWAYS** use `override` keyword for overridden methods (`ngOnInit`, `title`, etc.)
- **ALWAYS** update both `de.json` and `en.json` when adding i18n keys
- **ALWAYS** use global `styles.css` classes — keep component CSS files empty or minimal

## Essential Commands

```bash
# Code generation from backend (generates components, services, models)
# Run from the root project directory:
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x

# Angular dev server
npm start

# Run tests
npm test
```

## Code Marker System (Critical)

| Marker | Meaning | Editable | Overwritten by generator |
|--------|---------|----------|--------------------------|
| `//@AiCode` | AI-created TypeScript code | Yes | No |
| `<!--@AiCode-->` | AI-created HTML code | Yes | No |
| `//@GeneratedCode` | Auto-generated TypeScript | No | Yes |
| `<!--@GeneratedCode-->` | Auto-generated HTML | No | Yes |
| `//@CustomCode` | Protected TypeScript customization | Yes | No |
| `<!--@CustomCode-->` | Protected HTML customization | Yes | No |

Files with `//@GeneratedCode` or `<!--@GeneratedCode-->` are overwritten on each generation. To customize generated code, change the marker to `//@CustomCode` or `<!--@CustomCode-->`.

## Project Structure

```
src/app/
├── components/          # Generated Edit components
│   └── entities/        # Entity-specific Edit components (base + custom)
├── pages/               # Generated List components
│   └── entities/        # Entity-specific List components
├── services/            # Generated API services
├── models/              # TypeScript interfaces for entities
├── shared/              # Shared components and services
└── assets/
    └── i18n/            # Translation files (de.json, en.json)
```

### Component File Locations

- **Base Edit components**: `src/app/components/entities/[subfolder]/entity-name-base-edit.component.ts`
- **Custom Edit components**: `src/app/components/entities/[subfolder]/entity-name-edit.component.ts`
- **Base List components**: `src/app/components/entities/[subfolder]/entity-name-base-list.component.ts`
- **List page components**: `src/app/pages/entities/[subfolder]/entity-name-list.component.ts`

If entities are in subfolders, the corresponding components are created in matching subfolders.

## Angular Template Syntax (Mandatory)

**Always use new control-flow syntax — never legacy syntax!**

```html
<!-- Correct -->
@if (canAdd) {
    <button class="btn btn-primary" (click)="addCommand()">Add</button>
}
@for (item of dataItems; track item.id ?? $index) {
    <div>{{ item.name }}</div>
}

<!-- Wrong — NEVER use -->
<button *ngIf="canAdd">Add</button>
<div *ngFor="let item of dataItems">{{ item.name }}</div>
```

## CSS Rules (Critical)

**All standard styles come from global `styles.css`**. Component CSS files stay **empty or minimal**.

### Key Global CSS Classes

| Area | Classes |
|------|---------|
| **Lists** | `.list-container`, `.modern-list-item`, `.list-item-header`, `.list-item-details`, `.list-item-actions` |
| **Forms** | `.modern-edit-container`, `.modern-edit-card`, `.modern-edit-header`, `.modern-edit-body` |
| **Form elements** | `.modern-form-group`, `.modern-form-label`, `.modern-form-control`, `.modern-form-select` |
| **Buttons** | `.modern-btn-save`, `.modern-btn-cancel`, `.btn-close-custom` |
| **Sections** | `.form-section`, `.form-section-title` |
| **States** | `.empty-state`, `.empty-state-icon`, `.empty-state-text` |
| **Header** | `.page-header`, `.list-toolbar` |

### When to Use Component-Specific CSS

Only for unique animations, special layout exceptions, or component-specific positioning that don't apply elsewhere. Default: leave CSS file empty.

## Component Rules

- All components are **standalone** — no modules
- Always use **separate HTML and CSS files**
- Always import `CommonModule`, `FormsModule`, `TranslateModule` as needed
- Foreign keys → Dropdown with service data
- Enums → Dropdown with `Object.values(EnumType).filter(v => typeof v === 'number')`
- `override ngOnInit()` for loading dropdown data (always call `super.ngOnInit()`)

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

@Component({
  standalone: true,
  selector: 'app-entityname-list',
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule],
  templateUrl: './entityname-list.component.html',
  styleUrls: ['./entityname-list.component.scss']
})
export class EntityNameListComponent extends EntityNameBaseListComponent {
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
    // Additional initializations, e.g. loading dropdown data
  }
  override get title(): string {
    return this.editMode ? 'ENTITYNAME_EDIT.EDIT_TITLE' : 'ENTITYNAME_EDIT.CREATE_TITLE';
  }
}
```

### List Component HTML Rules

- File: `entity-name-list.component.html` in `src/app/pages/entities/[subfolder]/`
- Structure: Page-Header + Toolbar (Add/Search/Refresh) + List-Items + Empty-State
- Always use `@if/@for` syntax
- CSS file stays empty
- **Toolbar**: All elements (Add button, search input, Refresh button) **always in one row** using `d-flex align-items-center gap-2` — never use `row g-3` grid for the toolbar
- **Buttons**: Every button must have a Bootstrap icon and a `title` attribute as tooltip
- **Search input**: Use `flex-grow-1` wrapper `<div>` so it fills remaining space
- **Back button**: Subtle icon-only button in page header (`btn btn-link text-secondary`), `(click)="goBack()"`, icon `bi bi-arrow-left-circle-fill fs-5`

### Edit Component HTML Rules

- File: `entity-name-edit.component.html` in `src/app/components/entities/[subfolder]/`
- Structure: Bootstrap-Card + Form-Sections + Save/Cancel-Buttons
- Foreign keys → Dropdown with service data
- Enums → Dropdown

### Master-Details Rules

- New standalone component in `src/app/pages/entities/[subfolder]/`
- Filename: `masterEntity-detailsEntity.component.html` (e.g. `department-employees.component.html`)
- Master: readonly Bootstrap-Card, Details: editable list below

## i18n Rules (Critical)

**Always use the `translate` pipe in templates — NEVER `translateService.instant()` for template output!**

```typescript
// Correct: component returns key
override get title(): string {
  return this.editMode ? 'ENTITYNAME_EDIT.EDIT_TITLE' : 'ENTITYNAME_EDIT.CREATE_TITLE';
}
```

```html
<!-- Correct: template translates -->
<h3>{{ title | translate }}</h3>
```

`translateService.instant()` ONLY for `MessageBoxService.confirm()` parameters.

### i18n Key Format

- Keys: `ENTITYNAME_LIST.TITLE` / `ENTITYNAME_EDIT.KEY` (UPPERCASE)
- Always update **both** `de.json` and `en.json`
- Naming conventions and JSON structure: see `.github/prompts/add-i18n-translations.prompt.md`

## Services and Models

- Services are auto-generated — do not create manually!
- Models/Interfaces are generated in `src/app/models/entities/[subfolder]/`
- Entity interface filename: `i-entity-name.ts`
- Model interface pattern: `export interface IEntityName { id: IdType; ... }`

## Responsive Design

- **Mobile**: `btn-lg`, `col-12`, inline text with `d-none d-sm-inline`
- **Tablet**: `col-md-6`, standard `btn`
- **Desktop**: `col-lg-4`, `btn-sm`
- Button visibility: `d-none d-sm-inline` (text) + `d-inline d-sm-none` (icon)

## Naming Conventions

- Components: `kebab-case` for files, `PascalCase` for classes
- Services: `camelCase`
- Selectors: `app-entity-name-list`, `app-entity-name-edit`
- i18n keys: `ENTITY_LIST.KEY` / `ENTITY_EDIT.KEY` (UPPERCASE)

## Dashboard Integration

- Register all entity lists in the dashboard
- Use Bootstrap Icons
- Consider responsive navigation
- Details: `.github/prompts/add-dashboard-cards.prompt.md`

## Angular Development Workflow

| Step | Task | Prompt |
|------|------|--------|
| 1 | Pre-template checklist | `pre-template-checklist.prompt.md` |
| 2 | Create List components | `create-list-component.prompt.md` |
| 3 | Create Edit forms | `create-edit-component.prompt.md` |
| 4 | Master-Details (optional) | `create-master-details.prompt.md` |
| 5 | Configure routing | `configure-routing.prompt.md` |
| 6 | Add dashboard cards | `add-dashboard-cards.prompt.md` |
| 7 | Add i18n translations | `add-i18n-translations.prompt.md` |
| 8 | Fix common errors | `fix-common-errors.prompt.md` |

## Troubleshooting

| Problem | Solution |
|---------|---------|
| `Missing override modifier` | Add `override` to `ngOnInit`, `title`, etc. |
| Dropdown empty | `override ngOnInit()` with `super.ngOnInit()` call |
| `*ngIf/*ngFor` warning | Use `@if/@for` instead |
| Translate not found | Add key to both `de.json` and `en.json` |
| Component not rendering | Check standalone imports and routing configuration |
| Service not available | Check if generated — run code generation if missing |
