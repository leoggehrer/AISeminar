# Authentifizierung umschalten

Aktiviere oder deaktiviere die Token-basierte Authentifizierung für alle API-Endpunkte.

## Kommando zum Umschalten

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x
```

Dieser Befehl schaltet zwischen `ACCOUNT_ON` und `ACCOUNT_OFF` um.

## Defines

| Define | Bedeutung |
|--------|-----------|
| `ACCOUNT_ON` | Alle API-Endpunkte sind geschützt, JWT-Token erforderlich |
| `ACCOUNT_OFF` | Keine Authentifizierung (Standard) |

## Wenn Authentifizierung aktiviert (`ACCOUNT_ON`)

### Benutzer anlegen (in `StarterApp.cs`)

```csharp
static partial void CreateUserAccounts()
{
    Task.Run(async () =>
    {
        var accounts = new (string UserName, string Email, string Password, int Timeout, string[] Roles)[]
        {
            // Name, Email, Password, Session-Timeout (Minuten), Rollen
            new ("Admin User", "admin@example.com", "Passme1234", 60, new[] { "SysAdmin" }),
            new ("App Admin",  "appadmin@example.com", "Passme1234", 30, new[] { "AppAdmin" }),
            new ("Standard User", "user@example.com", "Passme1234", 30, new[] { "User" }),
        };

        foreach (var account in accounts)
        {
            await AddAppAccessAsync(SAEmail, SAPwd, account.UserName, account.Email,
                account.Password, account.Timeout, account.Roles);
        }
    }).Wait();
}
```

### Rollen-basierte Autorisierung konfigurieren

Erstelle `SEeAIToDoGenerator.Logic/DataContext/EntitySet.Custom.cs` → Prompt: `configure-authorization`

## Nächste Schritte nach dem Umschalten

1. Code-Generierung ausführen → Prompt: `generate`
2. Datenbank neu erstellen → Prompt: `create-database`
