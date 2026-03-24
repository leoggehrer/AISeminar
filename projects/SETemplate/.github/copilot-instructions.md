# GitHub Copilot Instructions für SETemplate

> **Version:** 3.0 | **Letzte Aktualisierung:** März 2026

---

## Projektübersicht

SETemplate ist ein **Template-basiertes Code-Generierungssystem** für .NET/Angular Anwendungen mit Clean Architecture. Der Kern ist ein ausgeklügeltes Code-Generierungssystem, das CRUD-Operationen, API-Controller, Services und UI-Komponenten automatisch aus Entity-Definitionen erstellt.

### Architektur-Übersicht

```
┌─────────────────────────────────────────────────────────────────┐
│                    Code Generation Layer                        │
│  TemplateTools.Logic + TemplateTools.ConApp                     │
│  - Liest Entity-Definitionen via Reflection                     │
│  - Generiert Code basierend auf CodeGeneration.csv              │
│  - Schreibt in alle Projektebenen                               │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌───────────────────────────────────────────────────────────────-─┐
│                      Backend Projects                           │
│  SETemplate.Logic (Entities, EntitySets, DbContext)             │
│  SETemplate.WebApi (Controllers)                                │
│  SETemplate.ConApp (CLI, Data Import)                           │
│  SETemplate.MVVMApp (Desktop UI)                                │
└──────────────────────────────────────────────────────────────-──┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                     Frontend Project                            │
│  SETemplate.AngularApp (Components, Services, Models)           │
└─────────────────────────────────────────────────────────────────┘
```

**Kernkonzepte:**
- **Entity-First**: Entities (in `SETemplate.Logic/Entities/`) sind die einzige Quelle der Wahrheit
- **Code-Marker**: `//@GeneratedCode` vs `//@CustomCode` vs `//@AiCode` steuern Überschreibverhalten
- **Conditional Compilation**: Defines wie `ACCOUNT_ON`, `SQLITE_ON` steuern Features global
- **Partielle Klassen**: Erlauben Erweiterung generierter Klassen ohne Überschreiben

> Für Schritt-für-Schritt-Workflows siehe `.github/prompts/` (cleanup, new-entity, generate, csv-import, etc.)

### Code-Marker (KRITISCH!)
- `//@AiCode` - Von der AI Generierter Code
- `//@GeneratedCode` - **NICHT BEARBEITEN!** Wird bei Generierung überschrieben
- `//@CustomCode` - Manuell angepasster Code, wird **nicht** überschrieben
- `#if GENERATEDCODE_ON` - Conditional Compilation für Features

### Grundregeln
✅ **IMMER:** Benutzer nach Datenbank-Wahl fragen (PostgreSQL/MSSQL/SQLite)
✅ **IMMER:** Benutzer nach Authentifizierung fragen (Ja/Nein)
✅ **IMMER:** Benutzer nach csv-Import fragen (Ja/Nein)
✅ **IMMER:** XML-Dokumentation für alle öffentlichen Member
✅ **IMMER:** Code-Generierung verwenden  
❌ **NIEMALS:** Controllers, Services oder CRUD-Operationen manuell erstellen  
❌ **NIEMALS:** Ändere nicht die Sichtbarkeit der bestehenden Klassen (z.B.: die Kontext-Klasse muss 'internal' bleiben)  
❌ **NIEMALS:** Navigation Properties vom Typ `Identity` erstellen (Identity-Klasse ist internal!)  

---

## Projektstruktur und Verantwortlichkeiten

| Projekt | Zweck | Editierbar |
|---------|-------|------------|
| **TemplateTools.Logic** | Code-Generator Engine | ❌ Nicht editieren |
| **TemplateTools.ConApp** | CLI für Generator-Steuerung | ❌ Nicht editieren |
| **SETemplate.Logic** | Entities, DbContext, EntitySets | ✅ Entities erstellen |
| **SETemplate.WebApi** | REST API Controller | 🔄 Generiert |
| **SETemplate.ConApp** | CLI, DB-Migration, CSV-Import | ✅ Import-Logik |
| **SETemplate.MVVMApp** | Desktop App (Avalonia) | 🔄 Generiert |
| **SETemplate.AngularApp** | SPA Frontend | 🔄 Generiert |
| **SETemplate.Common** | Shared Utilities | ✅ Helpers |

**Legende:** ✅ = Manuell bearbeiten, 🔄 = Generiert (nur `//@CustomCode`), ❌ = Nicht editieren

---

## Kernprinzipien

### 1. Code-Generierung First (KRITISCH!)

**⚠️ NIEMALS manuell Controllers, Services oder CRUD-Operationen erstellen!**

Alle CRUD-Operationen, EntitySets, Controllers und Services werden automatisch generiert. Manuelle Erstellung führt zu Inkonsistenzen und wird bei der nächsten Generierung überschrieben.

### 2. Code-Marker System (KRITISCH!)

Das Code-Marker System steuert, welcher Code vom Generator verwaltet wird:

| Marker | Bedeutung | Editierbar | Überschrieben |
|--------|-----------|------------|---------------|
| `//@AiCode` | Manuell für AI erstellt | ✅ Ja | ❌ Nein |
| `//@GeneratedCode` | Automatisch generiert | ❌ Nein | ✅ Ja |
| `//@CustomCode` | Geschützte Anpassung | ✅ Ja | ❌ Nein |
| `#if GENERATEDCODE_ON` | Conditional Feature | ✅ Ja | ❌ Nein |

**Wichtig:** Änderungen in `//@GeneratedCode` Dateien führen automatisch zur Umwandlung in `//@CustomCode` und verhindern weitere Updates durch den Generator.

### 3. Authentifizierung und Autorisierung

**Standard:** Authentifizierung ist deaktiviert. Frage den Benutzer vor dem Start, ob sie benötigt wird.

| Define        | Bedeutung |
|---------------|-----------|
| `ACCOUNT_ON`  | Authentifizierung eingeschaltet |
| `ACCOUNT_OFF` | Authentifizierung ausgeschaltet |

Wenn `ACCOUNT_ON` aktiv ist, werden alle API-Endpunkte geschützt. Der Zugriff wird über Rollen gesteuert. Autorisierung wird über eine partielle Klasse `EntitySet<TEntity>` im Namespace `SETemplate.Logic.DataContext` konfiguriert (siehe `configure-authorization.prompt.md`).

**IdentityId-Regel (KRITISCH):** Die `IdentityId` wird **IMMER** automatisch in der Logic-Schicht gesetzt (via `EntityNameSet.Custom.cs`), **NIEMALS** manuell im Import oder in Controllers!

---

## Wie Code-Generierung funktioniert

Der Generator nutzt **Reflection**, um alle Entities in `SETemplate.Logic/Entities/` zu finden. Für jede Entity werden dann automatisch erstellt:

1. **Backend**: EntitySets, DbContext-Einträge, API-Controller
2. **Frontend**: TypeScript Models, Services, List/Edit-Komponenten
3. **Konfiguration**: Gesteuert durch `CodeGeneration.csv`

**Wichtig:** Der Generator überschreibt nur Dateien mit `//@GeneratedCode`-Marker. Dateien mit `//@CustomCode` bleiben unberührt.

**Globale Feature-Steuerung über Defines:**
```xml
<DefineConstants>ACCOUNT_OFF;SQLITE_ON;POSTGRES_OFF;SQLSERVER_OFF;DEVELOP_ON;GENERATEDCODE_OFF</DefineConstants>
```

Diese Defines werden in **allen** Projekt-Dateien synchron gehalten und steuern Features wie Authentifizierung, Datenbanktyp und ID-Typ global.

---

## Datenbank auswählen

Standardmäßig ist SQLite eingestellt. Die Auswahl erfolgt via `setup-database.prompt.md`. Die aktive Datenbank wird über Defines in den `.csproj`-Dateien gesteuert:

```xml
<DefineConstants>ACCOUNT_OFF;SQLITE_ON;POSTGRES_OFF;SQLSERVER_OFF;DEVELOP_ON;GENERATEDCODE_OFF</DefineConstants>
```

## Entity-Entwicklung

### Grundregeln (KRITISCH!)

1. **Sprache:** Der gesamte Code wird **IMMER auf Englisch** implementiert – Klassen-, Methoden-, Property- und Variablennamen, XML-Kommentare, Enum-Werte und alle sonstigen Bezeichner
2. **Benennung:** Alle Entities **IMMER** auf Englisch
3. **Modifier:** Klassen sind `public` und `partial`
4. **Vererbung:** Klassen erben von `EntityObject`
5. **Dokumentation:** XML-Kommentare für alle Properties (Englisch)
6. **Namespaces:** `SETemplate.Logic.Entities[.SubFolder]` als Basis
7. **Enums:** Enums werden in eigenen Dateien und im `SETemplate.Common.Enums[.SubFolder]` abgelegt

### Dateistruktur

| Typ | Pfad | Verwendung |
|-----|------|------------|
| **Stammdaten** | `SETemplate.Logic/Entities/Data/` | Grundlegende Daten (Kategorien, etc.) |
| **Anwendungsdaten** | `SETemplate.Logic/Entities/App/` | Geschäftslogik-Daten |
| **Account** | `SETemplate.Logic/Entities/Account/` | Benutzerverwaltung |
| **Views** | `SETemplate.Logic/Entities/Views/` | Datenbankviews (ReadOnly) |

### Entity-Dateikonvention

- **Hauptdatei:** `EntityName.cs`
- **Validierung:** `EntityName.Validation.cs` (gleicher Namespace)
- **Custom Logic:** `EntityName.Custom.cs` (optional)  

### Entity Template (Vollständiges Beispiel)

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
                _category = null; // Navigation Property zurücksetzen
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

### Property-Regeln (WICHTIG!)

| Eigenschaft | Regel | Beispiel |
|-------------|-------|----------|
| **Primärschlüssel** | Von `EntityObject` geerbt | `Id` (nicht deklarieren) |
| **Fremdschlüssel** | Typ `IdType` | `public IdType CategoryId { get; set; }` |
| **Strings (Required)** | Mit `string.Empty` initialisieren | `= string.Empty` |
| **Strings (Optional)** | Nullable, keine Initialisierung | `string?` |
| **DateTime** | **IMMER UTC verwenden** | `DateTime.UtcNow` statt `DateTime.Now` |
| **Länge** | Mit Attribut deklarieren | `[MaxLength(100)]` |
| **Unique** | Index-Attribut auf Klasse | `[Index(nameof(Name), IsUnique = true)]` |
| **Auto-Property** | Wenn keine Logik nötig | `public string Name { get; set; } = string.Empty;` |
| **Full-Property** | Mit Logik/Events | Siehe Template oben |
| **Berechnete Properties** | `[NotMapped]` mit Getter UND Setter | Siehe unten |

**WICHTIG für DateTime:**
- ✅ **IMMER:** `DateTime.UtcNow` verwenden
- ✅ **IMMER:** Beim Parsen `DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)` verwenden
- ❌ **NIEMALS:** `DateTime.Now` verwenden (PostgreSQL erfordert UTC!)
- ❌ **NIEMALS:** DateTime mit `DateTimeKind.Unspecified` in Datenbank schreiben

### Berechnete Properties (WICHTIG!)

Berechnete Properties (`[NotMapped]`) **müssen** einen Setter haben, damit die Serialisierung für die WebApi funktioniert. Der Setter kann leer sein:

```csharp
/// <summary>
/// Gets a value indicating whether the exam was passed.
/// </summary>
[NotMapped]
public bool IsPassed 
{ 
    get => Grade <= 4.0; 
    set { /* Required for serialization */ } 
}
```

**Warum?** Der Code-Generator erstellt WebApi Models mit allen Properties. Ohne Setter kann das Model nicht deserialisiert werden und die berechneten Werte werden nicht korrekt an das Frontend übertragen.

### Navigation Properties (WICHTIG!)

**Navigationproperty Regeln:** 
- Navigation Properties **immer** vollqualifiziert deklarieren, z.B.: 
  `public SETemplate.Logic.Entities.App.Category? Category { get; set; }`
- In der Many-Seite: Fremdschlüssel `EntityNameId` deklarieren
- Für 1:n Beziehungen:  
  ```csharp
  public List<Type> EntityNames { get; set; } = [];
  ```
- Für 1:1 / n:1 Beziehungen:  
  ```csharp
  Type? EntityName { get; set; }
  ```
- Für n:m Beziehungen: Zwischentabelle verwenden
- Es gibt keine Navigation Properties für die Identity **WICHTIG**

#### Beziehungsarten

**1:n Beziehung (One-to-Many)**
```csharp
// In der "One"-Seite (z.B. Category):
/// <summary>
/// Gets or sets the list of entities in this category.
/// </summary>
public List<EntityName> EntityNames { get; set; } = [];

// In der "Many"-Seite (z.B. EntityName):
/// <summary>
/// Gets or sets the category identifier.
/// </summary>
public IdType CategoryId { get; set; }

/// <summary>
/// Gets or sets the associated category.
/// </summary>
public Category? Category { get; set; }
```

**n:1 oder 1:1 Beziehung**
```csharp
// Fremdschlüssel
public IdType ParentEntityId { get; set; }

// Navigation Property (immer vollqualifiziert bei erster Verwendung)
/// <summary>
/// Gets or sets the parent entity.
/// </summary>
public SETemplate.Logic.Entities.App.ParentEntity? ParentEntity { get; set; }
```

**n:m Beziehung (Many-to-Many) - über Zwischentabelle**
```csharp
// StudentCourse (Zwischentabelle)
public IdType StudentId { get; set; }
public Student? Student { get; set; }

public IdType CourseId { get; set; }
public Course? Course { get; set; }

// In Student:
public List<StudentCourse> StudentCourses { get; set; } = [];

// In Course:
public List<StudentCourse> StudentCourses { get; set; } = [];
```

### Using-Regeln

```csharp
// ❌ NICHT verwenden (Global Usings):
using System;

// ✅ Nur spezifische Usings:
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
```## Struktur für Validierungsklassen

- Lege eine separate *partial* Klasse für die Validierung im **gleichen Namespace** wie die Entität an.  
- Die Klasse implementiert `IValidatableEntity`.  
- Dateiname: **EntityName.Validation.cs**.  
- Erkennbare Validierungsregeln aus der Beschreibung müssen implementiert werden.

```csharp
//@AiCode
namespace SETemplate.Logic.Entities[.SubFolder]
{
    using System.... // Required usings
    using SETemplate.Logic.Modules.Exceptions;

    partial class EntityName : SETemplate.Logic.Contracts.IValidatableEntity 
    {
        public void Validate(SETemplate.Logic.Contracts.IContext context, EntityState entityState)
        {
            bool handled = false;
            BeforeExecuteValidation(ref handled, context, entityState);

            if (!handled)
            {
                // Implement validation logic here
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
            // Implement validation logic here
            return true;
        }
        #endregion methods

        #region partial methods
        partial void BeforeExecuteValidation(ref bool handled, SETemplate.Logic.Contracts.IContext context, EntityState entityState);
        #endregion partial methods
    }
}
```

## Validierungsregeln

- Keine Validierungen für Id-Felder (werden von der Datenbank verwaltet).

---

## Struktur für Views (Datenbank-Views)

Für ReadOnly-Ansichten aus der Datenbank:

```csharp
//@AiCode
namespace SETemplate.Logic.Entities.Views[.SubFolder]
{
    using System.ComponentModel.DataAnnotations;
    using CommonModules.Attributes;

    /// <summary>
    /// Represents a database view for [ViewName].
    /// </summary>
    [View("ViewNames")]
    public partial class ViewName : ViewObject 
    {
        #region properties
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;
        #endregion properties
    }
}
```

**Hinweis:** Views sind ReadOnly und unterstützen keine Navigation Properties.

---

## CSV-Import System

Für die vollständige Import-Implementierung siehe `csv-import.prompt.md`.

**Konventionen:**
- Namespace: `SETemplate.ConApp.Apps` (unveränderlich)
- Datei: `StarterApp.Import.cs` (partial method)
- CSV-Dateien: `ConApp/data/entityname_set.csv` (Semikolon-getrennt, `#` für Kommentare)
- Fehlerbehandlung: `try/catch` mit `RejectChangesAsync()` pro Zeile
- Alle Imports: `async/await`
- CSV-Pfad: **`AppContext.BaseDirectory`** verwenden (nicht `SolutionPath`!)
- Import nur im `#if DEBUG && DEVELOP_ON` Modus aktiv
- CSV-Datei muss in `.csproj` als `CopyToOutputDirectory` eingetragen werden

### CSV Format
```csv
#Name;Description
Developer;Software Developer
Manager;Project Manager
```

### .csproj Eintrag (KRITISCH!)

```xml
<ItemGroup>
  <Content Include="data\entityname_set.csv">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### StarterApp.Import.cs Template (KRITISCH!)

```csharp
//@AiCode
#if GENERATEDCODE_ON

namespace SETemplate.ConApp.Apps
{
    partial class StarterApp
    {
        partial void CreateImportMenuItems(ref int menuIdx, List<MenuItem> menuItems)
        {
            menuItems.Add(new()
            {
                Key = "----",
                Text = new string('-', 65),
                Action = (self) => { },
                ForegroundColor = ConsoleColor.DarkGreen,
            });

            menuItems.Add(new()
            {
                Key = $"{++menuIdx}",
                Text = ToLabelText($"{nameof(ImportData).ToCamelCaseSplit()}", "Started the import of the csv-data"),
                Action = (self) =>
                {
#if DEBUG && DEVELOP_ON
                    ImportData();
#endif
                },
#if DEBUG && DEVELOP_ON
                ForegroundColor = ConsoleApplication.ForegroundColor,
#else
                ForegroundColor = ConsoleColor.Red,
#endif
            });
        }

        private static void ImportData()
        {
            Task.Run(async () =>
            {
                try
                {
                    await ImportDataAsync();
                }
                catch (Exception ex)
                {
                    var saveColor = ForegroundColor;
                    PrintLine();
                    ForegroundColor = ConsoleColor.Red;
                    PrintLine($"Error during data import: {ex.Message}");
                    ForegroundColor = saveColor;
                }
            }).Wait();
        }

        private static async Task ImportDataAsync()
        {
            Logic.Contracts.IContext context = CreateContext();
            var filePath = Path.Combine(AppContext.BaseDirectory, "data", "entityname_set.csv");

            foreach (var line in File.ReadLines(filePath).Skip(1).Where(l => !l.StartsWith('#')))
            {
                var parts = line.Split(';');
                var entity = new Logic.Entities.App.EntityName
                {
                    Name = parts[0].Trim(),
                    Description = parts.Length > 1 ? parts[1].Trim() : null,
                };
                try
                {
                    await context.EntityNameSet.AddAsync(entity);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    await context.RejectChangesAsync();
                    Console.WriteLine($"Error importing line '{line}': {ex.Message}");
                }
            }
        }
    }
}
#endif
```

**Wichtige Hinweise:**
- `context.EntityNameSet.AddAsync(entity)` — korrekte API (NICHT `InsertAsync`, `GetAsync` etc.)
- `context.EntityNameSet` — Property-Name = `{EntityName}Set` (generierter Name!)
- `AppContext.BaseDirectory` für CSV-Pfad verwenden
- `IdentityId` **NIEMALS** manuell im Import setzen

---

## Backend-Logik-Erweiterungen

### Automatische IdentityId-Verwaltung in EntitySets (KRITISCH!)

**WICHTIG:** Die IdentityId wird **IMMER** automatisch in der Logic-Schicht gesetzt, **NIEMALS** manuell im Import oder in Controllers!

#### Regel für EntitySets mit IdentityId

Wenn eine Entity eine `IdentityId` Property hat, erstelle eine Custom-Datei `EntityNameSet.Custom.cs` wenn nicht bereits vorhanden, und implementiere die Logik zum automatischen Setzen der `IdentityId` aus dem aktuellen Session-Token:

**Dateiname:** `SETemplate.Logic/DataContext/[SubFolder]/EntityNameSet.Custom.cs`

```csharp
//@AiCode
#if GENERATEDCODE_ON && ACCOUNT_ON

namespace SETemplate.Logic.DataContext.[SubFolder]
{
    using TEntity = Entities.[SubFolder].EntityName;

    /// <summary>
    /// Custom logic for EntityNameSet.
    /// </summary>
    partial class EntityNameSet
    {
        /// <summary>
        /// Gets the current user's IdentityId from the session token.
        /// </summary>
        /// <returns>The IdentityId of the current user, or null if not logged in.</returns>
        private IdType? GetCurrentIdentityId()
        {
            if (!string.IsNullOrEmpty(SessionToken))
            {
                var session = Modules.Account.AccountManager.QueryLoginSession(SessionToken);
                return session?.IdentityId;
            }
            return null;
        }

        /// <summary>
        /// Called before adding a new EntityName entity.
        /// Sets the IdentityId to the current session identity.
        /// </summary>
        protected override Task BeforePersistingAddAsync(TEntity entity)
        {
            var identityId = GetCurrentIdentityId();

            if (identityId.HasValue)
            {
                entity.IdentityId = identityId.Value;
            }
            return base.BeforePersistingAddAsync(entity);
        }

        /// <summary>
        /// Called before updating an EntityName entity.
        /// Sets the IdentityId to the current session identity.
        /// </summary>
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

**Best Practices:**
- ✅ SessionToken wird vom Context bereitgestellt
- ✅ IdentityId wird aus der aktuellen Session gelesen
- ✅ Gilt für Add UND Update Operationen
- ❌ NIEMALS IdentityId manuell im Import setzen
- ❌ NIEMALS IdentityId im Controller setzen

### Modules-Ordnerstruktur (WICHTIG!)

Wenn zusätzliche Geschäftslogik benötigt wird, die **nicht direkt zu einer Entity gehört**, erstelle diese im `Modules`-Ordner mit einem entsprechenden Unterordner.

**Grundregel:** Alle Logik-Erweiterungen (z.B. externe API-Aufrufe, Services, Helper-Klassen) gehören in `SETemplate.Logic/Modules/`.

**Wichtig:** Wenn externe Services oder API-Clients benötigt werden, **niemals** diese direkt in die Entity-Klassen einfügen. Verwende stattdessen den `Modules`-Ordner. Verwende die appSettings.json für Konfigurationen. Schreibe **niemals** URLs oder API-Schlüssel hart in den Code.

### Ordnerstruktur für Module

| Typ | Pfad | Verwendung |
|-----|------|------------|
| **API-Clients** | `SETemplate.Logic/Modules/ApiClients/` | REST-API Aufrufe zu Fremdsystemen |
| **Services**    | `SETemplate.Logic/Modules/Services/`   | Geschäftslogik-Services |
| **Helpers**     | `SETemplate.Logic/Modules/Helpers/`    | Utility-Klassen und Hilfsmethoden |
| **Exceptions**  | `SETemplate.Logic/Modules/Exceptions/` | Custom Exceptions (bereits vorhanden) |
| **Extensions**  | `SETemplate.Logic/Modules/Extensions/` | Extension Methods |
| **Validators**  | `SETemplate.Logic/Modules/Validators/` | Komplexe Validierungslogik |

### Best Practices für Module

✅ **DO:**
- Klare, beschreibende Namespace-Struktur verwenden
- Klassen als `partial` deklarieren für Erweiterbarkeit
- XML-Dokumentation für alle öffentlichen Member
- Async/Await Pattern für I/O-Operationen
- Dependency Injection verwenden (HttpClient, IContext, etc.)
- Partial Methods für Erweiterungspunkte bereitstellen

❌ **DON'T:**
- Keine Entity-CRUD-Operationen in Modules (gehört in generierte Controller)
- Keine Datenbankzugriffe ohne IContext
- Keine hartcodierten Konfigurationswerte (verwende appsettings.json)
- Keine synchronen Blockierungen bei async-Operationen

---

## Angular Komponenten

Die spezifischen Anweisungen für die Erstellung und Strukturierung der Angular-Komponenten befinden sich im Ordner SETemplate.AngularApp/.github/copilot-instructions.md.

## Entwicklungs-Workflow

**WICHTIG: Zu Beginn stelle dich kurz vor und erkläre, dass du diese Anweisungen befolgst, um konsistenten und qualitativ hochwertigen Code zu gewährleisten.**

Verwende die Prompt-Dateien in `.github/prompts/` für die schrittweise Ausführung:

| Aufgabe | Prompt |
|---------|--------|
| Cleanup vor Entity-Änderungen | `cleanup.prompt.md` |
| Datenbank wählen | `setup-database.prompt.md` |
| Authentifizierung umschalten | `toggle-auth.prompt.md` |
| Neue Entity erstellen | `new-entity.prompt.md` |
| Validierung hinzufügen | `add-validation.prompt.md` |
| Autorisierung konfigurieren | `configure-authorization.prompt.md` |
| Code generieren | `generate.prompt.md` |
| CSV-Import implementieren | `csv-import.prompt.md` |
| Datenbank erstellen | `create-database.prompt.md` |
| Vollständiger Workflow | `full-workflow.prompt.md` |

## Konventionen

### Naming
- **Sprache:** Sämtlicher Code (Klassen, Methoden, Properties, Variablen, Enums, Kommentare) wird **IMMER auf Englisch** geschrieben
- Entities: PascalCase, Englisch
- Properties: PascalCase mit XML-Dokumentation
- Navigation Properties: Vollqualifiziert

### Validierung
- Keine Validierung für Id-Felder
- BusinessRuleException für Geschäftsregeln
- Async-Pattern mit RejectChangesAsync()

### Internationalisierung
- Alle Labels in i18n-Dateien
- Format: `ENTITYNAME_LIST.TITLE`
- Unterstützung für DE/EN

## Troubleshooting

| Problem | Ursache | Lösung |
|---------|---------|--------|
| Build-Fehler nach Entity-Änderung | Generierter Code ist veraltet | Code-Generierung ausführen |
| Import-Fehler | CSV-Format oder Beziehungen | CSV-Datei und Fremdschlüssel prüfen |
| PostgreSQL DateTime-Fehler | `DateTime.Now` verwendet | Immer `DateTime.UtcNow` verwenden |
| Controller nicht gefunden | Nicht generiert | Code-Generierung ausführen |

### Checkliste bei Problemen

1. ✅ Ist das Logic-Projekt fehlerfrei kompilierbar?
2. ✅ Wurde die Code-Generierung nach Entity-Änderungen ausgeführt?
3. ✅ Sind alle Fremdschlüssel korrekt definiert (Typ `IdType`)?
4. ✅ Verwenden alle DateTime-Properties `DateTime.UtcNow`?
5. ✅ Haben berechnete Properties (`[NotMapped]`) einen Setter?
6. ✅ Ist die richtige Datenbank konfiguriert?
