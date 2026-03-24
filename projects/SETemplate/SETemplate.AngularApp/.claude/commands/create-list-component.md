Erstelle das HTML-Template für eine List-Komponente (bereits generiert unter `src/app/pages/entities/`).

Frage den Benutzer nach dem Entity-Namen.

## Pflichtschritte

### 1. Interface prüfen
Öffne `src/app/models/entities/**/i-[entity-name].ts` und notiere alle Properties und Typen. Nur Properties aus dem Interface verwenden!

### 2. HTML-Template erstellen
Dateiname: `[entity-name]-list.component.html` in `src/app/pages/entities/[subfolder]/`

Struktur:
- Page-Header: Titel + Zurück-Button
- Toolbar: Hinzufügen + Suche + Aktualisieren
- List Items mit `@for` (NIEMALS `*ngFor`)
- Empty State

Verwende globale CSS-Klassen: `.list-container`, `.page-header`, `.list-toolbar`, `.modern-list-item`, `.list-item-header`, `.list-item-details`, `.list-item-actions`, `.empty-state`

Regeln:
- IMMER `@if/@for` Syntax (kein `*ngIf/*ngFor`)
- IMMER `track item.id ?? $index` bei `@for`
- CSS-Datei leer lassen — alle Styles aus globaler `styles.css`
- Alle Labels als i18n-Keys mit `| translate` Pipe
- **Toolbar**: Add-Button, Suchfeld und Refresh-Button **immer in einer Zeile** mit `d-flex align-items-center gap-2` (KEIN `row g-3`)
- **Buttons**: Jeder Button hat ein Bootstrap-Icon und ein `title`-Attribut als Tooltip
- **Suchfeld**: in `<div class="flex-grow-1">` einwickeln
- **Back-Button**: dezenter Icon-only Button (`btn btn-link text-secondary`) mit `(click)="goBack()"` und `title="{{ 'COMMON.BACK' | translate }}"`, Icon: `bi bi-arrow-left-circle-fill fs-5`

### 3. i18n-Übersetzungen ergänzen
BEIDE Dateien gleichzeitig: `src/assets/i18n/de.json` und `src/assets/i18n/en.json`

Keys: `ENTITYNAME_LIST.TITLE`, `ENTITYNAME_LIST.ADD_ITEM`, `ENTITYNAME_LIST.SEARCH_PLACEHOLDER`, etc.

## Nach der Erstellung
- Routing eintragen → `/configure-routing`
- Dashboard-Card hinzufügen → `/add-dashboard-cards`
- `npm run build` ausführen
