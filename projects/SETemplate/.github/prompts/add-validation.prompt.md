# Validierungsklasse zu einer Entity hinzufügen

Erstelle eine separate Validierungsklasse als `partial`-Klasse im gleichen Namespace wie die Entity.

## Dateikonvention

- **Dateiname:** `EntityName.Validation.cs`
- **Speicherort:** Gleicher Ordner wie die Entity-Hauptdatei
- **Namespace:** Identisch mit der Entity-Klasse

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
                // Pflichtfeld-Validierung
                if (string.IsNullOrWhiteSpace(Name))
                {
                    throw new BusinessRuleException($"The '{nameof(Name)}' must not be empty.");
                }

                // Wertebereich-Validierung
                if (!IsStatusValid(Status))
                {
                    throw new BusinessRuleException(
                        $"The value of '{nameof(Status)}' ('{Status}') is not valid.");
                }
            }
        }

        #region methods
        /// <summary>
        /// Validates the status value.
        /// </summary>
        public static bool IsStatusValid(string value)
        {
            return value is "Active" or "Inactive" or "Pending";
        }
        #endregion methods

        #region partial methods
        partial void BeforeExecuteValidation(ref bool handled,
            SETemplate.Logic.Contracts.IContext context, EntityState entityState);
        #endregion partial methods
    }
}
```

## Validierungsregeln

- ❌ **Keine Validierung** für Id-Felder (werden von der Datenbank verwaltet)
- ✅ Verwende `BusinessRuleException` für Geschäftsregeln
- ✅ Async-Pattern mit `RejectChangesAsync()` in der Import-Logik
- ✅ Statische `IsXyzValid()`-Methoden für wiederverwendbare Prüfungen
- ✅ `BeforeExecuteValidation` Partial-Method für externe Erweiterbarkeit

## Nächste Schritte

1. Logic-Projekt bauen und Fehler beheben
2. Code-Generierung ausführen → Prompt: `generate`
