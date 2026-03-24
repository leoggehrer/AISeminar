Erstelle die Datenbank-Tabellen anhand der Entity-Definitionen.

## Voraussetzungen prüfen
- Entities sind vollständig und compilierbar
- Code-Generierung wurde ausgeführt (`/generate`)
- Gewünschte Datenbank ist konfiguriert (`/setup-database`)
- Solution baut ohne Fehler

## Kommando
```bash
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

## Verbindungsstrings (in SETemplate.ConApp/appsettings.json)
- **SQLite:** `"Data Source=SETemplate.db"`
- **PostgreSQL:** `"Host=localhost;Database=SETemplate;Username=postgres;Password=yourpassword"`
- **MSSQL:** `"Server=localhost;Database=SETemplate;Trusted_Connection=True;"`

## Nach der Datenbank-Erstellung
1. CSV-Import starten (falls konfiguriert) → `/csv-import`
2. WebApi starten und mit Swagger testen
