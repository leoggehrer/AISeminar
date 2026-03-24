Konfiguriere rollen-basierte Autorisierung: welche Rollen auf welche EntitySets und Operationen (Read/Create/Update/Delete) zugreifen dürfen. Nur relevant wenn `ACCOUNT_ON` aktiv ist.

## Datei erstellen
**Dateiname:** `SETemplate.Logic/DataContext/EntitySet.Custom.cs`

```csharp
//@AiCode
#if GENERATEDCODE_ON && ACCOUNT_ON

namespace SETemplate.Logic.DataContext;

using SETemplate.Logic.DataContext.App;

partial class EntitySet<TEntity>
{
    static partial void ClassConstructed()
    {
        var appAdminAuthorize = new Modules.Security.AuthorizeAttribute("SysAdmin", "AppAdmin");
        var allUsersAuthorize = new Modules.Security.AuthorizeAttribute();
        var readersAuthorize = appAdminAuthorize.Clone("Manager", "User");
        var writersAuthorize = appAdminAuthorize.Clone("Manager");

        // Globaler Standard
        SetAuthorization(typeof(EntitySet<TEntity>), appAdminAuthorize);

        // Spezifische Konfiguration pro EntitySet
        // SetAuthorization(typeof(SomeEntitySet), appAdminAuthorize);
        // SetAuthorization4Read(typeof(SomeEntitySet), allUsersAuthorize);
    }
}
#endif
```

## Verfügbare Methoden
| Methode | Beschreibung |
|---------|-------------|
| `SetAuthorization(type, attr)` | Standard für alle Operationen |
| `SetAuthorization4Read(type, attr)` | Nur Lesezugriff |
| `SetAuthorization4Create(type, attr)` | Nur Erstellen |
| `SetAuthorization4Update(type, attr)` | Nur Aktualisieren |
| `SetAuthorization4Delete(type, attr)` | Nur Löschen |

## IdentityId automatisch setzen
Wenn eine Entity eine `IdentityId`-Property hat, erstelle zusätzlich `EntityNameSet.Custom.cs` — siehe CLAUDE.md Abschnitt "IdentityId Management".

Frage den Benutzer welche Entities welche Rollen-Konfiguration benötigen.
