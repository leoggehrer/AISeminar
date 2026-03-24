# Beispiel: ToDoList-App entwickeln

> Dieses Beispiel zeigt, wie man mit SETemplate eine App mit mehreren Entities entwickelt.

---

## Schritt 1 – Datenmodell planen (ZUERST!)

Bevor eine einzige Entity angelegt wird, das gesamte Modell mit der AI entwerfen und **bestätigen lassen**.

**Einstiegssatz an die AI:**
> "Ich möchte eine ToDoList-App entwickeln. Folge dem Workflow aus `full-workflow.prompt.md`. Beginne mit dem Entwurf des gesamten Datenmodells und warte auf meine Bestätigung, bevor du Code schreibst."

**Ergebnis (Beispiel-Modell):**

```
Category        – Stammdaten (Dringlichkeit, Typ, etc.)
Tag             – Stammdaten (Labels)
ToDoList        – Eine Liste mit Titel und Beschreibung
ToDoItem        – Aufgabe, gehört zu ToDoList + Category
ToDoItemTag     – Zwischentabelle (n:m zwischen ToDoItem und Tag)
```

**Beziehungen:**
- `ToDoList` → `ToDoItem`: 1:n
- `ToDoItem` → `Category`: n:1
- `ToDoItem` ↔ `Tag`: n:m über `ToDoItemTag`

Erst nach **Bestätigung des Modells** wird mit dem Code begonnen.

---

## Schritt 2 – Cleanup (einmalig)

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
dotnet build SETemplate.sln
```

→ Details: `.github/prompts/cleanup.prompt.md`

---

## Schritt 3 – Alle Entities anlegen

**Reihenfolge: Stammdaten zuerst, dann abhängige Entities!**

| Reihenfolge | Entity | Abhängigkeiten | Pfad |
|-------------|--------|----------------|------|
| 1 | `Category` | – | `Entities/Data/Category.cs` |
| 2 | `Tag` | – | `Entities/Data/Tag.cs` |
| 3 | `ToDoList` | – | `Entities/App/ToDoList.cs` |
| 4 | `ToDoItem` | `ToDoList`, `Category` | `Entities/App/ToDoItem.cs` |
| 5 | `ToDoItemTag` | `ToDoItem`, `Tag` | `Entities/App/ToDoItemTag.cs` |

**Beispiel `ToDoList.cs`:**
```csharp
//@AiCode
namespace SETemplate.Logic.Entities.App
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#if SQLITE_ON
    [Table("ToDoLists")]
#else
    [Table("ToDoLists", Schema = "app")]
#endif
    public partial class ToDoList : EntityObject
    {
        /// <summary>Gets or sets the title of the list.</summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the description.</summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>Gets or sets the creation date.</summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>Gets or sets the items of this list.</summary>
        public List<ToDoItem> ToDoItems { get; set; } = [];
    }
}
```

**Beispiel `ToDoItem.cs`:**
```csharp
//@AiCode
namespace SETemplate.Logic.Entities.App
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#if SQLITE_ON
    [Table("ToDoItems")]
#else
    [Table("ToDoItems", Schema = "app")]
#endif
    public partial class ToDoItem : EntityObject
    {
        /// <summary>Gets or sets the title.</summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets whether the item is done.</summary>
        public bool IsDone { get; set; }

        /// <summary>Gets or sets the due date.</summary>
        public DateTime? DueDate { get; set; }

        /// <summary>Gets or sets the ToDoList identifier.</summary>
        public IdType ToDoListId { get; set; }

        /// <summary>Gets or sets the Category identifier.</summary>
        public IdType CategoryId { get; set; }

        // Navigation Properties
        /// <summary>Gets or sets the parent list.</summary>
        public SETemplate.Logic.Entities.App.ToDoList? ToDoList { get; set; }

        /// <summary>Gets or sets the category.</summary>
        public SETemplate.Logic.Entities.Data.Category? Category { get; set; }

        /// <summary>Gets or sets the tag assignments.</summary>
        public List<ToDoItemTag> ToDoItemTags { get; set; } = [];
    }
}
```

**Beispiel `ToDoItemTag.cs` (n:m Zwischentabelle):**
```csharp
//@AiCode
namespace SETemplate.Logic.Entities.App
{
    using System.ComponentModel.DataAnnotations.Schema;

#if SQLITE_ON
    [Table("ToDoItemTags")]
#else
    [Table("ToDoItemTags", Schema = "app")]
#endif
    public partial class ToDoItemTag : EntityObject
    {
        /// <summary>Gets or sets the ToDoItem identifier.</summary>
        public IdType ToDoItemId { get; set; }

        /// <summary>Gets or sets the Tag identifier.</summary>
        public IdType TagId { get; set; }

        // Navigation Properties
        /// <summary>Gets or sets the todo item.</summary>
        public SETemplate.Logic.Entities.App.ToDoItem? ToDoItem { get; set; }

        /// <summary>Gets or sets the tag.</summary>
        public SETemplate.Logic.Entities.Data.Tag? Tag { get; set; }
    }
}
```

→ Details: `.github/prompts/new-entity.prompt.md`

---

## Schritt 4 – Build prüfen

```bash
dotnet build SETemplate.Logic/SETemplate.Logic.csproj
```

Alle Fehler beheben, bevor es weitergeht.

---

## Schritt 5 – Validierungen erstellen

Für jede Entity eine `EntityName.Validation.cs` anlegen.

→ Details: `.github/prompts/add-validation.prompt.md`

---

## Schritt 6 – Code generieren (einmalig für alle Entities)

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

Der Generator verarbeitet automatisch **alle** Entities in einem Lauf.

→ Details: `.github/prompts/generate.prompt.md`

---

## Schritt 7 – Datenbank erstellen

```bash
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

→ Details: `.github/prompts/create-database.prompt.md`

---

## Schritt 8 – CSV-Import (optional)

CSV-Dateien für Stammdaten anlegen, z.B.:

**`SETemplate.ConApp/data/category_set.csv`**
```csv
#Name;Description
Work;Work-related tasks
Personal;Personal tasks
Urgent;High priority tasks
```

**`SETemplate.ConApp/data/tag_set.csv`**
```csv
#Name
Important
Quick
Blocked
```

→ Details: `.github/prompts/csv-import.prompt.md`
