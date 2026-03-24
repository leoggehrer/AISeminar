Pflichtcheckliste vor Template-Erstellung. Führe diese Prüfungen durch, BEVOR du ein HTML-Template erstellst.

Frage den Benutzer nach dem Entity-Namen.

## Schritt 1: Model-Interface öffnen und analysieren
Öffne `src/app/models/entities/**/i-[entity-name].ts` und notiere:
- Alle Properties und deren Typen (VOLLSTÄNDIGE Liste)
- Fremdschlüssel identifizieren → Dropdown nötig
- Enum-Felder identifizieren → Dropdown mit Enum-Werten nötig
- Status/Boolean-Felder → Checkbox

## Schritt 2: Enum-Definitionen prüfen
Für jeden Enum-Typ öffne `src/app/enums/[enum-file].ts` und notiere alle Werte.

## Schritt 3: Abhängige Services bestimmen
Für jeden Fremdschlüssel prüfe: `src/app/services/entities/**/[entity]-service.ts`

## Schritt 4: Edit-Komponente für Dropdowns vorbereiten
Plane die TypeScript-Ergänzungen für Fremdschlüssel und Enums.

## Schritt 5: Template-Feldnamen festlegen
Häufige Fallen vermeiden — NUR Properties aus dem Interface verwenden.

## Schritt 6: i18n-Keys planen
Liste aller benötigten Keys: `ENTITYNAME_LIST.*`, `ENTITYNAME_EDIT.*`

## Freigabe
Erst wenn alle Punkte abgehakt sind, Template erstellen.
