# Vollständiger Entwicklungs-Workflow

Folge dieser Schritt-für-Schritt-Anleitung beim Anlegen eines neuen Features/einer neuen Entity von Grund auf.

## Vorbereitungsschritte (einmalig pro Projekt)

### Schritt 1 – Datenbank wählen

Frage den Benutzer und führe das passende Kommando aus:

| Datenbank | Kommando |
|-----------|----------|
| **SQLite** (Standard) | `dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x` |
| **PostgreSQL** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x` |
| **MSSQL Server** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x` |

→ Details: Prompt `setup-database`

### Schritt 2 – Authentifizierung einstellen

Frage den Benutzer, ob Authentifizierung benötigt wird:

```bash
# Umschalten (ON ↔ OFF):
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x
```

→ Details: Prompt `toggle-auth`

### Schritt 3 – CSV-Import klären

Frage den Benutzer, ob Initialdaten per CSV-Import geladen werden sollen.

- **Ja:** CSV-Dateien und Import-Logik werden nach der Code-Generierung eingerichtet → Prompt `csv-import`
- **Nein:** Schritt überspringen

---

## Entity-Entwicklung

### Schritt 4 – Generierte Klassen löschen

Vor Änderungen an Entities immer zuerst aufräumen:

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### Schritt 5 – Entity erstellen

Entity-Klasse in `SETemplate.Logic/Entities/{Data|App}/EntityName.cs` anlegen.

→ Details: Prompt `new-entity`

### Schritt 6 – Build-Prüfung

```bash
dotnet build SETemplate.Logic/SETemplate.Logic.csproj
```

Alle Fehler beheben, bevor du weitermachst.

### Schritt 7 – Modell mit Benutzer überprüfen

- Alle Entities und Beziehungen korrekt?
- Navigation Properties vollqualifiziert?
- `IdType` für alle Fremdschlüssel?
- **Warte auf Bestätigung des Benutzers.**

### Schritt 8 – Validierung erstellen

Validierungsklasse in `EntityName.Validation.cs` anlegen.

→ Details: Prompt `add-validation`

Erneut bauen und Fehler beheben.

---

## Code-Generierung

### Schritt 9 – Code generieren

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

→ Details: Prompt `generate`

---

## Daten & Datenbank

### Schritt 10 (optional) – CSV-Import einrichten

CSV-Datei und Import-Logik unter `SETemplate.ConApp/` anlegen.

→ Details: Prompt `csv-import`

### Schritt 11 – Datenbank erstellen

```bash
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

→ Details: Prompt `create-database`

---

## Autorisierung (nur bei ACCOUNT_ON)

### Schritt 12 (optional) – Rollen konfigurieren

`EntitySet.Custom.cs` mit Rollen-Autorisierung anlegen.

→ Details: Prompt `configure-authorization`

---

## Angular Frontend

### Schritt 13 – Vor-Template-Checkliste (für jede Entity)

Vor dem Erstellen eines Templates das Model-Interface prüfen:
- Alle Properties und Typen notieren
- Fremdschlüssel → Dropdown nötig
- Enums → Dropdown mit Enum-Werten nötig

→ Details: `.github/prompts/pre-template-checklist.prompt.md` (im AngularApp-Ordner)

### Schritt 14 – List-Komponenten erstellen (alle Entities)

HTML-Templates für alle generierten List-Komponenten in `src/app/pages/entities/` erstellen.

→ Details: `.github/prompts/create-list-component.prompt.md`

### Schritt 15 – Edit-Formulare erstellen (alle Entities)

HTML-Templates und TypeScript-Erweiterungen für alle generierten Edit-Komponenten in `src/app/components/entities/` erstellen.

→ Details: `.github/prompts/create-edit-component.prompt.md`

### Schritt 16 – Routing konfigurieren

Alle List-Komponenten in `app-routing.module.ts` eintragen.

→ Details: `.github/prompts/configure-routing.prompt.md`

### Schritt 17 – Dashboard-Cards hinzufügen

Für jede Entity eine Card im Dashboard anlegen.

→ Details: `.github/prompts/add-dashboard-cards.prompt.md`

### Schritt 18 – i18n-Übersetzungen ergänzen

Alle Labels in `de.json` und `en.json` gleichzeitig eintragen.

→ Details: `.github/prompts/add-i18n-translations.prompt.md`

### Schritt 19 – Build & Test

```bash
cd SETemplate.AngularApp && npm run build
```

Alle TypeScript-Fehler beheben. Häufige Fehler → `.github/prompts/fix-common-errors.prompt.md`

---

## Abschluss-Checkliste

```
□ Datenbank gewählt (SQLite / PostgreSQL / MSSQL)
□ Authentifizierung konfiguriert (ON / OFF)
□ CSV-Import geklärt (Ja / Nein)
□ Generierte Klassen gelöscht (AppArg=4,7,x,x)
□ Entities erstellt in Logic/Entities/{Data|App}/
□ Logic-Projekt baut ohne Fehler
□ Entity-Modell mit Benutzer bestätigt
□ Validierung in .Validation.cs erstellt
□ Code-Generierung ausgeführt (AppArg=4,9,x,x)
□ CSV-Dateien erstellt (falls gewünscht)
□ Import-Logik implementiert (falls gewünscht)
□ Datenbank erstellt und migriert (AppArg=1,2,x)
□ Angular: Interfaces geprüft (pre-template-checklist)
□ Angular: List-Komponenten HTML erstellt
□ Angular: Edit-Formulare HTML + TypeScript erstellt
□ Angular: Routing konfiguriert
□ Angular: Dashboard-Cards hinzugefügt
□ Angular: i18n DE + EN ergänzt
□ Angular: npm run build ohne Fehler
```

## Änderungen an bestehenden Entities

Wenn nachträglich Entities verändert werden:

1. Generierte Klassen löschen: `dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x`
2. Entity anpassen
3. Build-Prüfung
4. Validierung anpassen (falls nötig)
5. Code neu generieren: `dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x`
6. Datenbank migrieren: `dotnet run --project SETemplate.ConApp -- AppArg=1,2,x`
7. Angular: geänderte Komponenten anpassen und `npm run build` prüfen
