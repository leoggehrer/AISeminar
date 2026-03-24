Ergänze i18n-Übersetzungskeys in BEIDEN Sprachdateien gleichzeitig.

Frage den Benutzer nach: Komponente/Entity-Name und benötigte Labels.

## Dateipfade
- Deutsch: `src/assets/i18n/de.json`
- Englisch: `src/assets/i18n/en.json`

## Namens-Konventionen
| Bereich | Muster | Beispiel |
|---------|--------|---------|
| Listen | `ENTITYNAME_LIST.*` | `TODOITEM_LIST.TITLE` |
| Edit-Formulare | `ENTITYNAME_EDIT.*` | `TODOITEM_EDIT.CREATE_TITLE` |
| Detail-Seiten | `ENTITYNAME_DETAILS.*` | `DEPARTMENT_DETAILS.EMPLOYEES` |
| Dashboard | `DASHBOARD.CARDS.*` | `DASHBOARD.CARDS.TODOLIST_TITLE` |
| Enums | `ENUM_NAME.*` | `PRIORITY.LOW`, `TODO_STATE.OPEN` |

## Standard-Keys pro Komponente

### List
`TITLE`, `ADD_ITEM`, `SEARCH_PLACEHOLDER`, `REFRESH`, `EDIT`, `DELETE`, `BACK_TO_DASHBOARD`

### Edit
`CREATE_TITLE`, `EDIT_TITLE`, `BASIC_INFO`, `FIELDNAME`, `IS_ACTIVE`, `NO_SELECTION`

## Translate-Regel
- Im Template IMMER `| translate` Pipe verwenden
- `translateService.instant()` NUR für `MessageBoxService.confirm()` Parameter
- Title-Property gibt Key zurück, Template übersetzt

## Nach der Erstellung
- Beide Dateien auf JSON-Gültigkeit prüfen
- Browser prüfen ob alle Labels angezeigt werden (kein `[key]` sichtbar)
