# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with the **Backend** of this repository.

> For Angular frontend instructions, see `SETemplate.AngularApp/CLAUDE.md` (used when opening the AngularApp folder in a separate VSCode session).

## Project Overview

SETemplate is a **template-based code generation system** for .NET 8.0/Angular 19 applications using Clean Architecture. The core is a code generation engine that automatically creates CRUD operations, API controllers, services, and UI components from entity definitions.

**Language note:** Documentation and instructions are in German; **all code must be in English** — this includes class names, method names, property names, variable names, enum values, XML comments, and all other identifiers.

### Core Concepts

- **Entity-First**: Entities in `SETemplate.Logic/Entities/` are the single source of truth
- **Code-Markers**: `//@GeneratedCode` vs `//@CustomCode` vs `//@AiCode` control overwrite behavior
- **Conditional Compilation**: Defines like `ACCOUNT_ON`, `SQLITE_ON` control features globally
- **Partial Classes**: Allow extending generated classes without overwriting

### Fundamental Rules

- **ALWAYS** ask user for: database choice (PostgreSQL/MSSQL/SQLite), authentication (yes/no), CSV import (yes/no)
- **ALWAYS** provide XML documentation for all public members
- **ALWAYS** use code generation for CRUD operations
- **NEVER** manually create controllers, services, or CRUD operations — they are generated and will be overwritten
- **NEVER** change visibility of existing classes (e.g., context classes must remain `internal`)
- **NEVER** create navigation properties to `Identity` (Identity class is internal!)

## Essential Commands

```bash
# Build the solution
dotnet build

# Code generation (generates controllers, services, models, Angular components)
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x

# Delete generated classes (MUST run before entity changes)
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x

# Create/migrate database
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x

# Toggle authentication on/off
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x

# Switch database engine
dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x   # PostgreSQL
dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x   # MSSQL
dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x  # SQLite (default)
```

## Architecture

```
TemplateTools.Logic/ConApp  →  Code Generation Engine (DO NOT EDIT)
        ↓ generates into
SETemplate.Common           →  Shared contracts, enums, extensions
SETemplate.Logic            →  Entities, DbContext, EntitySets, business logic
SETemplate.WebApi           →  ASP.NET Core REST controllers
SETemplate.ConApp           →  CLI for DB init, CSV import
SETemplate.MVVMApp          →  Desktop app (Avalonia)
SETemplate.AngularApp       →  Angular 19 SPA frontend (separate VSCode session)
SETemplate.McpSrv           →  MCP server for AI integration
```

The generator uses **Reflection** to find all entities in `SETemplate.Logic/Entities/`. For each entity it auto-generates: EntitySets, DbContext entries, API controllers, TypeScript models, services, and Angular components. Configuration is controlled via `CodeGeneration.csv`.

## Code Marker System (Critical)

| Marker | Meaning | Editable | Overwritten by generator |
|--------|---------|----------|--------------------------|
| `//@AiCode` | AI-created code | Yes | No |
| `//@GeneratedCode` | Auto-generated | No | Yes |
| `//@CustomCode` | Protected customization | Yes | No |
| `#if GENERATEDCODE_ON` | Conditional compilation for features | Yes | No |

Files with `//@GeneratedCode` are overwritten on each generation. To customize generated code, change the marker to `//@CustomCode`.

## Conditional Compilation Defines

Defined in `.csproj` files and kept in sync across all projects:

```xml
<DefineConstants>ACCOUNT_OFF;SQLITE_ON;POSTGRES_OFF;SQLSERVER_OFF;DEVELOP_ON;GENERATEDCODE_OFF</DefineConstants>
```

- `ACCOUNT_ON`/`ACCOUNT_OFF` — authentication toggle
- `SQLITE_ON`/`POSTGRES_ON`/`SQLSERVER_ON` — database selection
- `GENERATEDCODE_ON`/`GENERATEDCODE_OFF` — feature gating for generated code
- `DEVELOP_ON` — development mode

When `ACCOUNT_ON` is active, all API endpoints are protected. Access is controlled via roles. Authorization is configured via a partial class `EntitySet<TEntity>` in namespace `SETemplate.Logic.DataContext`.

## Entity Development Rules

### File Structure

| Type | Path | Usage |
|------|------|-------|
| **Master data** | `SETemplate.Logic/Entities/Data/` | Basic data (categories, etc.) |
| **Application data** | `SETemplate.Logic/Entities/App/` | Business logic data |
| **Account** | `SETemplate.Logic/Entities/Account/` | User management |
| **Views** | `SETemplate.Logic/Entities/Views/` | Database views (ReadOnly) |

### File Naming Convention

- **Main file:** `EntityName.cs`
- **Validation:** `EntityName.Validation.cs` (same namespace)
- **Custom Logic:** `EntityName.Custom.cs` (optional)

### Basic Rules

- Classes must be `public partial` and inherit from `EntityObject` (or `ViewObject` for views)
- Namespaces: `SETemplate.Logic.Entities[.SubFolder]`
- Enums go in `SETemplate.Common/Enums/` in separate files

### Property Rules

| Property | Rule | Example |
|----------|------|---------|
| **Primary key** | Inherited from `EntityObject` | `Id` (do not declare) |
| **Foreign keys** | Type `IdType` | `public IdType CategoryId { get; set; }` |
| **Strings (Required)** | Initialize with `string.Empty` | `= string.Empty` |
| **Strings (Optional)** | Nullable, no initialization | `string?` |
| **DateTime** | **ALWAYS use UTC** | `DateTime.UtcNow` (never `DateTime.Now` — PostgreSQL requires UTC!) |
| **DateTime parsing** | Specify UTC kind | `DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)` |
| **Length** | Declare with attribute | `[MaxLength(100)]` |
| **Unique** | Index attribute on class | `[Index(nameof(Name), IsUnique = true)]` |
| **Auto-Property** | When no logic needed | `public string Name { get; set; } = string.Empty;` |
| **Full-Property** | With logic/events | See template below |

### Computed Properties (Important)

Computed properties (`[NotMapped]`) **must** have a setter for WebApi serialization to work:

```csharp
[NotMapped]
public bool IsPassed
{
    get => Grade <= 4.0;
    set { /* Required for serialization */ }
}
```

### Table Attribute Rules

```csharp
#if SQLITE_ON
    [Table("EntityNames")]
#else
    [Table("EntityNames", Schema = "app")]
#endif
```

### Entity Template (Complete Example)

```csharp
//@AiCode
namespace SETemplate.Logic.Entities.App
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
        private int _categoryId;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the name of the entity.
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
        /// Gets or sets the description of the entity.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        public IdType CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                _category = null;
            }
        }
        #endregion properties

        #region navigation properties
        private Category? _category;

        /// <summary>
        /// Gets or sets the associated category.
        /// </summary>
        public Category? Category
        {
            get => _category;
            set
            {
                _category = value;
                if (value != null)
                {
                    _categoryId = value.Id;
                }
            }
        }
        #endregion navigation properties

        #region partial methods
        partial void OnNameChanging(ref bool handled, ref string value);
        partial void OnNameChanged(string value);
        #endregion partial methods
    }
}
```

### Navigation Properties (Important)

- Navigation properties **always** fully qualified on first use, e.g.:
  `public SETemplate.Logic.Entities.App.Category? Category { get; set; }`
- **No navigation properties for Identity** (Identity class is internal!)
- In the Many-side: declare foreign key `EntityNameId`

**1:n Relationship (One-to-Many)**
```csharp
// In the "One" side (e.g. Category):
public List<EntityName> EntityNames { get; set; } = [];

// In the "Many" side (e.g. EntityName):
public IdType CategoryId { get; set; }
public Category? Category { get; set; }
```

**n:1 or 1:1 Relationship**
```csharp
public IdType ParentEntityId { get; set; }
public SETemplate.Logic.Entities.App.ParentEntity? ParentEntity { get; set; }
```

**n:m Relationship (Many-to-Many) — via junction table**
```csharp
// StudentCourse (junction table)
public IdType StudentId { get; set; }
public Student? Student { get; set; }
public IdType CourseId { get; set; }
public Course? Course { get; set; }

// In Student:
public List<StudentCourse> StudentCourses { get; set; } = [];

// In Course:
public List<StudentCourse> StudentCourses { get; set; } = [];
```

### Using Rules

```csharp
// Do NOT use (Global Usings):
using System;

// Only specific usings:
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
```

## Validation Classes

Separate partial class in the **same namespace** as the entity, implementing `IValidatableEntity`.
Filename: `EntityName.Validation.cs`. No validation for Id fields.

```csharp
//@AiCode
namespace SETemplate.Logic.Entities[.SubFolder]
{
    using SETemplate.Logic.Modules.Exceptions;

    partial class EntityName : SETemplate.Logic.Contracts.IValidatableEntity
    {
        public void Validate(SETemplate.Logic.Contracts.IContext context, EntityState entityState)
        {
            bool handled = false;
            BeforeExecuteValidation(ref handled, context, entityState);

            if (!handled)
            {
                if (!IsPropertyNameValid(PropertyName))
                {
                    throw new BusinessRuleException(
                        $"The value of {nameof(PropertyName)} '{PropertyName}' is not valid.");
                }
            }
        }

        #region methods
        public static bool IsPropertyNameValid(PropertyType value)
        {
            return true;
        }
        #endregion methods

        #region partial methods
        partial void BeforeExecuteValidation(ref bool handled, SETemplate.Logic.Contracts.IContext context, EntityState entityState);
        #endregion partial methods
    }
}
```

## Views (Database Views)

```csharp
//@AiCode
namespace SETemplate.Logic.Entities.Views[.SubFolder]
{
    using System.ComponentModel.DataAnnotations;
    using CommonModules.Attributes;

    [View("ViewNames")]
    public partial class ViewName : ViewObject
    {
        #region properties
        public string PropertyName { get; set; } = string.Empty;
        #endregion properties
    }
}
```

Views are ReadOnly and do not support navigation properties.

## IdentityId Management in EntitySets (Critical)

**IdentityId is ALWAYS set automatically in the Logic layer, NEVER manually in imports or controllers!**

When an entity has an `IdentityId` property, create `EntityNameSet.Custom.cs`:

```csharp
//@AiCode
#if GENERATEDCODE_ON && ACCOUNT_ON

namespace SETemplate.Logic.DataContext.[SubFolder]
{
    using TEntity = Entities.[SubFolder].EntityName;

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
            {
                entity.IdentityId = identityId.Value;
            }
            return base.BeforePersistingAddAsync(entity);
        }

        protected override Task BeforePersistingUpdateAsync(TEntity entity)
        {
            var identityId = GetCurrentIdentityId();
            if (identityId.HasValue)
            {
                entity.IdentityId = identityId.Value;
            }
            return base.BeforePersistingUpdateAsync(entity);
        }
    }
}
#endif
```

## Modules Folder Structure

For business logic that does **not belong directly to an entity**, use `SETemplate.Logic/Modules/`:

| Type | Path | Usage |
|------|------|-------|
| **API Clients** | `SETemplate.Logic/Modules/ApiClients/` | REST API calls to external systems |
| **Services** | `SETemplate.Logic/Modules/Services/` | Business logic services |
| **Helpers** | `SETemplate.Logic/Modules/Helpers/` | Utility classes |
| **Exceptions** | `SETemplate.Logic/Modules/Exceptions/` | Custom exceptions (already exists) |
| **Extensions** | `SETemplate.Logic/Modules/Extensions/` | Extension methods |
| **Validators** | `SETemplate.Logic/Modules/Validators/` | Complex validation logic |

**Rules:** Use `appsettings.json` for configuration. Never hardcode URLs or API keys. Use async/await for I/O operations. Use dependency injection.

## CSV Import System

- Namespace: `SETemplate.ConApp.Apps` (unchangeable)
- File: `StarterApp.Import.cs` (partial method)
- CSV files: `ConApp/data/entityname_set.csv` (semicolon-delimited, `#` for comments)
- Error handling: `try/catch` with `RejectChangesAsync()` per line
- All imports: `async/await`

```csv
#Name;Description
Developer;Software Developer
Manager;Project Manager
```

## Project Editability

| Project | Editable |
|---------|----------|
| `TemplateTools.*` | Never edit |
| `SETemplate.Logic/Entities/` | Create/edit entities here |
| `SETemplate.Logic/Modules/` | Custom business logic, services, helpers |
| `SETemplate.Logic/DataContext/` | Custom EntitySet extensions (`.Custom.cs`) |
| `SETemplate.ConApp` | Import logic, DB setup |
| `SETemplate.Common` | Shared helpers |
| Generated files (`//@GeneratedCode`) | Do not edit directly |

## Development Workflow

1. Ask user for: database choice, authentication needs, CSV import needs
2. Delete generated classes: `AppArg=4,7,x,x`
3. Create/modify entities in `Logic/Entities/{Data|App}/`
4. Build Logic project to verify: `dotnet build SETemplate.Logic`
5. Review entity model with user
6. Create validation files (`*.Validation.cs`)
7. Run code generation: `AppArg=4,9,x,x`
8. Create CSV data files in `ConApp/data/` and import logic in `StarterApp.Import.cs` (if needed)
9. Create database: `dotnet run --project SETemplate.ConApp -- AppArg=1,2,x`

When changing entities: always delete generated classes first (step 2), then restart from step 3.

For step-by-step workflows see `.github/prompts/` (cleanup, new-entity, generate, csv-import, etc.)

## Troubleshooting

| Problem | Cause | Solution |
|---------|-------|---------|
| Build error after entity change | Generated code is outdated | Run code generation |
| Import error | CSV format or relationships | Check CSV file and foreign keys |
| PostgreSQL DateTime error | `DateTime.Now` used | Always use `DateTime.UtcNow` |
| Controller not found | Not generated | Run code generation |

### Checklist for Issues

1. Is the Logic project compilable without errors?
2. Was code generation run after entity changes?
3. Are all foreign keys correctly defined (type `IdType`)?
4. Do all DateTime properties use `DateTime.UtcNow`?
5. Do computed properties (`[NotMapped]`) have a setter?
6. Is the correct database configured?
