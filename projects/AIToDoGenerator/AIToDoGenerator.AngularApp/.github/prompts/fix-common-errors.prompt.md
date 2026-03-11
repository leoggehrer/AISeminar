---
description: "Diagnosiert und behebt häufige Angular-Fehler in diesem Projekt: TS4114 override, Property does not exist, Parser-Fehler, fehlende Dropdown-Daten, i18n-Fehler"
name: "Häufige Fehler beheben"
argument-hint: "Fehlermeldung oder Fehlerbeschreibung"
agent: "agent"
---

# Häufige Angular-Fehler diagnostizieren und beheben

Analysiere den Fehler `$input` und wende die passende Lösung an.

## Fehler-Diagnose

Führe zunächst aus:
```bash
npm run build 2>&1 | head -50
```
um alle aktuellen Fehler zu sehen.

---

## Bekannte Fehler & Lösungen

### 1. TS4114: `override` fehlt

**Fehlermeldung:**
```
error TS4114: This member must have an 'override' modifier because it overrides a member in the base class.
```

**Ursache:** `ngOnInit()`, `title` oder andere geerbte Methoden ohne `override`.

**Lösung:**
```typescript
// ❌ Falsch:
ngOnInit(): void { ... }
get title(): string { ... }

// ✅ Richtig:
override ngOnInit(): void { ... }
override get title(): string { ... }
```

---

### 2. Property does not exist on type

**Fehlermeldung:**
```
error TS2339: Property 'xyz' does not exist on type 'IEntityName'.
```

**Ursache:** Template oder TypeScript verwendet einen Property-Namen, der im Interface nicht existiert.

**Lösung:**
1. Interface öffnen: `src/app/models/entities/**/i-entity-name.ts`
2. Tatsächliche Property-Namen prüfen
3. Template/TypeScript korrigieren

**Häufige Verwechslungen:**
| Falsch | Richtig (prüfen!) |
|--------|-------------------|
| `.title` | `.name` |
| `.done` | `.isCompleted` |
| `.createdAt` | `.createdOn` |
| `.updatedAt` | `.modifiedOn` |

---

### 3. Template-Parser-Fehler bei Event-Binding

**Fehlermeldung:**
```
Parser Error: Unexpected token...
```

**Ursache:** Komplexe Ausdrücke in Event-Bindings:
```html
<!-- ❌ Falsch – verursacht Parser-Fehler: -->
(change)="setValue(($event.target as HTMLInputElement).value)"
```

**Lösung:** `[(ngModel)]` verwenden:
```html
<!-- ✅ Richtig: -->
<input type="date" [(ngModel)]="dataItem.dueDate" name="dueDate" />
```

---

### 4. Enum nicht gefunden / Enum-Werte leer

**Fehlermeldung:**
```
Cannot find name 'Priority'
```
oder Dropdown ist leer.

**Lösung:**
```typescript
// 1. Enum importieren:
import { Priority } from '@app/enums/priority';
// oder
import { Priority } from '../../enums/priority';

// 2. Numerische Werte extrahieren (nicht string-Werte!):
public priorities = Object.values(Priority)
  .filter(v => typeof v === 'number') as number[];
```

---

### 5. Dropdown-Daten fehlen (leere Selectbox)

**Ursache:** `ngOnInit()` nicht überschrieben oder Service nicht injiziert.

**Diagnose:** Prüfe ob in der Komponente `override ngOnInit()` vorhanden ist.

**Lösung:**
```typescript
// Service injizieren (nicht im Constructor, sondern mit inject()):
private relatedService = inject(RelatedEntityService);

// Array definieren:
public relatedEntities: IRelatedEntity[] = [];

// ngOnInit MUSS override sein und super aufrufen:
override ngOnInit(): void {
  super.ngOnInit();  // WICHTIG!
  this.loadRelatedEntities();
}

private loadRelatedEntities(): void {
  this.relatedService.getAll().subscribe({
    next: (data) => this.relatedEntities = data,
    error: (err) => console.error('Error:', err)
  });
}
```

---

### 6. i18n-Übersetzungskey wird als roher String angezeigt `[KEY_NAME]`

**Ursache:** Key existiert nicht in `de.json` oder `en.json`.

**Diagnose:** Key suchen:
```bash
grep -r "KEY_NAME" src/assets/i18n/
```

**Lösung:**
1. `src/assets/i18n/de.json` öffnen und Key hinzufügen
2. `src/assets/i18n/en.json` öffnen und Key hinzufügen
3. JSON-Syntax prüfen (korrekte Kommas, keine doppelten Keys)

---

### 7. Module nicht importiert (Component not found / NgModel)

**Fehlermeldung:**
```
Can't bind to 'ngModel' since it isn't a known property...
```

**Ursache:** `FormsModule` fehlt in `imports[]`.

**Lösung:** In `@Component` Decorator:
```typescript
@Component({
  standalone: true,
  imports: [
    CommonModule,   // *ngIf, *ngFor, | async
    FormsModule,    // [(ngModel)]
    RouterModule,   // routerLink, routerLinkActive
    TranslateModule // | translate
  ]
})
```

---

### 8. `super.ngOnInit()` wird nicht aufgerufen

**Symptom:** Basisdaten werden nicht geladen (leere Liste, kein `dataItem` im Edit).

**Lösung:**
```typescript
// IMMER super.ngOnInit() als ERSTEN Aufruf:
override ngOnInit(): void {
  super.ngOnInit();  // ← Zuerst!
  this.loadAdditionalData();
}
```

---

## Allgemeiner Diagnose-Workflow

1. `npm run build` → alle Fehler anzeigen
2. Fehler nach Typ kategorisieren (TS-Fehler, Template-Fehler, Runtime-Fehler)
3. Lösungen von oben anwenden
4. `npm run build` erneut prüfen bis keine Fehler mehr
