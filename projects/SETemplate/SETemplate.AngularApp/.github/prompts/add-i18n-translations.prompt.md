---
description: "Fügt i18n-Übersetzungskeys für neue Komponenten in de.json und en.json hinzu – immer beide Sprachen gleichzeitig"
name: "i18n-Übersetzungen hinzufügen"
argument-hint: "Komponente/Entity-Name und Liste der benötigten Labels"
agent: "agent"
---

# i18n-Übersetzungen hinzufügen

Ergänze Übersetzungskeys für `$input` in **beiden** Sprachdateien gleichzeitig.

> ⚠️ **Immer beide Dateien gleichzeitig bearbeiten!** Fehlende Keys in einer Sprache verursachen `[key]`-Anzeigen im UI.

## Dateipfade
- **Deutsch:** `src/assets/i18n/de.json`
- **Englisch:** `src/assets/i18n/en.json`

## Standard-Keys (COMMON – falls nicht vorhanden)
```json
"COMMON": {
  "SAVE": "Speichern",
  "CANCEL": "Abbrechen",
  "DELETE": "Löschen",
  "EDIT": "Bearbeiten",
  "BACK": "Zurück",
  "CLOSE": "Schließen",
  "ADD": "Hinzufügen",
  "SEARCH": "Suchen",
  "REFRESH": "Aktualisieren",
  "ACTIVE": "Aktiv",
  "INACTIVE": "Inaktiv",
  "YES": "Ja",
  "NO": "Nein",
  "NO_DATA": "Keine Daten vorhanden",
  "DELETE_TITLE": "Löschen bestätigen",
  "DELETE_CONFIRM": "Möchten Sie diesen Eintrag wirklich löschen?"
}
```

## Namens-Konventionen

| Bereich | Muster | Beispiel |
|---------|--------|---------|
| Listen | `ENTITYNAME_LIST.*` | `TODOITEM_LIST.TITLE` |
| Edit-Formulare | `ENTITYNAME_EDIT.*` | `TODOITEM_EDIT.CREATE_TITLE` |
| Detail-Seiten | `ENTITYNAME_DETAILS.*` | `DEPARTMENT_DETAILS.EMPLOYEES` |
| Dashboard | `DASHBOARD.CARDS.*` | `DASHBOARD.CARDS.TODOLIST_TITLE` |
| Enums | `ENUM_NAME.*` | `PRIORITY.LOW`, `TODO_STATE.OPEN` |

## Vollständige Keys pro Komponente

### List-Komponente
```json
"ENTITYNAME_LIST": {
  "TITLE": "Entity-Titel",
  "ADD_ITEM": "Neues Entity hinzufügen",
  "SEARCH_PLACEHOLDER": "Nach Entity suchen...",
  "REFRESH": "Aktualisieren",
  "EDIT": "Bearbeiten",
  "DELETE": "Löschen",
  "BACK_TO_DASHBOARD": "Zum Dashboard"
}
```

### Edit-Formular
```json
"ENTITYNAME_EDIT": {
  "CREATE_TITLE": "Neues Entity erstellen",
  "EDIT_TITLE": "Entity bearbeiten",
  "BASIC_INFO": "Grundinformationen",
  "FIELDNAME": "Feldname",
  "IS_ACTIVE": "Aktiv",
  "NO_SELECTION": "– Keine Auswahl –"
}
```

### Enum-Labels (falls vorhanden)
```json
"PRIORITY": {
  "LOW": "Niedrig",
  "MEDIUM": "Mittel",
  "HIGH": "Hoch",
  "URGENT": "Dringend"
},
"TODO_STATE": {
  "OPEN": "Offen",
  "IN_PROGRESS": "In Bearbeitung",
  "DONE": "Erledigt"
}
```

## Übersetzungs-Strategie

**Im Template:**
```html
<!-- ✅ Richtig: translate Pipe verwenden -->
{{ 'ENTITYNAME_LIST.TITLE' | translate }}
{{ title | translate }}
```

**Im TypeScript (nur für MessageBox, nicht für Template):**
```typescript
// ✅ Richtig: instant() NUR für MessageBox-Parameter
const title = this.translateService.instant('COMMON.DELETE_TITLE');
this.messageBoxService.confirm(title, message);

// ❌ Falsch: instant() für Template-Ausgabe
get title() { return this.translateService.instant('...'); } // NICHT tun!
```

**Property gibt Key zurück (kein instant!):**
```typescript
// ✅ Richtig: Key zurückgeben, Template übersetzt
get title(): string { 
  return this.editMode ? 'ENTITY_EDIT.EDIT_TITLE' : 'ENTITY_EDIT.CREATE_TITLE'; 
}
```

## Nach der Erstellung
- [ ] Beide Dateien auf JSON-Gültigkeit prüfen (korrekte Kommas, keine doppelten Keys)
- [ ] Browser prüfen ob alle Labels angezeigt werden (kein `[key]` sichtbar)
