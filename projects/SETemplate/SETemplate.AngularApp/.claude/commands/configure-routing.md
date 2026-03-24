Konfiguriere Angular-Routing für Entity-Listen-Komponenten in `app-routing.module.ts`.

Frage den Benutzer nach den Entity-Routen.

## Pflichtschritte

### 1. Bestehende Routen-Datei prüfen
Öffne `src/app/app-routing.module.ts` und prüfe vorhandene Import-Pfade, AuthGuard, Lazy-Loading.

### 2. Routen eintragen

**Lazy-Loading (bevorzugt):**
```typescript
{
  path: 'entitynames',
  loadComponent: () => import('./pages/entities/[subfolder]/entityname-list.component')
    .then(m => m.EntityNameListComponent),
  canActivate: [AuthGuard],
  title: 'EntityNames'
},
```

**Master-Details-Route:**
```typescript
{
  path: 'masters/:id/details',
  loadComponent: () => import('./pages/entities/[subfolder]/master-details.component')
    .then(m => m.MasterDetailsComponent),
  canActivate: [AuthGuard],
  title: 'Details'
},
```

### 3. URL-Konventionen
- Entity-Namen als kebab-case: `ToDoItem` → `to-do-items`
- Plural-Form verwenden
- Spezifischere Routen ÜBER generischeren

## Nach der Konfiguration
- Dashboard-Links prüfen
- Navigation im Browser testen
- `npm run build` ausführen
