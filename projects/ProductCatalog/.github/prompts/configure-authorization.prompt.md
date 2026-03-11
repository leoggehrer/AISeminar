# Rollen-basierte Autorisierung konfigurieren

Konfiguriert, welche Rollen auf welche EntitySets und Operationen (Read/Create/Update/Delete) zugreifen dürfen. Nur relevant wenn `ACCOUNT_ON` aktiv ist.

## Datei erstellen

**Dateiname:** `SEeAIToDoGenerator.Logic/DataContext/EntitySet.Custom.cs`

```csharp
//@AiCode
#if GENERATEDCODE_ON && ACCOUNT_ON

namespace SEeAIToDoGenerator.Logic.DataContext;

using SEeAIToDoGenerator.Logic.DataContext.App;

partial class EntitySet<TEntity>
{
    static partial void ClassConstructed()
    {
        // Standard: Nur SysAdmin und AppAdmin dürfen alles
        var appAdminAuthorize = new Modules.Security.AuthorizeAttribute("SysAdmin", "AppAdmin");

        // Alle authentifizierten Benutzer (keine Rolleneinschränkung)
        var allUsersAuthorize = new Modules.Security.AuthorizeAttribute();

        // Benutzergruppen definieren
        var readersAuthorize  = appAdminAuthorize.Clone("Manager", "User");
        var writersAuthorize  = appAdminAuthorize.Clone("Manager");

        // --- Globaler Standard für alle EntitySets ---
        SetAuthorization(typeof(EntitySet<TEntity>), appAdminAuthorize);

        // --- Spezifische Konfiguration pro EntitySet ---

        // Beispiel 1: Alle lesen, nur Admins schreiben
        SetAuthorization(typeof(SomeEntitySet), appAdminAuthorize);
        SetAuthorization4Read(typeof(SomeEntitySet), allUsersAuthorize);

        // Beispiel 2: Verschiedene Rollen für CRUD
        SetAuthorization(typeof(AnotherEntitySet), appAdminAuthorize);
        SetAuthorization4Read(typeof(AnotherEntitySet),   readersAuthorize);
        SetAuthorization4Create(typeof(AnotherEntitySet), writersAuthorize);
        SetAuthorization4Update(typeof(AnotherEntitySet), writersAuthorize);
    }
}
#endif
```

## Verfügbare Autorisierungs-Methoden

| Methode | Beschreibung |
|---------|-------------|
| `SetAuthorization(type, attr)` | Standard für alle Operationen des EntitySets |
| `SetAuthorization4Read(type, attr)` | Nur Lesezugriff |
| `SetAuthorization4Create(type, attr)` | Nur Erstellen |
| `SetAuthorization4Update(type, attr)` | Nur Aktualisieren |
| `SetAuthorization4Delete(type, attr)` | Nur Löschen |

## AuthorizeAttribute erstellen

```csharp
// Mehrere Rollen – eine davon ist ausreichend (OR-Logik):
var attr = new Modules.Security.AuthorizeAttribute("SysAdmin", "AppAdmin");

// Aus bestehender Instanz klonen und Rollen ergänzen:
var extended = attr.Clone("Manager", "User");

// Alle authentifizierten Benutzer (keine Rolleneinschränkung):
var allUsers = new Modules.Security.AuthorizeAttribute();
```

## IdentityId automatisch setzen (für User-spezifische Entities)

Wenn eine Entity eine `IdentityId`-Property hat, erstelle zusätzlich:

**Datei:** `SEeAIToDoGenerator.Logic/DataContext/App/EntityNameSet.Custom.cs`

```csharp
//@AiCode
#if GENERATEDCODE_ON && ACCOUNT_ON

namespace SEeAIToDoGenerator.Logic.DataContext.App
{
    using TEntity = Entities.App.EntityName;

    partial class EntityNameSet
    {
        private IdType? GetCurrentIdentityId()
        {
            if (!string.IsNullOrEmpty(SessionToken))
            {
                var session = Modules.Account.AccountManager.QueryLoginSession(SessionToken);
                return session?.IdentityId;
            }
            return null;
        }

        protected override Task BeforePersistingAddAsync(TEntity entity)
        {
            var identityId = GetCurrentIdentityId();
            if (identityId.HasValue)
                entity.IdentityId = identityId.Value;
            return base.BeforePersistingAddAsync(entity);
        }

        protected override Task BeforePersistingUpdateAsync(TEntity entity)
        {
            var identityId = GetCurrentIdentityId();
            if (identityId.HasValue)
                entity.IdentityId = identityId.Value;
            return base.BeforePersistingUpdateAsync(entity);
        }
    }
}
#endif
```

## Hinweise

- ❌ **NIEMALS** `IdentityId` manuell im Import oder Controller setzen
- Authentifizierung muss aktiviert sein → Prompt: `toggle-auth`
- Nach Änderungen: Code-Generierung ausführen → Prompt: `generate`
