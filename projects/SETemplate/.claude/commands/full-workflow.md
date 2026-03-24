Vollständiger Entwicklungs-Workflow für ein neues Feature/eine neue Entity von Grund auf.

## Vorbereitungsschritte (einmalig pro Projekt)

### Schritt 1 – Datenbank wählen
Frage den Benutzer: SQLite (Standard), PostgreSQL oder MSSQL?
| Datenbank | Kommando |
|-----------|----------|
| SQLite | `dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x` |
| PostgreSQL | `dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x` |
| MSSQL | `dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x` |

### Schritt 2 – Authentifizierung
Frage den Benutzer ob Authentifizierung benötigt wird:
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x
```

### Schritt 3 – CSV-Import klären
Frage ob Initialdaten per CSV geladen werden sollen.

## Entity-Entwicklung

### Schritt 4 – Generierte Klassen löschen
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### Schritt 5 – Entity erstellen
Entity-Klasse in `SETemplate.Logic/Entities/{Data|App}/` anlegen. Siehe `/new-entity`.

### Schritt 6 – Build-Prüfung
```bash
dotnet build SETemplate.Logic/SETemplate.Logic.csproj
```

### Schritt 7 – Modell mit Benutzer überprüfen
Alle Entities und Beziehungen korrekt? Warte auf Bestätigung.

### Schritt 8 – Validierung erstellen
Siehe `/add-validation`. Erneut bauen.

## Code-Generierung

### Schritt 9 – Code generieren
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

## Daten & Datenbank

### Schritt 10 (optional) – CSV-Import einrichten
Siehe `/csv-import`.

### Schritt 11 – Datenbank erstellen
```bash
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

### Schritt 12 (optional) – Rollen konfigurieren
Siehe `/configure-authorization`.

## Abschluss-Checkliste
```
□ Datenbank gewählt
□ Authentifizierung konfiguriert
□ Generierte Klassen gelöscht
□ Entities erstellt
□ Logic-Projekt baut ohne Fehler
□ Entity-Modell bestätigt
□ Validierung erstellt
□ Code-Generierung ausgeführt
□ CSV-Import eingerichtet (falls gewünscht)
□ Datenbank erstellt
```
