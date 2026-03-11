---
description: "Checkliste und Pflichtprüfungen VOR der Erstellung von Angular-Templates: Interface verifizieren, Felder prüfen, Dropdowns planen"
name: "Vor-Template-Checkliste"
argument-hint: "Entity-Name (z.B. ToDoItem)"
agent: "agent"
---

# Pflichtcheckliste vor Template-Erstellung

Führe diese Prüfungen für `$input` durch, BEVOR du ein HTML-Template erstellst.

> ⚠️ **Dieser Schritt ist KRITISCH.** Überspringen führt zu Fehlern wie nicht existierenden Properties, fehlenden Dropdowns oder falschen Feldnamen.

## Schritt 1: Model-Interface öffnen und analysieren

```
src/app/models/entities/**/i-[entity-name].ts
```

**Notiere folgende Informationen:**

### Alle Properties (VOLLSTÄNDIGE Liste)
```
[ ] property1: Typ          (z.B. id: number)
[ ] property2: Typ          (z.B. name: string)
[ ] property3?: Typ         (optionaler Wert)
```

### Fremdschlüssel identifizieren → Dropdown nötig!
```
[ ] [propertyId]: number | null    → Dropdown mit Service-Daten
    zugehörige Navigation-Property: [property]: IRelatedEntity | null
```

### Enum-Felder identifizieren → Dropdown mit Enum-Werten nötig!
```
[ ] [enumProperty]: EnumType      → Import des Enum-Typs nötig
    Enum-Datei: src/app/enums/[enum-file].ts
```

### Status/Boolean-Felder → Checkbox
```
[ ] isActive?: boolean
[ ] isCompleted?: boolean
```

## Schritt 2: Enum-Definitionen prüfen

Für jeden gefundenen Enum-Typ, öffne `src/app/enums/[enum-file].ts` und notiere alle Werte:
```typescript
// Beispiel:
export enum Priority {
  Low = 0,
  Medium = 1,
  High = 2
}
// → priorities = [0, 1, 2] im Component
```

## Schritt 3: Abhängige Services bestimmen

Für jeden Fremdschlüssel, prüfe den Service-Pfad:
```
src/app/services/entities/**/[entity]-service.ts
```
→ Notiere: Import-Pfad, Service-Klasse, `getAll()`-Methode

## Schritt 4: Edit-Komponente für Dropdowns vorbereiten

Wenn Fremdschlüssel oder Enums gefunden wurden, plane die TypeScript-Ergänzungen:

```typescript
// Für JEDEN Fremdschlüssel:
public relatedEntities: IRelatedEntity[] = [];
private relatedService = inject(RelatedService);

// Für JEDEN Enum:
public enumValues = Object.values(EnumType).filter(v => typeof v === 'number') as number[];

// ngOnInit MUSS override sein:
override ngOnInit(): void {
  super.ngOnInit();
  this.loadRelatedEntities();
}
```

## Schritt 5: Template-Feldnamen festlegen

**Häufige Fallen:**
| Annahme (FALSCH) | Tatsächlich (PRÜFEN!) |
|------------------|----------------------|
| `item.title` | könnte `item.name` sein |
| `item.done` | könnte `item.isCompleted` sein |
| `item.isActive` | könnte gar nicht existieren |
| `item.createdAt` | könnte `item.createdOn` sein |
| `item.updatedAt` | könnte `item.modifiedOn` sein |

## Schritt 6: i18n-Keys planen

Erstelle Liste aller benötigten Übersetzungskeys:
```
[ ] ENTITYNAME_LIST.TITLE
[ ] ENTITYNAME_LIST.ADD_ITEM
[ ] ENTITYNAME_EDIT.CREATE_TITLE
[ ] ENTITYNAME_EDIT.EDIT_TITLE
[ ] ENTITYNAME_EDIT.[FIELD_NAME] für jeden Feldlabel
```

## Freigabe-Kriterien

Erst wenn alle Punkte abgehakt sind, Template erstellen:
- [ ] Interface vollständig gelesen
- [ ] Alle Properties und Typen bekannt
- [ ] Fremdschlüssel und Dropdowns identifiziert
- [ ] Enum-Typen und Werte geprüft
- [ ] Services für alle Dropdowns gefunden
- [ ] i18n-Keys geplant (beide Sprachen)
- [ ] TypeScript-Ergänzungen geplant (falls Dropdowns nötig)
