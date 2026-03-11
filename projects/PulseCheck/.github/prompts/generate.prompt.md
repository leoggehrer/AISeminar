# Code-Generierung ausführen

Generiere alle notwendigen Klassen aus den Entity-Definitionen: EntitySets, DbContext, API-Controller, WebApi-Models, Angular-Components, TypeScript-Services, etc.

## Schritte

### 1. Generierte Klassen löschen (Cleanup)

Vor der Neu-Generierung zuerst alle bestehenden generierten Klassen entfernen:

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### 2. Code-Generierung ausführen

Nach dem Cleanup die Code-Generierung starten:

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

### 3. Build-Prüfung

Sicherstellen, dass der generierte Code fehlerfrei kompiliert:

```bash
dotnet build SEeAIToDoGenerator.sln
```

## Hinweise

- `//@GeneratedCode` Dateien werden bei jedem Durchlauf überschrieben
- `//@CustomCode` und `//@AiCode` Dateien bleiben unberührt
- Nur nach Änderungen an **Entities** in `SEeAIToDoGenerator.Logic/Entities/` notwendig
- Der Generator liest Entities via **Reflection** → Logic-Projekt muss kompilierbar sein
