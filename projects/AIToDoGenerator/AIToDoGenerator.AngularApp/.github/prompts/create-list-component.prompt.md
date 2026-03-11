---
description: "Erstellt eine Angular List-Komponente (HTML-Template) für eine Entity mit Bootstrap-Card-Layout, Suche, Toolbar und i18n-Übersetzungen"
name: "List-Komponente erstellen"
argument-hint: "Entity-Name (z.B. ToDoItem, Department)"
agent: "agent"
---

# Angular List-Komponente erstellen

Erstelle das **HTML-Template** für die List-Komponente `$input` (bereits generiert unter `src/app/pages/entities/`).

## Pflichtschritte – in dieser Reihenfolge

### 1. Interface prüfen
Öffne `src/app/models/entities/**/i-$input.ts` und notiere:
- Alle Properties und deren Typen
- Pflichtfelder und optionale Felder
- Fremdschlüssel (Navigation-Properties wie `toDoList`, `department.name`)
- Enum-Felder

### 2. HTML-Template erstellen
Dateiname: `[entity-name]-list.component.html` in `src/app/pages/entities/[subfolder]/`

Verwende folgende Struktur:
```html
<!--@AiCode-->
<div class="list-container">
    <div class="container mt-4">
        <!-- Page Header: Titel + Zurück-Button -->
        <div class="d-flex justify-content-between align-items-center mb-4 p-3 page-header">
            <h3 class="mb-0 flex-grow-1">
                <i class="bi bi-[passendes-icon] me-2"></i>
                {{ 'ENTITYNAME_LIST.TITLE' | translate }}
            </h3>
            <a routerLink="/dashboard" class="btn btn-light">
                <i class="bi bi-arrow-left-circle me-2"></i>
                <span class="d-none d-md-inline">{{ 'COMMON.BACK' | translate }}</span>
            </a>
        </div>

        <!-- Toolbar: Hinzufügen + Suche + Aktualisieren -->
        <div class="list-toolbar">
            <div class="row g-3 align-items-center">
                <div class="col-auto">
                    <button *ngIf="canAdd" class="btn btn-primary" (click)="addCommand()">
                        <i class="bi bi-plus-circle me-2"></i>
                        <span class="d-none d-sm-inline">{{ 'ENTITYNAME_LIST.ADD_ITEM' | translate }}</span>
                    </button>
                </div>
                <div class="col">
                    <input *ngIf="canSearch" type="text" class="form-control"
                           [(ngModel)]="searchTerm"
                           [placeholder]="'ENTITYNAME_LIST.SEARCH_PLACEHOLDER' | translate">
                </div>
                <div class="col-auto">
                    <button *ngIf="canRefresh" class="btn btn-outline-primary" (click)="reloadData()">
                        <i class="bi bi-arrow-clockwise"></i>
                    </button>
                </div>
            </div>
        </div>

        <!-- List Items -->
        <div class="list-items">
            <div *ngFor="let item of dataItems" class="modern-list-item">
                <!-- Item Header: Icon + Titel + Status-Badge -->
                <div class="list-item-header">
                    <div class="list-item-icon">
                        <i class="bi bi-[passendes-icon]"></i>
                    </div>
                    <div class="flex-grow-1">
                        <h5 class="list-item-title">{{ item.[hauptfeld] }}</h5>
                    </div>
                    <!-- Optionaler Status-Badge -->
                </div>

                <!-- Details Grid -->
                <div class="list-item-details">
                    <!-- detail-row pro Eigenschaft -->
                </div>

                <!-- Aktions-Buttons -->
                <div class="list-item-actions">
                    <button *ngIf="canEdit" class="btn btn-outline-secondary" (click)="editCommand(item)">
                        <i class="bi bi-pencil me-2"></i>{{ 'ENTITYNAME_LIST.EDIT' | translate }}
                    </button>
                    <button *ngIf="canDelete" class="btn btn-outline-danger" (click)="deleteCommand(item)">
                        <i class="bi bi-trash me-2"></i>{{ 'ENTITYNAME_LIST.DELETE' | translate }}
                    </button>
                </div>
            </div>

            <!-- Empty State -->
            <div *ngIf="dataItems.length === 0" class="empty-state">
                <div class="empty-state-icon"><i class="bi bi-inbox"></i></div>
                <p class="empty-state-text">{{ 'COMMON.NO_DATA' | translate }}</p>
            </div>
        </div>
    </div>
</div>
```

### 3. i18n-Übersetzungen ergänzen
Ergänze **beide** Dateien gleichzeitig:
- `src/assets/i18n/de.json`
- `src/assets/i18n/en.json`

Mindest-Keys:
```json
"ENTITYNAME_LIST": {
  "TITLE": "...",
  "ADD_ITEM": "...",
  "SEARCH_PLACEHOLDER": "...",
  "EDIT": "Bearbeiten",
  "DELETE": "Löschen",
  "BACK_TO_DASHBOARD": "Zurück zum Dashboard"
}
```

## CSS-Regel
CSS-Datei der Komponente **leer lassen** – alle Styles kommen aus der globalen `styles.css`.

## Nach der Erstellung
- [ ] Routing in `app-routing.module.ts` eintragen
- [ ] Dashboard-Card hinzufügen
- [ ] `npm run build` ausführen und Fehler prüfen
