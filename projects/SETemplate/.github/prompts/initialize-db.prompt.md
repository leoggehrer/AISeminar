# Datenbank initialisieren

Initialisiert die Datenbank für das aktuelle Entity-Modell (inkl. vorheriger Konfiguration und Validierung).

Arbeite in der angegebenen Reihenfolge und stoppe beim ersten Fehler.

## Ziel

- Gewünschte Datenbank aktivieren
- Aktuellen Code-Stand generieren
- Datenbank erstellen/migrieren
- Optional vorhandenen CSV-Import ausführen (nach Rückfrage)
- Build-Erfolg als Abschlussprüfung sicherstellen

## Schritte

### 1. Datenbank auswählen und aktivieren

Wähle genau **eine** Option:

**PostgreSQL**
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x
```

**MSSQL Server**
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x
```

**SQLite**
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x
```

### 2. Generierte Klassen aufräumen

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### 3. Code neu generieren

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

### 4. Datenbank erstellen/migrieren

```bash
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

### 5. Optional: CSV-Import (vorher fragen)

Frage vor der Ausführung explizit:

`Ist ein CSV-Import vorhanden und soll er jetzt ausgeführt werden? (Ja/Nein)`

- Wenn **Nein**: Schritt 6 ausführen.
- Wenn **Ja**:
	1. Prüfe, ob CSV-Dateien im Ordner `SETemplate.ConApp/data/` vorhanden sind.
	2. Prüfe, ob Import-Logik in `SETemplate.ConApp/Apps/StarterApp.Import.cs` vorhanden ist.
	3. Starte die ConApp und führe den Import-Menüpunkt aus.

```bash
dotnet run --project SETemplate.ConApp
```

### 6. Build-Prüfung

```bash
dotnet build SETemplate.sln
```

## Abschluss-Reporting

Melde kompakt:

- Gewählte Datenbank (PostgreSQL/MSSQL/SQLite)
- Ergebnis Schritt 2 (Cleanup)
- Ergebnis Schritt 3 (Generierung)
- Ergebnis Schritt 4 (DB-Erstellung/Migration)
- Ergebnis Schritt 5 (CSV-Import): Ausgeführt/Übersprungen + Ergebnis
- Ergebnis Schritt 6 (Solution-Build)
- Datenbank initialisiert: Ja/Nein
- Falls Fehler: erster Fehler (Projekt/Datei) und nächster sinnvoller Fix-Schritt

## Hinweise

- `//@GeneratedCode` wird bei jeder Generierung überschrieben
- `//@CustomCode` und `//@AiCode` bleiben erhalten
- Verbindungsstrings in `SETemplate.ConApp/appsettings.json` prüfen
- Für PostgreSQL immer UTC-Datumswerte verwenden (`DateTime.UtcNow`)
- Import nur ausführen, wenn CSV und Import-Logik vorhanden sind
