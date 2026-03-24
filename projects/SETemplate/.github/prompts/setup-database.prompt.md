# Datenbank konfigurieren

Wähle und aktiviere die gewünschte Datenbank für SETemplate. Standardmäßig ist **SQLite** eingestellt.

## Verfügbare Datenbankoptionen

### PostgreSQL aktivieren

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x
```

### MSSQL Server aktivieren

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x
```

### SQLite aktivieren (Standard)

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x
```

## Prüfung nach der Umstellung

Überprüfe die Projektdatei `SETemplate.ConApp.csproj`, ob die Defines korrekt gesetzt wurden:

```xml
<PropertyGroup>
  <!-- Beispiel für SQLite: -->
  <DefineConstants>ACCOUNT_OFF;POSTGRES_OFF;SQLSERVER_OFF;SQLITE_ON;...</DefineConstants>
</PropertyGroup>
```

Es darf immer nur **ein** Datenbank-Define `_ON` sein.

## Wichtige Hinweise

- **PostgreSQL** erfordert zwingend `DateTime.UtcNow` – niemals `DateTime.Now` verwenden!
- Nach dem Wechsel der Datenbank: Code-Generierung erneut ausführen → Prompt: `generate`
- Beim Wechsel zu einer anderen Datenbank: Datenbank neu erstellen → Prompt: `create-database`
- **Tabellen-Schema:** Bei SQLite wird kein Schema verwendet (`[Table("Name")]`), bei PostgreSQL/MSSQL wird ein Schema verwendet (`[Table("Name", Schema = "app")]`)
