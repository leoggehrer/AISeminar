# Cleanup für generierte Klassen

Führe diesen Prompt aus, bevor eine oder mehrere Entities geändert werden.
Ziel ist ein sauberer, fehlerfreier Ausgangszustand ohne Syntaxfehler.

Arbeite strikt in der angegebenen Reihenfolge und stoppe sofort beim ersten Fehler.

## Ziel

- Alte generierte Dateien entfernen
- Sicherstellen, dass keine Syntax- oder Build-Fehler vorhanden sind
- Erst danach Änderungen an Entities vornehmen

## Schritte

### 1. Generierte Klassen löschen (Cleanup)

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### 2. Lösung bauen

```bash
dotnet build SETemplate.sln
```

### 3. Erst danach Entities ändern

Beginne Entity-Änderungen nur, wenn Schritt 2 erfolgreich war.

## Abschluss-Reporting

Melde nach der Ausführung kompakt:

- Ergebnis von Schritt 1 (Cleanup)
- Ergebnis von Schritt 2 (Solution-Build)
- Freigabe für Entity-Änderungen: Ja/Nein
- Falls Fehler auftraten: erster Fehler mit Datei/Projekt und nächster sinnvoller Fix-Schritt

## Erfolgskriterien

- Cleanup ohne Fehler abgeschlossen
- Solution-Build erfolgreich
- Entity-Änderungen sind erst jetzt freigegeben

## Hinweise

- `//@GeneratedCode` Dateien werden bei jedem Lauf überschrieben
- `//@CustomCode` und `//@AiCode` Dateien bleiben erhalten
- Vor Änderungen in `SETemplate.Logic/Entities/` ausführen
- Der Generator arbeitet via Reflection, daher muss die Solution vorab kompilierbar sein
- Keine manuellen Änderungen in generierten `//@GeneratedCode` Bereichen vornehmen
