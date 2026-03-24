Füge Dashboard-Cards für neue Entity-Listen hinzu.

Frage den Benutzer nach den Entity-Namen und gewünschten Icons/Farben.

## Pflichtschritte

### 1. Dashboard-Komponente prüfen
Öffne `src/app/pages/dashboard/dashboard.component.ts` und prüfe vorhandene Card-Struktur.

### 2. Cards-Array erweitern
```typescript
{
  visible: true,
  title: 'DASHBOARD.CARDS.ENTITYNAME_TITLE',
  text: 'DASHBOARD.CARDS.ENTITYNAME_TEXT',
  type: '/entitynames',
  bg: 'bg-primary text-white',
  icon: 'bi bi-[passendes-icon]'
},
```

Verfügbare Farben: `bg-primary`, `bg-success`, `bg-info`, `bg-warning text-dark`, `bg-danger`, `bg-secondary`, `bg-dark`, `bg-light text-dark`

Bootstrap-Icons: `bi-list-task`, `bi-people`, `bi-building`, `bi-file-text`, `bi-gear`, `bi-database`, `bi-graph-up`, etc.

### 3. i18n-Übersetzungen (BEIDE Sprachen!)
```json
"DASHBOARD": {
  "CARDS": {
    "ENTITYNAME_TITLE": "Entity-Name",
    "ENTITYNAME_TEXT": "Entity-Name verwalten"
  }
}
```

## Nach der Erstellung
- Dashboard im Browser prüfen (Responsive)
- Alle Links testen
- `npm run build` ausführen
