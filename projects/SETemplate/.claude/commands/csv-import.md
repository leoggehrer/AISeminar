Implementiere CSV-Import-Logik für eine oder mehrere Entities, damit Stamm-/Testdaten aus CSV-Dateien geladen werden können.

Frage den Benutzer: welche Entities sollen importiert werden, und welche Daten.

## Schritte

### 1. CSV-Datei erstellen
Lege die Datei unter `SETemplate.ConApp/data/entityname_set.csv` an:
```csv
#Name;Description
First Entry;Description for first entry
Second Entry;Description for second entry
```
- Erste Zeile: Spaltenüberschriften (mit `#` auskommentiert)
- Semikolon-getrennt, `#`-Zeilen werden ignoriert

### 2. CSV in csproj eintragen
In `SETemplate.ConApp.csproj`:
```xml
<ItemGroup>
  <Content Include="data\entityname_set.csv">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### 3. Import-Logik in StarterApp.Import.cs implementieren
Datei: `SETemplate.ConApp/Apps/StarterApp.Import.cs`
- Namespace: `SETemplate.ConApp.Apps` (unveränderlich)
- Fehlerbehandlung: `try/catch` mit `RejectChangesAsync()` pro Zeile
- Alle Imports: `async/await`
- Import nur im `DEBUG && DEVELOP_ON` Modus aktiv
- IdentityId NIEMALS manuell im Import setzen

Verwende das Import-Template aus der CLAUDE.md/copilot-instructions.

## Nächste Schritte
1. Datenbank erstellen → `/create-database`
2. SETemplate.ConApp ausführen und Import-Menüpunkt starten
