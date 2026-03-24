Kopiere das SETemplate in ein neues Projektverzeichnis und öffne es in einer neuen VS Code Instanz.

Frage den Benutzer nach:
| Parameter | Beschreibung | Beispiel |
|-----------|-------------|---------|
| **Zielverzeichnis** | Übergeordneter Ordner | `C:\Users\name\source\repos\MyCompany` |
| **Projektname** | Name des neuen Projekts | `MyNewProject` |

## Schritt 1 – Template kopieren
```bash
dotnet run --project TemplateTools.ConApp -- TargetSolutionSubPath=<ZIELVERZEICHNIS> TargetSolutionName=<PROJEKTNAME> AppArg=copier,start,exit,exit
```

> Schlägt fehl wenn Zielverzeichnis existiert. Für Überschreiben: `Force=True` hinzufügen.

## Schritt 2 – Neues Projekt in VS Code öffnen
```bash
code --new-window <ZIELVERZEICHNIS>/<PROJEKTNAME>
```

Hinweise:
- Kopiertes Projekt ist ein vollständiges SETemplate mit allen Projekten
- Alle Dateien mit `//@BaseCode`-Marker werden exakt kopiert
- Nach dem Öffnen empfiehlt sich `/full-workflow`
- TemplateTools.ConApp muss im Quell-Workspace ausgeführt werden
