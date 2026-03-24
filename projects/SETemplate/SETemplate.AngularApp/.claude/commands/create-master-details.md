Erstelle eine neue standalone Master-Details-Komponente mit readonly Master-Ansicht und bearbeitbarer Detail-Liste.

Frage den Benutzer nach: Master-Entity und Detail-Entity.

## Pflichtschritte

### 1. Interfaces prüfen
Öffne beide Interfaces und notiere alle Properties:
- `src/app/models/entities/**/i-master-entity.ts` (readonly angezeigt)
- `src/app/models/entities/**/i-detail-entity.ts` (CRUD mit Aktionsbuttons)

### 2. Neue Komponente anlegen
Pfad: `src/app/pages/entities/[subfolder]/masterentity-detailsentity.component.*`

TypeScript: Standalone Component mit `OnInit`, Services für Master und Detail per `inject()`, Modal für Edit-Komponente, MessageBox für Delete-Bestätigung.

### 3. HTML-Template erstellen
- Master: readonly Bootstrap-Card oben
- Details: bearbeitbare Liste darunter mit Add/Edit/Delete
- Verwende globale CSS-Klassen
- Alle Labels als i18n-Keys

### 4. Route hinzufügen
In `app-routing.module.ts`:
```typescript
{ path: 'master/:id/details', component: MasterDetailsComponent, title: 'Details' }
```

### 5. i18n-Übersetzungen
BEIDE Dateien: `de.json` und `en.json`

## Nach der Erstellung
- Link in Master-List-Ansicht ergänzen
- `npm run build` ausführen
