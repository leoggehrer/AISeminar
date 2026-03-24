Führe einen Cleanup der generierten Klassen durch, bevor Entities geändert werden.

Arbeite strikt in dieser Reihenfolge und stoppe beim ersten Fehler:

### Schritt 1 – Generierte Klassen löschen
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### Schritt 2 – Solution bauen
```bash
dotnet build SETemplate.sln
```

### Schritt 3 – Ergebnis melden
Melde kompakt:
- Ergebnis Cleanup
- Ergebnis Build
- Freigabe für Entity-Änderungen: Ja/Nein
- Falls Fehler: erster Fehler mit Datei/Projekt und nächster Fix-Schritt

Hinweise:
- `//@GeneratedCode` Dateien werden überschrieben
- `//@CustomCode` und `//@AiCode` bleiben erhalten
- Vor Änderungen in `SETemplate.Logic/Entities/` ausführen
