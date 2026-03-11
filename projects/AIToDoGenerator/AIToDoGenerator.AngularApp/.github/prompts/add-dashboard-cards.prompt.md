---
description: "Fügt Dashboard-Cards für neue Entity-Listen hinzu, inkl. TypeScript-Cards-Array und i18n-Übersetzungen"
name: "Dashboard-Cards hinzufügen"
argument-hint: "Entity-Namen und Routen (z.B. ToDoList → /to-do-lists)"
agent: "agent"
---

# Dashboard-Cards hinzufügen

Füge für `$input` neue Cards im Dashboard hinzu.

## Pflichtschritte

### 1. Dashboard-Komponente prüfen
Öffne `src/app/pages/dashboard/dashboard.component.ts` und prüfe:
- Vorhandene Card-Struktur und Properties
- Vorhandene Icons und Farbschemas
- Import-Pfade für RouterModule

### 2. Cards-Array erweitern
Füge neue Einträge zum Cards-Array hinzu:
```typescript
// In dashboard.component.ts
{
  visible: true,
  title: 'DASHBOARD.CARDS.ENTITYNAME_TITLE',
  text: 'DASHBOARD.CARDS.ENTITYNAME_TEXT',
  type: '/entitynames',
  bg: 'bg-primary text-white',   // Farbe anpassen (Varianten unten)
  icon: 'bi bi-[passendes-icon]'
},
```

**Verfügbare Bootstrap-Hintergrundfarben:**
| Klasse | Verwendung |
|--------|-----------|
| `bg-primary text-white` | Primäre Entitäten |
| `bg-success text-white` | Aktive/positive Entitäten |
| `bg-info text-white` | Informations-Entitäten |
| `bg-warning text-dark` | Warn-/Prioritäts-Entitäten |
| `bg-danger text-white` | Kritische Entitäten |
| `bg-secondary text-white` | Sekundäre/unterstützende Entitäten |
| `bg-dark text-white` | System-/Admin-Entitäten |
| `bg-light text-dark` | Neutrale Entitäten |

**Bootstrap-Icons Empfehlungen:**
- Listen/Aufgaben: `bi-list-task`, `bi-check2-square`, `bi-journals`
- Personen: `bi-people`, `bi-person-badge`, `bi-person-circle`
- Organisationen: `bi-building`, `bi-diagram-3`, `bi-briefcase`
- Dokumente: `bi-file-text`, `bi-folder`, `bi-archive`
- System: `bi-gear`, `bi-sliders`, `bi-shield`
- Daten: `bi-database`, `bi-table`, `bi-graph-up`

### 3. Dashboard-HTML prüfen
Stelle sicher, dass `src/app/pages/dashboard/dashboard.component.html` die Cards dynamisch rendert (typischerweise mit `*ngFor`):
```html
<div *ngFor="let card of visibleCards" class="col-md-4 col-sm-6 mb-4">
    <a [routerLink]="card.type" class="card h-100 text-decoration-none [card.bg]">
        <div class="card-body text-center">
            <i class="[card.icon] fs-1 mb-3"></i>
            <h5 class="card-title">{{ card.title | translate }}</h5>
            <p class="card-text">{{ card.text | translate }}</p>
        </div>
    </a>
</div>
```

### 4. i18n-Übersetzungen ergänzen (beide Sprachen!)
In `src/assets/i18n/de.json`:
```json
"DASHBOARD": {
  "CARDS": {
    "ENTITYNAME_TITLE": "Entity-Name",
    "ENTITYNAME_TEXT": "Entity-Name verwalten"
  }
}
```

In `src/assets/i18n/en.json`:
```json
"DASHBOARD": {
  "CARDS": {
    "ENTITYNAME_TITLE": "Entity Name",
    "ENTITYNAME_TEXT": "Manage Entity Name"
  }
}
```

## Gestaltungsregeln
- **Responsive**: Dashboard muss auf allen Bildschirmgrößen gut aussehen
- **Verschiedene Farben**: Pro Entität eine andere Hintergrundfarbe für bessere Erkennbarkeit
- **Konsistente Icons**: Gleiche Icon-Bibliothek (Bootstrap Icons) für alle Cards
- **Aussagekräftige Texte**: Kurze, prägnante Beschreibungen

## Nach der Erstellung
- [ ] Dashboard im Browser prüfen (Responsive auf Mobile/Desktop)
- [ ] Alle Links funktionsfähig testen
- [ ] `npm run build` ausführen
