Generiere alle Klassen aus den Entity-Definitionen: EntitySets, DbContext, API-Controller, WebApi-Models, Angular-Components, TypeScript-Services.

## Schritte

### 1. Generierte Klassen löschen (Cleanup)
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### 2. Code-Generierung ausführen
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

### 3. Build-Prüfung
```bash
dotnet build SETemplate.sln
```

Melde kompakt das Ergebnis jedes Schritts.

Hinweise:
- `//@GeneratedCode` Dateien werden bei jedem Durchlauf überschrieben
- `//@CustomCode` und `//@AiCode` bleiben unberührt
- Der Generator liest Entities via Reflection → Logic-Projekt muss kompilierbar sein
