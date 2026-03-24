Diagnostiziere und behebe häufige Angular-Fehler in diesem Projekt.

Führe zunächst aus:
```bash
npm run build 2>&1 | head -50
```

## Bekannte Fehler & Lösungen

### TS4114: `override` fehlt
```typescript
// Falsch:
ngOnInit(): void { ... }
// Richtig:
override ngOnInit(): void { ... }
```

### Property does not exist
Interface öffnen, tatsächliche Property-Namen prüfen, Template korrigieren.

### Template-Parser-Fehler bei Event-Binding
`[(ngModel)]` verwenden statt komplexe `$event`-Ausdrücke.

### Enum nicht gefunden / Dropdown leer
```typescript
import { Priority } from '@app/enums/priority';
public priorities = Object.values(Priority).filter(v => typeof v === 'number') as number[];
```

### Dropdown-Daten fehlen
`override ngOnInit()` mit `super.ngOnInit()` und Service-Aufruf prüfen.

### i18n-Key wird als roher String angezeigt
Key in BEIDEN Dateien `de.json` und `en.json` hinzufügen.

### NgModel nicht erkannt
`FormsModule` in `imports[]` der Komponente hinzufügen.

### super.ngOnInit() fehlt
```typescript
override ngOnInit(): void {
  super.ngOnInit();  // IMMER als Erstes!
  this.loadAdditionalData();
}
```

## Diagnose-Workflow
1. `npm run build` → alle Fehler anzeigen
2. Fehler nach Typ kategorisieren
3. Lösungen anwenden
4. `npm run build` erneut bis fehlerfrei
