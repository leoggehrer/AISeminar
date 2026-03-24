Wähle und aktiviere die gewünschte Datenbank für SETemplate. Standardmäßig ist SQLite eingestellt.

Frage den Benutzer welche Datenbank gewünscht ist und führe das passende Kommando aus:

### PostgreSQL
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x
```

### MSSQL Server
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x
```

### SQLite (Standard)
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x
```

## Prüfung nach der Umstellung
Überprüfe die Projektdatei `SETemplate.ConApp.csproj`, ob die Defines korrekt gesetzt wurden. Es darf immer nur ein Datenbank-Define `_ON` sein.

Hinweise:
- PostgreSQL erfordert zwingend `DateTime.UtcNow`
- Nach dem Wechsel: Code-Generierung erneut ausführen → `/generate`
- Beim Wechsel: Datenbank neu erstellen → `/create-database`
