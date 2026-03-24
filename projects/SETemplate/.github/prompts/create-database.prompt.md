# Datenbank erstellen und migrieren

Erstellt die Datenbank-Tabellen anhand der Entity-Definitionen und führt ausstehende Migrationen aus.

## Kommando

```bash
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

## Voraussetzungen

- [ ] Entities sind vollständig und compilierbar
- [ ] Validierungsklassen sind erstellt
- [ ] Code-Generierung wurde ausgeführt → Prompt: `generate`
- [ ] Gewünschte Datenbank ist konfiguriert → Prompt: `setup-database`
- [ ] Solution baut ohne Fehler: `dotnet build SETemplate.sln`

## Datenbank-spezifische Verbindungsstrings

Die Verbindungsstrings werden in `SETemplate.ConApp/appsettings.json` konfiguriert:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SETemplate.db"
  }
}
```

Für **PostgreSQL**:
```json
"DefaultConnection": "Host=localhost;Database=SETemplate;Username=postgres;Password=yourpassword"
```

Für **MSSQL**:
```json
"DefaultConnection": "Server=localhost;Database=SETemplate;Trusted_Connection=True;"
```

## Nach der Datenbank-Erstellung

1. CSV-Import starten (falls konfiguriert) → Prompt: `csv-import`
2. WebApi starten und mit Swagger testen: `https://localhost:xxxx/swagger`
