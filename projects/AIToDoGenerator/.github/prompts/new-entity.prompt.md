# Neue Entity erstellen

Erstelle eine neue Entity-Klasse in `SEeAIToDoGenerator.Logic/Entities/` nach den Projektkonventionen.

## Pflichtregeln

- Klassen **IMMER** auf Englisch benennen
- Modifier: `public partial class`
- Vererbung: `: EntityObject`
- `Id`-Property **NICHT** deklarieren (kommt von `EntityObject`)
- Fremdschlüssel immer vom Typ `IdType`
- **IMMER** `DateTime.UtcNow` (niemals `DateTime.Now`)
- XML-Dokumentation für alle Properties
- Navigation Properties **vollqualifiziert** deklarieren
- Berechnete Properties (`[NotMapped]`) **müssen** einen (leeren) Setter haben

## Dateistruktur

| Typ | Pfad |
|-----|------|
| Stammdaten (Kategorien, Lookup) | `SEeAIToDoGenerator.Logic/Entities/Data/EntityName.cs` |
| Anwendungsdaten (Geschäftslogik) | `SEeAIToDoGenerator.Logic/Entities/App/EntityName.cs` |
| Views (ReadOnly) | `SEeAIToDoGenerator.Logic/Entities/Views/EntityName.cs` |

## Entity-Template

```csharp
//@AiCode
namespace SEeAIToDoGenerator.Logic.Entities.App
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

#if SQLITE_ON
    [Table("EntityNames")]
#else
    [Table("EntityNames", Schema = "app")]
#endif
    [Index(nameof(Name), IsUnique = true)]
    public partial class EntityName : EntityObject
    {
        #region fields
        private string _name = string.Empty;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name
        {
            get => _name;
            set
            {
                bool handled = false;
                OnNameChanging(ref handled, ref value);
                if (!handled)
                {
                    _name = value;
                }
                OnNameChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the creation date (UTC).
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        #endregion properties

        #region partial methods
        partial void OnNameChanging(ref bool handled, ref string value);
        partial void OnNameChanged(string value);
        #endregion partial methods
    }
}
```

## Navigation Properties

**1:n (One-to-Many)**
```csharp
// "One"-Seite (z.B. Category):
public List<SEeAIToDoGenerator.Logic.Entities.App.EntityName> EntityNames { get; set; } = [];

// "Many"-Seite (z.B. EntityName) – Fremdschlüssel + Navigation:
public IdType CategoryId { get; set; }
public SEeAIToDoGenerator.Logic.Entities.App.Category? Category { get; set; }
```

**n:m über Zwischentabelle**
```csharp
// Zwischentabelle EntityCross:
public IdType AId { get; set; }
public SEeAIToDoGenerator.Logic.Entities.App.EntityA? A { get; set; }

public IdType BId { get; set; }
public SEeAIToDoGenerator.Logic.Entities.App.EntityB? B { get; set; }
```

## Berechnete Property (NotMapped)

```csharp
/// <summary>
/// Gets a value indicating whether the item is active.
/// </summary>
[NotMapped]
public bool IsActive
{
    get => Status == "Active";
    set { /* Required for serialization */ }
}
```

## Nächste Schritte nach der Entity-Erstellung

1. Logic-Projekt bauen und Fehler beheben
2. Entity-Modell mit Benutzer überprüfen und bestätigen lassen
3. Validierung in `EntityName.Validation.cs` erstellen → Prompt: `add-validation`
4. Code-Generierung ausführen → Prompt: `generate`
