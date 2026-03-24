# Beispiel: ToDoList-App – Angular Frontend

> Fortsetzung von `ToDo_Sample.md` (Schritte 1–8, Backend).
> Voraussetzung: Backend-Code wurde generiert (`dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x`).

---

## Übersicht der zu erstellenden Komponenten

| Entity | List | Edit-Formular | Master-Details |
|--------|------|---------------|----------------|
| `Category` | ✅ category-list | ✅ category-edit | – |
| `Tag` | ✅ tag-list | ✅ tag-edit | – |
| `ToDoList` | ✅ to-do-list-list | ✅ to-do-list-edit | ✅ to-do-list-items |
| `ToDoItem` | ✅ to-do-item-list | ✅ to-do-item-edit (Dropdowns!) | – |
| `ToDoItemTag` | optional | optional | – |

**Reihenfolge:** Stammdaten zuerst (`Category`, `Tag`), dann abhängige Entities!

---

## Schritt 9 – Vor-Template-Checkliste für alle Entities

Für `Category`, `Tag`, `ToDoList`, `ToDoItem` je das Interface analysieren:

| Entity | Interface-Datei | Fremdschlüssel | Enums |
|--------|----------------|----------------|-------|
| `Category` | `i-category.ts` | – | – |
| `Tag` | `i-tag.ts` | – | – |
| `ToDoList` | `i-to-do-list.ts` | – | – |
| `ToDoItem` | `i-to-do-item.ts` | `toDoListId`, `categoryId` | – |

**Einstiegssatz an die AI:**
> "Führe für alle Entities der ToDoList-App (`Category`, `Tag`, `ToDoList`, `ToDoItem`) die Vor-Template-Checkliste aus. Öffne die jeweiligen `i-*.ts` Interfaces und notiere alle Felder, Fremdschlüssel und Enum-Typen."

→ Details: `.github/prompts/pre-template-checklist.prompt.md`

---

## Schritt 10 – List-Komponenten erstellen

### Category-List (`category-list.component.html`)

Einfache Liste ohne Fremdschlüssel:
- Anzeige: `name`, `description`
- Suche nach Name/Description

### Tag-List (`tag-list.component.html`)

Einfache Liste:
- Anzeige: `name`

### ToDoList-List (`to-do-list-list.component.html`)

- Anzeige: `title`, `description`, `createdOn` (formatiert als Datum)
- Badge: Anzahl der enthaltenen `toDoItems`

### ToDoItem-List (`to-do-item-list.component.html`)

Komplex – mit Badges und Status-Anzeige:
- Anzeige: `title`, `isDone` (Status-Badge), `dueDate`
- Verknüpfte Daten: `toDoList?.title`, `category?.name`
- Status-Badge: Grün = erledigt, Rot = überfällig, Gelb = offen

**Beispiel Status-Badge in `@for`-Schleife:**

```html
<!--@AiCode-->
@for (item of dataItems; track item.id ?? $index) {
<div class="modern-list-item">
    <div class="list-item-header">
        <div class="flex-grow-1">
            <h5 class="list-item-title">{{ item.title }}</h5>
            <small class="text-muted">{{ item.toDoList?.title }}</small>
        </div>
        @if (item.isDone) {
        <span class="badge bg-success">{{ 'TODOITEM_LIST.DONE' | translate }}</span>
        } @else {
        <span class="badge bg-warning text-dark">{{ 'TODOITEM_LIST.OPEN' | translate }}</span>
        }
        <div class="list-item-actions">
            <button class="btn btn-sm btn-outline-primary" (click)="editCommand(item)">
                <i class="bi bi-pencil"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger" (click)="deleteCommand(item)">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    </div>
</div>
}
```

→ Details: `.github/prompts/create-list-component.prompt.md`

---

## Schritt 11 – Edit-Formulare erstellen

### Category-Edit & Tag-Edit

Einfache Formulare ohne Dropdowns:

```html
<!--@AiCode-->
@if (dataItem) {
<div class="modern-edit-container">
    <div class="modern-edit-card">
        <div class="modern-edit-header d-flex justify-content-between align-items-center">
            <h3><i class="bi bi-tag me-2"></i>{{ title | translate }}</h3>
            <button type="button" class="btn-close-custom" (click)="dismiss()">&times;</button>
        </div>
        <div class="modern-edit-body">
            <div class="form-section">
                <div class="modern-form-group">
                    <label class="modern-form-label">{{ 'CATEGORY_EDIT.NAME' | translate }}</label>
                    <input type="text" class="modern-form-control"
                           [(ngModel)]="dataItem.name" required />
                </div>
                <div class="modern-form-group">
                    <label class="modern-form-label">{{ 'CATEGORY_EDIT.DESCRIPTION' | translate }}</label>
                    <textarea class="modern-form-control"
                              [(ngModel)]="dataItem.description" rows="3"></textarea>
                </div>
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

---

### ToDoItem-Edit (mit Dropdowns)

Komplexes Formular mit zwei Fremdschlüssel-Dropdowns.

**TypeScript (`to-do-item-edit.component.ts`):**

```typescript
//@AiCode
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ToDoItemBaseEditComponent } from '@app/components/entities/app/to-do-item-base-edit.component';
import { IToDoList } from '@app-models/entities/app/i-to-do-list';
import { ICategory } from '@app-models/entities/data/i-category';
import { ToDoListService } from '@app-services/http/app/to-do-list.service';
import { CategoryService } from '@app-services/http/data/category.service';

@Component({
  standalone: true,
  selector: 'app-to-do-item-edit',
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './to-do-item-edit.component.html',
  styleUrl: './to-do-item-edit.component.css'
})
export class ToDoItemEditComponent extends ToDoItemBaseEditComponent {
  toDoLists: IToDoList[] = [];
  categories: ICategory[] = [];

  constructor(
    private toDoListService: ToDoListService,
    private categoryService: CategoryService
  ) {
    super();
  }

  override ngOnInit(): void {
    super.ngOnInit();
    this.toDoListService.getAll().subscribe(r => this.toDoLists = r.items ?? []);
    this.categoryService.getAll().subscribe(r => this.categories = r.items ?? []);
  }

  override get title(): string {
    return this.editMode ? 'TODOITEM_EDIT.EDIT_TITLE' : 'TODOITEM_EDIT.CREATE_TITLE';
  }
}
```

**HTML-Template (`to-do-item-edit.component.html`):**

```html
<!--@AiCode-->
@if (dataItem) {
<div class="modern-edit-container">
    <div class="modern-edit-card">
        <div class="modern-edit-header d-flex justify-content-between align-items-center">
            <h3><i class="bi bi-check2-square me-2"></i>{{ title | translate }}</h3>
            <button type="button" class="btn-close-custom" (click)="dismiss()">&times;</button>
        </div>
        <div class="modern-edit-body">
            <div class="form-section">

                <!-- Titel -->
                <div class="modern-form-group">
                    <label class="modern-form-label">{{ 'TODOITEM_EDIT.TITLE' | translate }}</label>
                    <input type="text" class="modern-form-control"
                           [(ngModel)]="dataItem.title" required />
                </div>

                <!-- ToDoList-Dropdown -->
                <div class="modern-form-group">
                    <label class="modern-form-label">{{ 'TODOITEM_EDIT.TODO_LIST' | translate }}</label>
                    <select class="modern-form-select" [(ngModel)]="dataItem.toDoListId" required>
                        <option [ngValue]="null">{{ 'COMMON.PLEASE_SELECT' | translate }}</option>
                        @for (list of toDoLists; track list.id) {
                        <option [ngValue]="list.id">{{ list.title }}</option>
                        }
                    </select>
                </div>

                <!-- Category-Dropdown -->
                <div class="modern-form-group">
                    <label class="modern-form-label">{{ 'TODOITEM_EDIT.CATEGORY' | translate }}</label>
                    <select class="modern-form-select" [(ngModel)]="dataItem.categoryId" required>
                        <option [ngValue]="null">{{ 'COMMON.PLEASE_SELECT' | translate }}</option>
                        @for (cat of categories; track cat.id) {
                        <option [ngValue]="cat.id">{{ cat.name }}</option>
                        }
                    </select>
                </div>

                <!-- Fälligkeitsdatum -->
                <div class="modern-form-group">
                    <label class="modern-form-label">{{ 'TODOITEM_EDIT.DUE_DATE' | translate }}</label>
                    <input type="date" class="modern-form-control" [(ngModel)]="dataItem.dueDate" />
                </div>

                <!-- Erledigt-Checkbox -->
                <div class="modern-form-group form-check">
                    <input type="checkbox" class="form-check-input" id="isDone"
                           [(ngModel)]="dataItem.isDone" />
                    <label class="form-check-label ms-2" for="isDone">
                        {{ 'TODOITEM_EDIT.IS_DONE' | translate }}
                    </label>
                </div>

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

→ Details: `.github/prompts/create-edit-component.prompt.md`

---

## Schritt 12 – Master-Details: ToDoList + ToDoItems

Zeigt eine ToDoList mit ihren ToDoItems in einer kombinierten Ansicht:

- **Master** (oben): ToDoList-Details readonly (Titel, Beschreibung, Datum)
- **Details** (unten): Liste der ToDoItems mit Edit-Möglichkeit

**Einstiegssatz an die AI:**
> "Erstelle eine Master-Details-Komponente `to-do-list-items` mit ToDoList als Master und ToDoItems als Details. Folge `.github/prompts/create-master-details.prompt.md`."

→ Details: `.github/prompts/create-master-details.prompt.md`

---

## Schritt 13 – Routing konfigurieren

Routen für alle Entities eintragen:

| Entity | Pfad | Komponente |
|--------|------|------------|
| `Category` | `/categories` | `CategoryListComponent` |
| `Tag` | `/tags` | `TagListComponent` |
| `ToDoList` | `/to-do-lists` | `ToDoListListComponent` |
| `ToDoItem` | `/to-do-items` | `ToDoItemListComponent` |
| Master-Details | `/to-do-lists/:id/items` | `ToDoListItemsComponent` |

→ Details: `.github/prompts/configure-routing.prompt.md`

---

## Schritt 14 – Dashboard-Cards ergänzen

Für jede Entity eine Card im Dashboard hinterlegen:

```html
<!--@AiCode-->
<!-- ToDoList Card -->
<div class="col-md-4">
    <a routerLink="/to-do-lists" class="text-decoration-none">
        <div class="card dashboard-card">
            <div class="card-body text-center">
                <i class="bi bi-list-task display-4 text-primary"></i>
                <h5 class="mt-2">{{ 'DASHBOARD.TODO_LISTS' | translate }}</h5>
            </div>
        </div>
    </a>
</div>

<!-- ToDoItem Card -->
<div class="col-md-4">
    <a routerLink="/to-do-items" class="text-decoration-none">
        <div class="card dashboard-card">
            <div class="card-body text-center">
                <i class="bi bi-check2-square display-4 text-success"></i>
                <h5 class="mt-2">{{ 'DASHBOARD.TODO_ITEMS' | translate }}</h5>
            </div>
        </div>
    </a>
</div>
```

→ Details: `.github/prompts/add-dashboard-cards.prompt.md`

---

## Schritt 15 – i18n-Übersetzungen ergänzen

**Gleichzeitig** in `de.json` und `en.json` ergänzen:

**`de.json` (Ausschnitt):**

```json
{
  "CATEGORY_LIST": {
    "TITLE": "Kategorien",
    "ADD_ITEM": "Kategorie hinzufügen",
    "SEARCH_PLACEHOLDER": "Kategorie suchen..."
  },
  "CATEGORY_EDIT": {
    "EDIT_TITLE": "Kategorie bearbeiten",
    "CREATE_TITLE": "Neue Kategorie",
    "NAME": "Name",
    "DESCRIPTION": "Beschreibung"
  },
  "TODOLIST_LIST": {
    "TITLE": "To-Do Listen",
    "ADD_ITEM": "Liste hinzufügen",
    "SEARCH_PLACEHOLDER": "Liste suchen..."
  },
  "TODOLIST_EDIT": {
    "EDIT_TITLE": "Liste bearbeiten",
    "CREATE_TITLE": "Neue Liste",
    "TITLE": "Titel",
    "DESCRIPTION": "Beschreibung"
  },
  "TODOITEM_LIST": {
    "TITLE": "Aufgaben",
    "ADD_ITEM": "Aufgabe hinzufügen",
    "SEARCH_PLACEHOLDER": "Aufgabe suchen...",
    "DONE": "Erledigt",
    "OPEN": "Offen"
  },
  "TODOITEM_EDIT": {
    "EDIT_TITLE": "Aufgabe bearbeiten",
    "CREATE_TITLE": "Neue Aufgabe",
    "TITLE": "Titel",
    "TODO_LIST": "Liste",
    "CATEGORY": "Kategorie",
    "DUE_DATE": "Fällig am",
    "IS_DONE": "Erledigt"
  },
  "DASHBOARD": {
    "TODO_LISTS": "To-Do Listen",
    "TODO_ITEMS": "Aufgaben",
    "CATEGORIES": "Kategorien",
    "TAGS": "Tags"
  }
}
```

**`en.json` (Ausschnitt):**

```json
{
  "CATEGORY_LIST": {
    "TITLE": "Categories",
    "ADD_ITEM": "Add Category",
    "SEARCH_PLACEHOLDER": "Search categories..."
  },
  "TODOITEM_LIST": {
    "TITLE": "Tasks",
    "ADD_ITEM": "Add Task",
    "SEARCH_PLACEHOLDER": "Search tasks...",
    "DONE": "Done",
    "OPEN": "Open"
  },
  "TODOITEM_EDIT": {
    "EDIT_TITLE": "Edit Task",
    "CREATE_TITLE": "New Task",
    "TITLE": "Title",
    "TODO_LIST": "List",
    "CATEGORY": "Category",
    "DUE_DATE": "Due date",
    "IS_DONE": "Done"
  },
  "DASHBOARD": {
    "TODO_LISTS": "To-Do Lists",
    "TODO_ITEMS": "Tasks",
    "CATEGORIES": "Categories",
    "TAGS": "Tags"
  }
}
```

→ Details: `.github/prompts/add-i18n-translations.prompt.md`

---

## Schritt 16 – Build & Test

```bash
cd SETemplate.AngularApp
npm run build

# Für Entwicklung mit Hot-Reload:
npm start
```

**Checkliste:**

- [ ] Alle Listen laden Daten korrekt aus der API
- [ ] Erstellen / Bearbeiten speichert korrekt
- [ ] `ToDoItem`-Edit: beide Dropdowns (ToDoList, Category) befüllt
- [ ] Master-Details zeigt ToDoList + zugehörige ToDoItems
- [ ] Routing: alle Pfade erreichbar, kein 404
- [ ] Dashboard: alle Entity-Cards sichtbar und Links funktionieren
- [ ] i18n: alle Keys in DE **und** EN vorhanden
- [ ] Keine `*ngIf/*ngFor` Warnungen in der Browser-Konsole
