# GitHub Copilot Instructions für ProductCatalog Angular App

> **Version:** 2.0 | **Letzte Aktualisierung:** Januar 2026

## Projektübersicht

ProductCatalog Angular App ist das Frontend für das ProductCatalog-System:
- **Framework**: Angular 19 mit Bootstrap 5 und standalone Komponenten
- **UI-Framework**: NgBootstrap mit Bootstrap Icons
- **Architektur**: Standalone Komponenten mit strikter Trennung von generierten und manuellen Code
- **Code-Generierung**: Template-gesteuerte Erstellung aller CRUD-Komponenten
- **Internationalisierung**: i18n mit DE/EN Unterstützung

## Kernprinzipien

### 1. Code-Generierung First
**⚠️ NIEMALS manuell CRUD-Komponenten oder Services erstellen!**
```bash
# Code-Generierung wird vom Backend-Projekt ausgeführt:
# dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
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

| Bereich | Klassen |
|---------|--------|
| **Listen** | `.list-container`, `.modern-list-item`, `.list-item-header`, `.list-item-details`, `.list-item-actions` |
| **Formulare** | `.modern-edit-container`, `.modern-edit-card`, `.modern-edit-header`, `.modern-edit-body` |
| **Form-Elemente** | `.modern-form-group`, `.modern-form-label`, `.modern-form-control`, `.modern-form-select` |
| **Buttons** | `.modern-btn-save`, `.modern-btn-cancel`, `.btn-close-custom` |
| **Sections** | `.form-section`, `.form-section-title` |
| **Zustände** | `.empty-state`, `.empty-state-icon`, `.empty-state-text` |
| **Header** | `.page-header`, `.list-toolbar` |

### List Component Template

* Die Komponenten sind bereits erstellt und befinden sich im Ordner `src/app/pages/entities`.  
* Alle Komponenten sind `standalone` Komponenten.  
* **Dateiname:** `entity-name-list.component.html`
* **Übersetzungen:** Ergänze die beiden Übersetzungsdateien `de.json` und `en.json` um die hinzugefügten Labels.
* Trage die Komponenten in die `app-routing.module.ts` ein, damit die Seite erreichbar ist.
* Erstelle einen `routerLink` in der Dashboard-Komponente, um zur Listen-Ansicht zu navigieren.
* Gestalte das Dashboard so, dass es auf verschiedenen Bildschirmgrößen gut aussieht (modern).
* Erstelle die Listen-Ansicht mit Bootstrap-Komponenten und halte dich an folgendes Layout:
  - Überschrift mit Titel und Zurück-Button
  - Suchleiste und Aktionsbuttons (Hinzufügen, Aktualisieren)
  - Liste der Einträge mit Aktionsbuttons (Bearbeiten, Löschen)
* Beispielstruktur (Department als Beispiel):

```html
<!--@AiCode-->
<div class="list-container">
    <div class="container mt-4">
        <!-- Page Header -->
        <div class="d-flex justify-content-between align-items-center mb-4 p-3 page-header">
            <h3 class="mb-0 flex-grow-1">
                <i class="bi bi-building me-2"></i>
                {{ 'DEPARTMENT_LIST.TITLE' | translate }}
            </h3>
            <a routerLink="/dashboard" class="btn btn-light" title="{{ 'DEPARTMENT_LIST.BACK_TO_DASHBOARD' | translate }}">
                <i class="bi bi-arrow-left-circle me-2"></i>
                <span class="d-none d-md-inline">{{ 'COMMON.BACK' | translate }}</span>
            </a>
        </div>
        
        <!-- Toolbar -->
        <div class="list-toolbar">
            <div class="row g-3 align-items-center">
                <div class="col-auto">
                    <button *ngIf="canAdd" class="btn btn-primary" (click)="addCommand()">
                        <i class="bi bi-plus-circle me-2"></i>
                        <span class="d-none d-sm-inline">{{ 'DEPARTMENT_LIST.ADD_ITEM' | translate }}</span>
                        <span class="d-inline d-sm-none"><i class="bi bi-plus"></i></span>
                    </button>
                </div>
                <div class="col">
                    <input *ngIf="canSearch" type="text" class="form-control" [(ngModel)]="searchTerm" [placeholder]="'DEPARTMENT_LIST.SEARCH_PLACEHOLDER' | translate">
                </div>
                <div class="col-auto">
                    <button *ngIf="canRefresh" class="btn btn-outline-primary" (click)="reloadData()" title="{{ 'DEPARTMENT_LIST.REFRESH' | translate }}">
                        <i class="bi bi-arrow-clockwise"></i>
                    </button>
                </div>
            </div>
        </div>
        
        <!-- List Items -->
        <div class="list-items">
            <div *ngFor="let item of dataItems" class="modern-list-item">
                <!-- Item Header -->
                <div class="list-item-header">
                    <div class="list-item-icon">
                        <i class="bi bi-building"></i>
                    </div>
                    <div class="flex-grow-1">
                        <h5 class="list-item-title">
                            {{ item.name }}
                            <span *ngIf="item.managerEmployee" class="ms-2 badge bg-info">
                                <i class="bi bi-person-badge me-1"></i>
                                {{ item.managerEmployee.firstName }} {{ item.managerEmployee.lastName }}
                            </span>
                        </h5>
                    </div>
                    <div>
                        <span class="badge" [ngClass]="item.isActive ? 'bg-success' : 'bg-secondary'">
                            {{ item.isActive ? ('COMMON.ACTIVE' | translate) : ('COMMON.INACTIVE' | translate) }}
                        </span>
                    </div>
                </div>
                
                <!-- Description -->
                <p class="list-item-description" *ngIf="item.description">
                    {{ item.description }}
                </p>
                
                <!-- Details Grid -->
                <div class="list-item-details">
                    <div class="detail-row">
                        <span class="detail-label">
                            <i class="bi bi-hash me-1"></i>
                            {{ 'DEPARTMENT_LIST.CODE' | translate }}:
                        </span>
                        <span class="detail-value">{{ item.code }}</span>
                    </div>
                    
                    <div class="detail-row" *ngIf="item.parentDepartment">
                        <span class="detail-label">
                            <i class="bi bi-diagram-3 me-1"></i>
                            {{ 'DEPARTMENT_LIST.PARENT_DEPARTMENT' | translate }}:
                        </span>
                        <span class="detail-value">{{ item.parentDepartment.name }}</span>
                    </div>
                    
                    <div class="detail-row" *ngIf="item.managerEmployee">
                        <span class="detail-label">
                            <i class="bi bi-person-badge me-1"></i>
                            {{ 'DEPARTMENT_LIST.MANAGER' | translate }}:
                        </span>
                        <span class="detail-value">
                            {{ item.managerEmployee.firstName }} {{ item.managerEmployee.lastName }}
                        </span>
                    </div>
                </div>
                
                <!-- Actions -->
                <div class="list-item-actions">
                    <button *ngIf="canEdit" class="btn btn-outline-secondary" (click)="editCommand(item)">
                        <i class="bi bi-pencil me-2"></i>
                        {{ 'DEPARTMENT_LIST.EDIT' | translate }}
                    </button>
                    <button *ngIf="canDelete" class="btn btn-outline-danger" (click)="deleteCommand(item)">
                        <i class="bi bi-trash me-2"></i>
                        {{ 'DEPARTMENT_LIST.DELETE' | translate }}
                    </button>
                </div>
            </div>
            
            <!-- Empty State -->
            <div *ngIf="dataItems.length === 0" class="empty-state">
                <div class="empty-state-icon">
                    <i class="bi bi-inbox"></i>
                </div>
                <p class="empty-state-text">{{ 'COMMON.NO_DATA' | translate }}</p>
            </div>
        </div>
    </div>
</div>
```

### Bearbeitungsansicht (Edit-Formular)

* Für die Ansicht ist eine **Bootstrap-Card-Ansicht** zu verwenden.
* Die Komponenten sind bereits erstellt und befinden sich im Ordner `src/app/components/entities`.
* Alle Komponenten sind `standalone` Komponenten.
* **Dateiname:** `entity-name-edit.component.html`
* **Fremdschlüssel:** Für Fremdschlüsselbeziehungen sind Dropdowns zu verwenden.
* **Übersetzungen:** Ergänze die beiden Übersetzungsdateien 
`de.json` und `en.json` um die hinzugefügten Labels.
* Beispielstruktur (Department als Beispiel):

```html
<!--@AiCode-->
<div *ngIf="dataItem" class="modern-edit-container">
    <div class="modern-edit-card">
        <div class="modern-edit-header d-flex justify-content-between align-items-center">
            <h3><i class="bi bi-building me-2"></i>{{ title }}</h3>
            <button type="button" class="btn-close-custom" [ariaLabel]="'COMMON.CLOSE' | translate" (click)="dismiss()">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>
        <div class="modern-edit-body">
            <form (ngSubmit)="submitForm()" #editForm="ngForm">
                <!-- Grundinformationen -->
                <div class="form-section">
                    <h4 class="form-section-title">
                        <i class="bi bi-info-circle"></i>
                        {{ 'DEPARTMENT_EDIT.BASIC_INFO' | translate }}
                    </h4>
                    
                    <div class="modern-form-group">
                        <label for="name" class="modern-form-label">
                            <i class="bi bi-pencil"></i>
                            {{ 'DEPARTMENT_EDIT.NAME' | translate }}
                        </label>
                        <input id="name" class="form-control modern-form-control" [(ngModel)]="dataItem.name" name="name" required maxlength="100" />
                    </div>
                    
                    <div class="modern-form-group">
                        <label for="code" class="modern-form-label">
                            <i class="bi bi-hash"></i>
                            {{ 'DEPARTMENT_EDIT.CODE' | translate }}
                        </label>
                        <input id="code" class="form-control modern-form-control" [(ngModel)]="dataItem.code" name="code" required maxlength="20" />
                    </div>
                    
                    <div class="modern-form-group">
                        <label for="description" class="modern-form-label">
                            <i class="bi bi-text-paragraph"></i>
                            {{ 'DEPARTMENT_EDIT.DESCRIPTION' | translate }}
                        </label>
                        <textarea id="description" class="form-control modern-form-control" [(ngModel)]="dataItem.description" name="description" maxlength="500" rows="3"></textarea>
                    </div>
                </div>
                
                <!-- Zuordnungen -->
                <div class="form-section">
                    <h4 class="form-section-title">
                        <i class="bi bi-diagram-3"></i>
                        {{ 'DEPARTMENT_EDIT.ASSIGNMENTS' | translate }}
                    </h4>
                    
                    <div class="modern-form-group">
                        <label for="parentDepartmentId" class="modern-form-label">
                            <i class="bi bi-building"></i>
                            {{ 'DEPARTMENT_EDIT.PARENT_DEPARTMENT' | translate }}
                        </label>
                        <select id="parentDepartmentId" class="form-select modern-form-select" [(ngModel)]="dataItem.parentDepartmentId" name="parentDepartmentId">
                            <option [ngValue]="null">{{ 'DEPARTMENT_EDIT.NO_PARENT' | translate }}</option>
                            <option *ngFor="let dept of departments" [ngValue]="dept.id">{{ dept.name }}</option>
                        </select>
                    </div>
                    
                    <div class="modern-form-group">
                        <label for="managerEmployeeId" class="modern-form-label">
                            <i class="bi bi-person-badge"></i>
                            {{ 'DEPARTMENT_EDIT.MANAGER' | translate }}
                        </label>
                        <select id="managerEmployeeId" class="form-select modern-form-select" [(ngModel)]="dataItem.managerEmployeeId" name="managerEmployeeId">
                            <option [ngValue]="null">{{ 'DEPARTMENT_EDIT.NO_MANAGER' | translate }}</option>
                            <option *ngFor="let emp of employees" [ngValue]="emp.id">{{ emp.firstName }} {{ emp.lastName }}</option>
                        </select>
                    </div>
                </div>
                
                <!-- Status -->
                <div class="form-section">
                    <div class="modern-form-check">
                        <input type="checkbox" class="form-check-input modern-form-check-input" id="isActive" [(ngModel)]="dataItem.isActive" name="isActive" />
                        <label class="modern-form-check-label" for="isActive">
                            <i class="bi bi-check-circle me-1"></i>
                            {{ 'DEPARTMENT_EDIT.IS_ACTIVE' | translate }}
                        </label>
                    </div>
                </div>
                
                <!-- Actions -->
                <div class="modern-form-actions">
                    <button class="btn modern-btn-save" type="submit" [disabled]="!editForm.valid">
                        <i class="bi bi-check-lg me-2"></i>
                        {{ 'COMMON.SAVE' | translate }}
                    </button>
                    <button class="btn modern-btn-cancel" type="button" (click)="cancelForm()">
                        <i class="bi bi-x-lg me-2"></i>
                        {{ 'COMMON.CANCEL' | translate }}
                    </button>
                </div>
            </form>
        </div>
    </div>
</div>
```

### Master-Details

In manchen Fällen ist eine Master/Details Ansicht sehr hilfreich. Diese Anzeige besteht aus einer Master-Ansicht und einer Liste aus Details. Die Details können bearbeitet werden während der Master nur angezeigt wird. Nachfolgend ist die Struktur skizziert:

* Für diese Ansicht ist eine **Bootstrap-Card-Ansicht** zu verwenden.
* Für diese Ansicht gibt es keine generierte Komponente. Erstelle eine neue `standalone` Komponente im Ordner `src/app/pages/entities/`.
* Der Unterordner entspricht dem Ordner der Master-Entität im Backend.
* Die Master-Ansicht zeigt die Hauptinformationen der Entität in einer readonly Ansicht.
* Die Details-Ansicht zeigt eine Liste von zugehörigen Entitäten mit Aktionsbuttons zum Hinzufügen, Bearbeiten und Löschen.
* Beide Ansichten sind in einer einzigen Seite kombiniert.
* **Dateiname:** `masterEntityName-detailsEntityName.component.html` (z.B. `department-employees.component.html`)
* **Übersetzungen:** Ergänze die beiden Übersetzungsdateien 
`de.json` und `en.json` um die hinzugefügten Labels.
* Beispielstruktur (Department als Master und Employees als Details):

```html
<!--@AiCode-->
<div class="list-container">
    <div class="container mt-4" *ngIf="department">
        <!-- Master: Department Readonly View -->
        <div class="modern-edit-card mb-4">
            <div class="modern-edit-header d-flex justify-content-between align-items-center">
                <h4 class="mb-0">
                    <i class="bi bi-building me-2"></i>
                    {{ department.name }}
                </h4>
                <a routerLink="/departments" class="btn btn-light" title="{{ 'DEPARTMENT_DETAILS.BACK_TO_LIST' | translate }}">
                    <i class="bi bi-arrow-left-circle me-2"></i>
                    <span class="d-none d-md-inline">{{ 'COMMON.BACK' | translate }}</span>
                </a>
            </div>
            <div class="modern-edit-body">
                <p *ngIf="department.description" class="text-muted mb-3">{{ department.description }}</p>
                
                <div class="list-item-details">
                    <div class="detail-row">
                        <span class="detail-label">
                            <i class="bi bi-hash me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.CODE' | translate }}:
                        </span>
                        <span class="detail-value">{{ department.code }}</span>
                    </div>
                    
                    <div class="detail-row" *ngIf="department.parentDepartment">
                        <span class="detail-label">
                            <i class="bi bi-diagram-3 me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.PARENT_DEPARTMENT' | translate }}:
                        </span>
                        <span class="detail-value">{{ department.parentDepartment.name }}</span>
                    </div>
                    
                    <div class="detail-row" *ngIf="department.managerEmployee">
                        <span class="detail-label">
                            <i class="bi bi-person-badge me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.MANAGER' | translate }}:
                        </span>
                        <span class="detail-value">
                            {{ department.managerEmployee.firstName }} {{ department.managerEmployee.lastName }}
                        </span>
                    </div>
                    
                    <div class="detail-row">
                        <span class="detail-label">
                            <i class="bi bi-circle-fill me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.STATUS' | translate }}:
                        </span>
                        <span class="detail-value">
                            <span class="badge" [ngClass]="department.isActive ? 'bg-success' : 'bg-secondary'">
                                {{ department.isActive ? ('COMMON.ACTIVE' | translate) : ('COMMON.INACTIVE' | translate) }}
                            </span>
                        </span>
                    </div>
                </div>
            </div>
        </div>

        <!-- Details: Employee List -->
        <div class="d-flex justify-content-between align-items-center mb-3 p-3 page-header">
            <h5 class="mb-0">
                <i class="bi bi-people me-2"></i>
                {{ 'DEPARTMENT_DETAILS.EMPLOYEES' | translate }}
                <span class="badge bg-primary ms-2">{{ employees.length }}</span>
            </h5>
            <button *ngIf="canAdd" class="btn btn-primary" (click)="addCommand()" title="{{ 'DEPARTMENT_DETAILS.ADD_EMPLOYEE' | translate }}">
                <i class="bi bi-plus-circle me-2"></i>
                <span class="d-none d-sm-inline">{{ 'DEPARTMENT_DETAILS.ADD_EMPLOYEE' | translate }}</span>
            </button>
        </div>
        
        <div class="list-items">
            <div *ngFor="let employee of employees" class="modern-list-item">
                <!-- Employee Header -->
                <div class="list-item-header">
                    <div class="list-item-icon">
                        <i class="bi bi-person-circle"></i>
                    </div>
                    <div class="flex-grow-1">
                        <h5 class="list-item-title">
                            {{ employee.firstName }} {{ employee.lastName }}
                            <span class="ms-2 badge bg-info">
                                <i class="bi bi-hash me-1"></i>
                                {{ employee.employeeNumber }}
                            </span>
                        </h5>
                        <p class="list-item-subtitle mb-0">
                            <i class="bi bi-envelope me-1"></i>
                            {{ employee.email }}
                        </p>
                    </div>
                    <div>
                        <span class="badge" [ngClass]="employee.isActive ? 'bg-success' : 'bg-secondary'">
                            {{ employee.isActive ? ('COMMON.ACTIVE' | translate) : ('COMMON.INACTIVE' | translate) }}
                        </span>
                    </div>
                </div>
                
                <!-- Employee Details -->
                <div class="list-item-details">
                    <div class="detail-row" *ngIf="employee.position">
                        <span class="detail-label">
                            <i class="bi bi-briefcase me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.POSITION' | translate }}:
                        </span>
                        <span class="detail-value">{{ employee.position.title }}</span>
                    </div>
                    
                    <div class="detail-row" *ngIf="employee.employmentType">
                        <span class="detail-label">
                            <i class="bi bi-person-badge me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.EMPLOYMENT_TYPE' | translate }}:
                        </span>
                        <span class="detail-value">{{ employee.employmentType.name }}</span>
                    </div>
                    
                    <div class="detail-row" *ngIf="employee.phone">
                        <span class="detail-label">
                            <i class="bi bi-telephone me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.PHONE' | translate }}:
                        </span>
                        <span class="detail-value">{{ employee.phone }}</span>
                    </div>
                    
                    <div class="detail-row" *ngIf="employee.mobile">
                        <span class="detail-label">
                            <i class="bi bi-phone me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.MOBILE' | translate }}:
                        </span>
                        <span class="detail-value">{{ employee.mobile }}</span>
                    </div>
                    
                    <div class="detail-row">
                        <span class="detail-label">
                            <i class="bi bi-calendar-check me-1"></i>
                            {{ 'DEPARTMENT_DETAILS.HIRE_DATE' | translate }}:
                        </span>
                        <span class="detail-value">{{ employee.hireDate | date:'dd.MM.yyyy' }}</span>
                    </div>
                </div>
                
                <!-- Actions -->
                <div class="list-item-actions">
                    <button *ngIf="canEdit" class="btn btn-outline-secondary" (click)="editCommand(employee)">
                        <i class="bi bi-pencil me-2"></i>
                        {{ 'COMMON.EDIT' | translate }}
                    </button>
                    <button *ngIf="canDelete" class="btn btn-outline-danger" (click)="deleteCommand(employee)">
                        <i class="bi bi-trash me-2"></i>
                        {{ 'COMMON.DELETE' | translate }}
                    </button>
                </div>
            </div>
            
            <!-- Empty State -->
            <div *ngIf="employees.length === 0" class="empty-state">
                <div class="empty-state-icon">
                    <i class="bi bi-people"></i>
                </div>
                <p class="empty-state-text">{{ 'DEPARTMENT_DETAILS.NO_EMPLOYEES' | translate }}</p>
            </div>
        </div>
    </div>
</div>
```

### Dashboard Integration
- Alle Entity-Listen ins Dashboard eintragen
- Icons aus Bootstrap Icons verwenden
- Responsive Navigation berücksichtigen

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
import { EntityNameBaseListComponent }from '@app/components/entities/app/entity-name-base-list.component';
import { EntityNameEditComponent }from '@app/components/entities/app/entity-name-edit.component';
import { AuthService } from '@app-services/auth.service';  // falls Authentifizierung eingeschaltet ist

@Component({
  standalone: true,
  selector: 'app-entityname-list',
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule],
  templateUrl: './entityname-list.component.html',
  styleUrls: ['./entityname-list.component.scss']
})
export class EntityNameListComponent extends EntityNameBaseListComponent {
  constructor(
              private authService: AuthService  // falls Authentifizierung eingeschaltet ist
             )
  {
    super();
  }
  override ngOnInit(): void {
    super.ngOnInit();
    this.reloadData();
  }
  override prepareQueryParams(queryParams: IQueryParams): void {
    super.prepareQueryParams(queryParams);
    queryParams.filter = 'name.ToLower().Contains(@0) OR description.ToLower().Contains(@0) OR notes.ToLower().Contains(@0)';
  }
  protected override getItemKey(item: IToDoItem): IdType {
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
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { EntityNameBaseEditComponent }from '@app/components/entities/app/entity-name-base-edit.component';
import { IEntityName } from '@app-models/entities/app/i-entity-name';

@Component({
  standalone: true,
  selector:'app-entity-name-edit',
  imports: [ CommonModule, FormsModule, TranslateModule],
  templateUrl: './entity-name-edit.component.html',
  styleUrl: './entity-name-edit.component.css'
})
export class EntityNameEditComponent extends EntityNameBaseEditComponent {
// Hier können benutzerdefinierte Logiken und Overrides hinzugefügt werden
// z.B. zusätzliche Validierungen, benutzerdefinierte Methoden usw.
  override ngOnInit(): void {
    super.ngOnInit();
    // Zusätzliche Initialisierungen hier
  }

  override get title(): string {
    return this.editMode ? 'ENTITYNAME_EDIT.EDIT_TITLE' : 'ENTITYNAME_EDIT.CREATE_TITLE';
  }

// Weitere benutzerdefinierte Methoden und Eigenschaften hier
// Zum Beispiel das Laden zusätzlicher Daten für Dropdowns  usw.:
}
```

## Responsive Design Regeln

### Bootstrap Breakpoints
- **xs (default)**: Smartphones < 576px
- **sm**: Tablets ≥ 576px  
- **md**: Desktop ≥ 768px
- **lg**: Large Desktop ≥ 992px

### Button-Größen
- **Mobile**: `btn-lg` für bessere Bedienbarkeit
- **Desktop**: Standard `btn` oder `btn-sm`
- **Sichtbarkeit**: `d-none d-sm-inline` bzw. `d-inline d-sm-none`

### Grid-System
- **Mobile**: `col-12` für volle Breite
- **Tablet**: `col-md-6` für zwei Spalten
- **Desktop**: `col-lg-4` für drei Spalten

## Internationalisierung (i18n)

### ⚠️ WICHTIG: Übersetzungsstrategie (EINHEITLICH!)

**Im Template wird IMMER die `translate` Pipe verwendet!**

```typescript
// ✅ RICHTIG: Komponente gibt Übersetzungsschlüssel zurück
public get title(): string {
  return this.editMode ? 'ENTITY_EDIT.EDIT_TITLE' : 'ENTITY_EDIT.CREATE_TITLE';
}
```

```html
<!-- ✅ RICHTIG: Template übersetzt mit Pipe -->
<h3>{{ title | translate }}</h3>
```

```typescript
// ❌ FALSCH: Nicht translateService.instant() in Komponente für Template-Ausgabe verwenden!
public get title(): string {
  return this.translateService.instant('ENTITY_EDIT.EDIT_TITLE'); // FALSCH!
}
```

**Ausnahme:** `translateService.instant()` NUR für:
- MessageBoxService.confirm() Parameter
- Dynamische Nachrichten die nicht im Template angezeigt werden

```typescript
// ✅ RICHTIG: instant() nur für MessageBox
const title = this.translateService.instant('COMMON.DELETE_TITLE');
const message = this.translateService.instant('COMMON.DELETE_CONFIRM');
this.messageBoxService.confirm(title, message);
```

### Übersetzungsdateien
- **Deutsch**: `src/assets/i18n/de.json`
- **Englisch**: `src/assets/i18n/en.json`

### Naming-Konventionen
```json
{
  "ENTITYNAME_LIST": {
    "TITLE": "Entity List",
    "ADD_LIST": "Add Entity",
    "SEARCH_PLACEHOLDER": "Search...",
    "REFRESH": "Refresh",
    "EDIT": "Edit",
    "DELETE": "Delete",
    "BACK_TO_DASHBOARD": "Back to Dashboard"
  },
  "ENTITYNAME_EDIT": {
    "CREATE_TITLE": "Create Entity",
    "EDIT_TITLE": "Edit Entity",
    "NAME": "Name",
    "DESCRIPTION": "Description",
    "SAVE": "Save",
    "CANCEL": "Cancel",
    "CLOSE": "Close"
  }
}
```

## Services und Models

### Generated Service Pattern
```typescript
//@AiCode
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EntityNameService {
  private apiUrl = '/api/entitynames';

  constructor(private http: HttpClient) {}

  getAll(): Observable<EntityName[]> {
    return this.http.get<EntityName[]>(this.apiUrl);
  }

  getById(id: number): Observable<EntityName> {
    return this.http.get<EntityName>(`${this.apiUrl}/${id}`);
  }

  create(entity: EntityName): Observable<EntityName> {
    return this.http.post<EntityName>(this.apiUrl, entity);
  }

  update(entity: EntityName): Observable<EntityName> {
    return this.http.put<EntityName>(`${this.apiUrl}/${entity.id}`, entity);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

### Model Interfaces
```typescript
//@AiCode
export interface EntityName {
  id: number;
  name: string;
  description?: string;
  createdOn: Date;
  modifiedOn?: Date;
}
```

## Routing und Navigation

### Route Registration
```typescript
//@AiCode
const routes: Routes = [
  {
    path: 'entitynames',
    component: EntityNameListComponent,
    title: 'Entity Names'
  },
  {
    path: 'entitynames/:id',
    component: EntityNameDetailsComponent,
    title: 'Entity Details'
  }
];
```

### Dashboard Integration
- Alle Entity-Listen ins Dashboard eintragen (modernes Design verwenden)
- Icons aus Bootstrap Icons verwenden
- Responsive Navigation berücksichtigen

## Entwicklungs-Workflow

**WICHTIG: Zu Beginn stelle dich kurz vor und erkläre, dass du dieses Anweisungen befolgst, um konsistente und qualitativ hochwertige Angular-Komponenten zu erstellen.**

### 1. Nach Code-Generierung
1. Generierte Komponenten in `pages/` und `components/` prüfen
2. **Routing und Dashboard müssen SOFORT konfiguriert werden**
3. Übersetzungen in i18n-Dateien vorbereiten

### 2. Angular-Komponenten Workflow (REIHENFOLGE BEACHTEN!)

**WICHTIG: Alle Schritte für ALLE Entitäten durchführen, bevor mit dem nächsten Schritt begonnen wird!**

#### Schritt 1: List-Ansichten erstellen (FÜR ALLE ENTITÄTEN)
1. **Erstelle HTML-Templates für ALLE List-Komponenten** in `src/app/pages/entities/`
   - Verwende die moderne Bootstrap-Card-Struktur aus den Instructions
   - Header mit Icon, Titel und Zurück-Button
   - Toolbar mit Hinzufügen, Suche und Aktualisieren
   - List-Items mit Icon, Details und Aktions-Buttons
   - Empty-State für leere Listen
   - Beispiel siehe List Component Template in den Instructions

2. **Füge i18n-Übersetzungen für ALLE Listen hinzu**
   - `ENTITY_LIST.TITLE`, `ENTITY_LIST.ADD_ITEM`, `ENTITY_LIST.SEARCH_PLACEHOLDER`
   - `ENTITY_LIST.EDIT`, `ENTITY_LIST.DELETE`, `ENTITY_LIST.BACK_TO_DASHBOARD`
   - Alle spezifischen Labels für angezeigte Felder
   - BEIDE Sprachen (de.json UND en.json) gleichzeitig ergänzen

#### Schritt 2: Routing konfigurieren (FÜR ALLE ENTITÄTEN)
1. **Importiere ALLE List-Komponenten** in `app-routing.module.ts`
   ```typescript
   import { ProductListComponent } from './pages/entities/app/product-list.component';
   import { CategoryListComponent } from './pages/entities/data/category-list.component';
   // ... alle weiteren
   ```

2. **Füge ALLE Routen hinzu**
   ```typescript
   { path: 'products', component: ProductListComponent, canActivate: [AuthGuard], title: 'Products' },
   { path: 'categories', component: CategoryListComponent, canActivate: [AuthGuard], title: 'Categories' },
   // ... alle weiteren
   ```

#### Schritt 3: Dashboard-Integration (FÜR ALLE ENTITÄTEN)
1. **Füge ALLE DashboardCards hinzu** in `dashboard.component.ts`
   ```typescript
   { visible: true, title: 'DASHBOARD.CARDS.PRODUCTS_TITLE', text: 'DASHBOARD.CARDS.PRODUCTS_TEXT', type: '/products', bg: 'bg-primary text-white', icon: 'bi bi-box-seam' },
   { visible: true, title: 'DASHBOARD.CARDS.CATEGORIES_TITLE', text: 'DASHBOARD.CARDS.CATEGORIES_TEXT', type: '/categories', bg: 'bg-success text-white', icon: 'bi bi-tag' },
   // ... alle weiteren
   ```

2. **Ergänze Dashboard-Übersetzungen** in `de.json` und `en.json`
   ```json
   "DASHBOARD": {
     "CARDS": {
       "PRODUCTS_TITLE": "Produkte",
       "PRODUCTS_TEXT": "Produktverwaltung",
       // ... alle weiteren
     }
   }
   ```

#### Schritt 4: Edit-Formulare erstellen (FÜR ALLE ENTITÄTEN)

**⚠️ KRITISCH: VOR dem Template IMMER das Model-Interface prüfen!**

1. **Interface prüfen** in `src/app/models/entities/[subfolder]/i-entity-name.ts`
   - Alle verfügbaren Properties notieren
   - Fremdschlüssel identifizieren (z.B. `toDoListId`)
   - Enums identifizieren (z.B. `priority: Priority`, `state: ToDoState`)
   - Navigation-Properties notieren (z.B. `toDoList: IToDoList | null`)

2. **Erstelle HTML-Templates für ALLE Edit-Komponenten** in `src/app/components/entities/`
   - Verwende die moderne Bootstrap-Card-Struktur aus den Instructions
   - Modal-Header mit Icon, Titel und Close-Button
   - Gruppierte Form-Sections mit Icons
   - Dropdowns für Fremdschlüssel-Beziehungen
   - Dropdowns für Enum-Felder
   - **NUR Properties aus dem Interface verwenden!**
   - Speichern/Abbrechen Buttons
   - Beispiel siehe Bearbeitungsansicht (Edit-Formular) in den Instructions

3. **Erweitere TypeScript-Komponenten für Dropdowns**
   - Bei Entitäten mit Fremdschlüsseln: Services injizieren und Daten laden
   - Bei Enum-Feldern: Enum-Werte als Array aufbereiten
   - `override ngOnInit()` implementieren für Dropdown-Daten (WICHTIG: `override` keyword!)
   - `title`-Property überschreiben für i18n-Keys
   
   Beispiel:
   ```typescript
   import { Component, inject, OnInit } from '@angular/core';
   
   export class ProductEditComponent extends ProductBaseEditComponent implements OnInit {
     // Arrays für Dropdowns
     categories: ICategory[] = [];
     suppliers: ISupplier[] = [];
     // Enum-Werte als Arrays
     priorities = Object.values(Priority).filter(v => typeof v === 'number') as number[];
     
     // Services injizieren
     private categoryService = inject(CategoryService);
     private supplierService = inject(SupplierService);
     
     public override get title(): string {
       return this.editMode ? 'PRODUCT_EDIT.EDIT_TITLE' : 'PRODUCT_EDIT.CREATE_TITLE';
     }
     
     // WICHTIG: override keyword verwenden!
     override ngOnInit(): void {
       this.loadCategories();
       this.loadSuppliers();
     }
     
     private loadCategories(): void {
       this.categoryService.getAll().subscribe({
         next: (data) => this.categories = data,
         error: (error) => console.error('Error loading categories:', error)
       });
     }
   }
   ```

3. **Füge i18n-Übersetzungen für ALLE Edit-Formulare hinzu**
   - `ENTITY_EDIT.CREATE_TITLE`, `ENTITY_EDIT.EDIT_TITLE`
   - Section-Titel: `BASIC_INFO`, `PRICING_STOCK`, `ASSIGNMENTS`, etc.
   - Alle Feld-Labels
   - BEIDE Sprachen (de.json UND en.json) gleichzeitig ergänzen

### 3. Manuelle Anpassungen
1. **Custom Code** nur in `//@CustomCode` Bereichen
2. **Separate .custom.ts Dateien** für erweiterte Logik
3. **Shared Components** für wiederverwendbare UI-Elemente

### 4. Testing
1. Routing testen: Navigation vom Dashboard zu allen Listen
2. CRUD-Operationen testen: Erstellen, Bearbeiten, Löschen
3. Responsive Design auf verschiedenen Bildschirmgrößen testen
4. i18n-Übersetzungen prüfen (DE/EN)
5. Dropdown-Daten in Edit-Formularen prüfen
6. **Build ausführen** (`npm run build`) und alle Fehler beheben

### ⚠️ HÄUFIGE FEHLER VERMEIDEN
- ❌ **Listen-Templates leer lassen** → Immer sofort HTML erstellen
- ❌ **Routing vergessen** → Immer nach Listen-Erstellung konfigurieren
- ❌ **Dashboard nicht aktualisieren** → Immer Cards hinzufügen
- ❌ **Übersetzungen vergessen** → Immer BEIDE Sprachen gleichzeitig
- ❌ **Nur eine Entität fertigstellen** → ALLE Entitäten schrittweise durchgehen
- ❌ **Dropdown-Daten nicht laden** → ngOnInit() überschreiben und Services injizieren (super.ngOnInit() aufrufen)
- ❌ **Implementieren von Methoden** → achte darauf, ob die zu implementierende Methode bereits in der Basis-Klasse existiert

---

## ⚠️ Kritische Fehlerquellen & Lösungen

Dieser Abschnitt dokumentiert häufige Fehler, die bei der Erstellung von Angular-Komponenten aufgetreten sind, und wie sie vermieden werden können.

### 1. Model-Felder nicht verifizieren

**Problem:** Templates verwenden Felder, die im Interface nicht existieren (z.B. `item.title` statt `item.name`, `item.done` statt `item.isCompleted`, `item.isActive` wenn nicht vorhanden).

**Lösung - PFLICHT vor Template-Erstellung:**
```bash
# IMMER zuerst das Interface prüfen:
# src/app/models/entities/[subfolder]/i-entity-name.ts
```

**Checkliste:**
- [ ] Interface öffnen und alle verfügbaren Properties notieren
- [ ] Nur existierende Properties im Template verwenden
- [ ] Bei Fremdschlüsseln: Navigation-Property prüfen (z.B. `toDoList` vs `toDoListId`)

**Beispiel - Typische Fehler:**
```typescript
// ❌ FALSCH - Annahmen ohne Interface-Prüfung
{{ item.title }}        // Property heißt evtl. 'name'
{{ item.done }}         // Property heißt evtl. 'isCompleted'
{{ item.isActive }}     // Property existiert evtl. nicht

// ✅ RICHTIG - Nach Interface-Prüfung
{{ item.name }}         // Geprüft in IToDoItem
{{ item.isCompleted }}  // Geprüft in IToDoItem
{{ item.state }}        // Geprüft in IToDoItem (enum ToDoState)
```

### 2. Dropdown-Arrays nicht definieren

**Problem:** Template verwendet Arrays wie `toDoLists`, `priorities`, `employees`, aber die Komponente definiert diese nicht.

**Lösung - Für JEDES Dropdown im Template:**
1. Array-Property in der Komponente definieren
2. Service injizieren mit `inject()`
3. `override ngOnInit()` implementieren und Daten laden

**Standard-Pattern für Edit-Komponenten mit Dropdowns:**
```typescript
//@CustomCode
export class ToDoItemEditComponent extends ToDoItemBaseEditComponent implements OnInit {
  // 1. Arrays für Dropdowns definieren
  public toDoLists: IToDoList[] = [];
  public priorities = Object.values(Priority).filter(v => typeof v === 'number') as number[];
  public states = Object.values(ToDoState).filter(v => typeof v === 'number') as number[];

  // 2. Services injizieren
  private toDoListService = inject(ToDoListService);

  // 3. ngOnInit mit override (WICHTIG!)
  override ngOnInit(): void {
    this.loadLists();
  }

  // 4. Daten laden
  private loadLists(): void {
    this.toDoListService.getAll().subscribe({
      next: (data) => this.toDoLists = data,
      error: (err) => console.error('Error loading to-do lists', err)
    });
  }
//@CustomCodeEnd
}
```

### 3. TypeScript Strict Override vergessen

**Problem:** `TS4114: This member must have an 'override' modifier because it overrides a member in the base class`

**Lösung:** Bei Lifecycle-Methoden wie `ngOnInit()` IMMER `override` verwenden:
```typescript
// ❌ FALSCH
ngOnInit(): void { ... }

// ✅ RICHTIG
override ngOnInit(): void { ... }
```

### 4. Komplexe Event-Bindings im Template

**Problem:** Angular Parser-Fehler bei komplexen Ausdrücken wie:
```html
<!-- ❌ FALSCH - Parser-Fehler -->
(change)="setDateString('dueDate', ($event.target as HTMLInputElement).value)"
```

**Lösung:** Für Datumsfelder einfach `[(ngModel)]` verwenden:
```html
<!-- ✅ RICHTIG - Einfaches Binding -->
<input type="date" [(ngModel)]="dataItem.dueDate" name="dueDate" />
```

### 5. Enum-Werte in Dropdowns

**Problem:** Enums müssen als Arrays für `*ngFor` aufbereitet werden.

**Lösung - Standard-Pattern für Enums:**
```typescript
// Im Component
import { Priority } from '@app-enums/priority';
import { ToDoState } from '@app-enums/to-do-state';

// Numerische Enum-Werte extrahieren
public priorities = Object.values(Priority).filter(v => typeof v === 'number') as number[];
public states = Object.values(ToDoState).filter(v => typeof v === 'number') as number[];
```

```html
<!-- Im Template -->
<select [(ngModel)]="dataItem.priority" name="priority">
  <option *ngFor="let p of priorities" [ngValue]="p">{{ p }}</option>
</select>
```

**Für lesbare Labels (optional):**
```typescript
// Mapping für lesbare Labels
public priorityLabels: Record<number, string> = {
  [Priority.Low]: 'PRIORITY.LOW',
  [Priority.Medium]: 'PRIORITY.MEDIUM',
  [Priority.High]: 'PRIORITY.HIGH'
};
```

### 6. i18n-Keys für neue Felder vergessen

**Problem:** Template verwendet Übersetzungskeys, die nicht in de.json/en.json existieren.

**Lösung - Checkliste bei jeder Template-Erstellung:**
- [ ] Neue Keys in `de.json` hinzufügen
- [ ] Gleiche Keys in `en.json` hinzufügen
- [ ] Standard-Labels nicht vergessen: `ACTIVE`, `INACTIVE`, `DONE`, `OPEN`, `NO_DATA`, `BACK`

**Beispiel:**
```json
// de.json
"COMMON": {
  "ACTIVE": "Aktiv",
  "INACTIVE": "Inaktiv",
  "DONE": "Erledigt",
  "OPEN": "Offen",
  "NO_DATA": "Keine Daten vorhanden",
  "BACK": "Zurück"
}
```

### 7. Lazy-Loading mit AuthGuard

**Problem:** Routen für Entity-Listen nicht korrekt mit AuthGuard geschützt.

**Lösung - Standard-Pattern für geschützte Routen:**
```typescript
// In app-routing.module.ts
{ 
  path: 'to-do-lists', 
  loadComponent: () => import('./pages/entities/app/to-do-list-list.component').then(m => m.ToDoListListComponent), 
  canActivate: [AuthGuard], 
  title: 'ToDo Lists' 
},
```

---

## Vor-Template-Erstellung Checkliste

**VOR dem Erstellen eines Templates diese Schritte durchführen:**

### Für List-Komponenten:
- [ ] Interface in `src/app/models/entities/[subfolder]/i-entity-name.ts` öffnen
- [ ] Alle Properties notieren und deren Typen prüfen
- [ ] Fremdschlüssel-Beziehungen identifizieren (Navigation-Properties)
- [ ] i18n-Keys für BEIDE Sprachen vorbereiten

### Für Edit-Komponenten:
- [ ] Interface prüfen (wie oben)
- [ ] Fremdschlüssel identifizieren → Dropdown benötigt
- [ ] Enums identifizieren → Dropdown mit Enum-Werten benötigt
- [ ] Für jedes Dropdown:
  - [ ] Array-Property in Komponente definieren
  - [ ] Service-Import hinzufügen
  - [ ] `override ngOnInit()` implementieren
  - [ ] Daten-Lademethode erstellen
- [ ] i18n-Keys für BEIDE Sprachen vorbereiten

### Nach Template-Erstellung:
- [ ] `npm run build` ausführen und Fehler prüfen
- [ ] Alle TypeScript-Fehler beheben bevor fortgefahren wird

## Konventionen

### Naming
- **Komponenten**: PascalCase (`EntityNameListComponent`)
- **Dateien**: kebab-case (`entity-name-list.component.ts`)
- **Properties**: camelCase (`dataItems`, `searchTerm`)
- **CSS-Klassen**: Bootstrap-Klassen bevorzugen

### Icons
- **Bootstrap Icons** verwenden: `bi bi-plus`, `bi bi-pencil`, `bi bi-trash`
- **Konsistente Icon-Verwendung** für gleiche Aktionen
- **Tooltips** für bessere Benutzerführung

### Accessibility
- **aria-label** für Icon-Buttons
- **Semantic HTML** verwenden
- **Keyboard-Navigation** berücksichtigen
- **Screen-Reader** freundliche Struktur

## Troubleshooting

### Häufige Probleme
- **Import-Fehler**: Alle Standalone-Imports in `imports: []` Array
- **Routing-Fehler**: Route-Pfade in `app-routing.module.ts` prüfen
- **i18n-Fehler**: Translation-Keys in beiden Sprachdateien definieren
- **Bootstrap-Fehler**: Korrekte CSS-Klassen und responsive Utilities verwenden
- **TS4114 Override-Fehler**: Bei `ngOnInit()` immer `override` keyword verwenden
- **Template-Parser-Fehler**: Keine komplexen Casts in Event-Bindings, stattdessen `[(ngModel)]` verwenden
- **Property does not exist**: Interface prüfen und nur existierende Properties verwenden

### Best Practices
- **Standalone Components** immer verwenden
- **Separate HTML/CSS-Dateien** für bessere Wartbarkeit  
- **TypeScript strict mode** beachten
- **Angular OnPush** für Performance-Optimierung bei Bedarf
- **Interface-First**: VOR Template-Erstellung IMMER das Model-Interface prüfen
- **Build-Test**: Nach jeder größeren Änderung `npm run build` ausführen
