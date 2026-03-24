Initialisiere die Datenbank für das aktuelle Entity-Modell (Datenbank wählen, generieren, erstellen).

Arbeite in dieser Reihenfolge und stoppe beim ersten Fehler.

### Schritt 1 – Datenbank auswählen
Frage den Benutzer und führe das passende Kommando aus:

| Datenbank | Kommando |
|-----------|----------|
| **SQLite** (Standard) | `dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x` |
| **PostgreSQL** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x` |
| **MSSQL Server** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x` |

### Schritt 2 – Generierte Klassen aufräumen
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### Schritt 3 – Code neu generieren
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

### Schritt 4 – Datenbank erstellen/migrieren
```bash
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

### Schritt 5 – Optional: CSV-Import
Frage: `Ist ein CSV-Import vorhanden und soll er jetzt ausgeführt werden? (Ja/Nein)`
- Wenn Ja: Prüfe ob CSV-Dateien in `SETemplate.ConApp/data/` und Import-Logik in `StarterApp.Import.cs` vorhanden sind.

### Schritt 6 – Build-Prüfung
```bash
dotnet build SETemplate.sln
```

Melde kompakt: gewählte Datenbank, Ergebnis jedes Schritts, ob DB initialisiert wurde.
