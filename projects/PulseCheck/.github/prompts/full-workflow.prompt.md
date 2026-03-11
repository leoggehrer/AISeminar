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

---

## Entity-Entwicklung

### Schritt 3 – Generierte Klassen löschen

Vor Änderungen an Entities immer zuerst aufräumen:

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### Schritt 4 – Entity erstellen

Entity-Klasse in `SEeAIToDoGenerator.Logic/Entities/{Data|App}/EntityName.cs` anlegen.

→ Details: Prompt `new-entity`

### Schritt 5 – Build-Prüfung

```bash
dotnet build SEeAIToDoGenerator.Logic/SEeAIToDoGenerator.Logic.csproj
```

Alle Fehler beheben, bevor du weitermachst.

### Schritt 6 – Modell mit Benutzer überprüfen

- Alle Entities und Beziehungen korrekt?
- Navigation Properties vollqualifiziert?
- `IdType` für alle Fremdschlüssel?
- **Warte auf Bestätigung des Benutzers.**

### Schritt 7 – Validierung erstellen

Validierungsklasse in `EntityName.Validation.cs` anlegen.

→ Details: Prompt `add-validation`

Erneut bauen und Fehler beheben.

---

## Code-Generierung

### Schritt 8 – Code generieren

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

→ Details: Prompt `generate`

---

## Daten & Datenbank

### Schritt 9 (optional) – CSV-Import einrichten

CSV-Datei und Import-Logik unter `SEeAIToDoGenerator.ConApp/` anlegen.

→ Details: Prompt `csv-import`

### Schritt 10 – Datenbank erstellen

```bash
dotnet run --project SEeAIToDoGenerator.ConApp -- AppArg=1,2,x
```

→ Details: Prompt `create-database`

---

## Autorisierung (nur bei ACCOUNT_ON)

### Schritt 11 (optional) – Rollen konfigurieren

`EntitySet.Custom.cs` mit Rollen-Autorisierung anlegen.

→ Details: Prompt `configure-authorization`

---

## Abschluss-Checkliste

```
□ Datenbank gewählt (SQLite / PostgreSQL / MSSQL)
□ Authentifizierung konfiguriert (ON / OFF)
□ Generierte Klassen gelöscht (AppArg=4,7,x,x)
□ Entities erstellt in Logic/Entities/{Data|App}/
□ Logic-Projekt baut ohne Fehler
□ Entity-Modell mit Benutzer bestätigt
□ Validierung in .Validation.cs erstellt
□ Code-Generierung ausgeführt (AppArg=4,9,x,x)
□ CSV-Dateien erstellt (falls gewünscht)
□ Import-Logik implementiert (falls gewünscht)
□ Datenbank erstellt und migriert (AppArg=1,2,x)
□ Anwendung getestet (Swagger / Angular)
```

## Änderungen an bestehenden Entities

Wenn nachträglich Entities verändert werden:

1. Generierte Klassen löschen: `dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x`
2. Entity anpassen
3. Build-Prüfung
4. Validierung anpassen (falls nötig)
5. Code neu generieren: `dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x`
6. Datenbank migrieren: `dotnet run --project SEeAIToDoGenerator.ConApp -- AppArg=1,2,x`
