---
description: "Konfiguriert Angular-Routing für Entity-Listen-Komponenten in app-routing.module.ts, inkl. Lazy-Loading und AuthGuard"
name: "Entity-Routing konfigurieren"
argument-hint: "Liste der Entity-Routen (z.B. ToDoList → /to-do-lists, ToDoItem → /to-do-items)"
agent: "agent"
---

# Entity-Routing konfigurieren

Füge die Routen für `$input` in `src/app/app-routing.module.ts` ein.

## Pflichtschritte

### 1. Bestehende Routen-Datei prüfen
Öffne `src/app/app-routing.module.ts` und prüfe:
- Vorhandene Import-Pfade und Muster
- Ob AuthGuard bereits verwendet wird
- Ob Lazy-Loading oder direkte Imports genutzt werden

### 2. Routen eintragen

**Standard-Muster mit Lazy-Loading (bevorzugt):**
```typescript
// import
// Keine statischen Imports nötig bei Lazy-Loading

// Route-Eintrag
{
  path: 'entitynames',
  loadComponent: () => import('./pages/entities/[subfolder]/entityname-list.component')
    .then(m => m.EntityNameListComponent),
  canActivate: [AuthGuard],
  title: 'EntityNames'
},
```

**Alternatives Muster mit direktem Import:**
```typescript
// import oben in der Datei:
import { EntityNameListComponent } from './pages/entities/[subfolder]/entityname-list.component';

// Route-Eintrag:
{ path: 'entitynames', component: EntityNameListComponent, canActivate: [AuthGuard], title: 'EntityNames' },
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
- Entity-Namen als **kebab-case** in der URL: `ToDoItem` → `to-do-items`
- Plural-Form verwenden: `to-do-lists`, `employees`, `departments`
- Konsistenz mit bestehenden Routen sicherstellen

### 4. Reihenfolge beachten
Routen werden von oben nach unten gemacht – spezifischere Routen ÜBER generischeren.
```typescript
// ✅ Korrekte Reihenfolge:
{ path: 'items/:id', ... },   // spezifisch zuerst
{ path: 'items', ... },       // generisch danach
{ path: '**', redirectTo: 'dashboard' }  // Fallback ganz unten
```

## Nach der Konfiguration
- [ ] Dashboard-Links für alle neuen Routen prüfen
- [ ] Navigation im Browser testen
- [ ] `npm run build` ausführen und alle Fehler prüfen
