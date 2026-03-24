Aktiviere oder deaktiviere die Token-basierte Authentifizierung für alle API-Endpunkte.

## Kommando zum Umschalten
```bash
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x
```

Schaltet zwischen `ACCOUNT_ON` und `ACCOUNT_OFF` um.

## Wenn Authentifizierung aktiviert (ACCOUNT_ON)

### Benutzer anlegen (in StarterApp.cs)
```csharp
static partial void CreateUserAccounts()
{
    Task.Run(async () =>
    {
        var accounts = new (string UserName, string Email, string Password, int Timeout, string[] Roles)[]
        {
            new ("Admin User", "admin@example.com", "Passme1234", 60, new[] { "SysAdmin" }),
            new ("App Admin", "appadmin@example.com", "Passme1234", 30, new[] { "AppAdmin" }),
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

## Nächste Schritte
1. Code-Generierung ausführen → `/generate`
2. Datenbank neu erstellen → `/create-database`
3. Rollen konfigurieren → `/configure-authorization`
