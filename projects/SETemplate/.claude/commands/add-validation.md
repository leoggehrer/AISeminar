Erstelle eine Validierungsklasse für eine Entity.

Frage den Benutzer nach: welche Entity und welche Validierungsregeln.

## Konventionen
- Dateiname: `EntityName.Validation.cs`
- Speicherort: gleicher Ordner wie die Entity-Hauptdatei
- Namespace: identisch mit der Entity-Klasse
- Separate `partial`-Klasse, implementiert `IValidatableEntity`
- Keine Validierung für Id-Felder
- `BusinessRuleException` für Geschäftsregeln
- Statische `IsXyzValid()`-Methoden für wiederverwendbare Prüfungen

## Template
```csharp
//@AiCode
namespace SETemplate.Logic.Entities.App
{
    using Microsoft.EntityFrameworkCore;
    using SETemplate.Logic.Modules.Exceptions;

    partial class EntityName : SETemplate.Logic.Contracts.IValidatableEntity
    {
        public void Validate(SETemplate.Logic.Contracts.IContext context, EntityState entityState)
        {
            bool handled = false;
            BeforeExecuteValidation(ref handled, context, entityState);

            if (!handled)
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    throw new BusinessRuleException($"The '{nameof(Name)}' must not be empty.");
                }
            }
        }

        #region methods
        public static bool IsPropertyValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        #endregion methods

        #region partial methods
        partial void BeforeExecuteValidation(ref bool handled,
            SETemplate.Logic.Contracts.IContext context, EntityState entityState);
        #endregion partial methods
    }
}
```

## Nächste Schritte
1. Logic-Projekt bauen und Fehler beheben
2. Code-Generierung ausführen → `/generate`
