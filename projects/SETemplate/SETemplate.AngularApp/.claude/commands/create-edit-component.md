Erstelle HTML-Template und TypeScript für eine Edit-Komponente (bereits generiert unter `src/app/components/entities/`).

Frage den Benutzer nach dem Entity-Namen.

## Pflichtschritte

### 1. Interface ZUERST prüfen (KRITISCH!)
Öffne `src/app/models/entities/**/i-[entity-name].ts` und notiere:
- Alle Properties und Typen
- Fremdschlüssel → Dropdown mit Service-Daten
- Enum-Felder → Dropdown mit Enum-Werten
- Pflichtfelder vs. optionale Felder

### 2. HTML-Template erstellen
Dateiname: `[entity-name]-edit.component.html` in `src/app/components/entities/[subfolder]/`

Struktur: Bootstrap-Card mit Form-Sections + Save/Cancel-Buttons
- Textfeld: `<input class="form-control modern-form-control" [(ngModel)]="dataItem.field">`
- Dropdown FK: `<select class="form-select modern-form-select">` mit `@for`
- Dropdown Enum: `<select>` mit Enum-Werten
- Checkbox: `<input type="checkbox" class="form-check-input">`
- Datum: `<input type="date">`
- Alle Labels als i18n-Keys mit `| translate` Pipe
- CSS-Datei leer lassen

### 3. TypeScript erweitern
Dateiname: `[entity-name]-edit.component.ts`
- Bei Fremdschlüsseln: Service mit `inject()` laden
- Bei Enums: `Object.values(EnumType).filter(v => typeof v === 'number')`
- IMMER `override ngOnInit()` mit `super.ngOnInit()` aufrufen
- Title-Property gibt i18n-Key zurück (NICHT `translateService.instant()`)

### 4. i18n-Übersetzungen
BEIDE Dateien: `de.json` und `en.json`
Keys: `ENTITYNAME_EDIT.CREATE_TITLE`, `ENTITYNAME_EDIT.EDIT_TITLE`, `ENTITYNAME_EDIT.BASIC_INFO`, etc.

## Nach der Erstellung
- `npm run build` ausführen
