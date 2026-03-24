Ändere eine oder mehrere bestehende Entities in `SETemplate.Logic/Entities/`.

Arbeite strikt in dieser Reihenfolge und stoppe beim ersten Fehler.

## Phase 1 – Vorbereitung (PFLICHT)

### Schritt 1 – Generierte Klassen löschen
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### Schritt 2 – Build nach Cleanup
```bash
dotnet build SETemplate.sln
```
Bei Fehlern: erst beheben, bevor Entities geändert werden.

## Phase 2 – Entity-Änderungen durchführen
Frage den Benutzer was geändert werden soll. Beachte:
- Fremdschlüssel immer `IdType`, DateTime immer `DateTime.UtcNow`
- Required-Strings mit `string.Empty` initialisieren, optionale als `string?`
- XML-Dokumentation für alle neuen Properties, `[MaxLength(...)]` für Strings
- Navigation Properties vollqualifiziert deklarieren
- FK und Navigation Property immer gemeinsam definieren/entfernen
- `[NotMapped]` Properties brauchen immer einen (leeren) Setter

## Phase 3 – Build-Prüfung
```bash
dotnet build SETemplate.Logic/SETemplate.Logic.csproj
```
Änderungen mit dem Benutzer überprüfen. Warte auf Bestätigung.

## Phase 4 – Validierung anpassen (falls nötig)
Passe `EntityName.Validation.cs` an → Details siehe `/add-validation`

## Phase 5 – Code-Generierung
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
dotnet build SETemplate.sln
```

Melde am Ende kompakt das Ergebnis jeder Phase.
