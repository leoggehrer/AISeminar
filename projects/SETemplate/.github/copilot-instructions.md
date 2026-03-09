# GitHub Copilot Instructions für SETemplate

> **Version:** 2.0 | **Letzte Aktualisierung:** Januar 2026

---

## Schnellreferenz - Wichtigste Kommandos

| Aktion | Kommando |
|--------|----------|
| **Authentifizierung umschalten** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x` |
| **PostgreSQL aktivieren** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x` |
| **MSSQL aktivieren** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x` |
| **SQLite aktivieren** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x` |
| **Code generieren** | `dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x` |
| **Generierte Klassen löschen** | `dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x` |
| **Datenbank erstellen** | `dotnet run --project SETemplate.ConApp -- AppArg=1,2,x` |

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

### Wichtigste Kommandos

```bash
# Authentifizierung ein/ausschalten
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x

# Datenbank Postgres verwenden
dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x

# Datenbank MSSQL Server verwenden
dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x

# Datenbank Sqlite verwenden
dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x

# Code-Generierung ausführen
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x

# Generierte Klassen löschen (vor Änderungen an Entities)
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x

# Datenbank erstellen und migrieren
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

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

```bash
# Code-Generierung ausführen:
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

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

Im Template sind grundlegende Authentifizierungs- und Autorisierungsmechanismen implementiert. Diese können über das TemplateTools-Projekt konfiguriert werden.

```bash
# Authentifizierung ein/ausschalten:
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x
```

**Standard:** Authentifizierung ist deaktiviert. Frage den Benutzer vor dem Start, ob sie benötigt wird.

| Define        | Bedeutung |
|---------------|-----------|
| `ACCOUNT_ON`  | Authentifizierung eingeschaltet |
| `ACCOUNT_OFF` | Authentifizierung ausgeschaltet |

Wenn die Authentifizierung aktiviert ist, werden alle API-Endpunkte geschützt und erfordern gültige Tokens. Das `ACCOUNT_ON` Define steuert die Einbindung der Account-Entities und -Logik. Der Zuriff kann über Rollen gesteuert werden. Diese Rollen werden beim Erstellen des Benutzers bzw. Account zugewiesen.

**Beispiel:**

```csharp
static partial void CreateUserAccounts()
{
    Task.Run(async () =>
    {
        var accounts = new (string UserName, string Email, string Password, int Timeout, string[] Roles)[]
        {
            // Name, Email, Password, Session Timeout (Minuten), Rollen
            new ("Richard Kainerstorfer", "r.kainerstorfer@htl-leonding.ac.at", "Passme1234", 30, new[] { "Staff", "Teacher", "Director" }),
            new ("Peter Bauer", "p.bauer@htl-leonding.ac.at", "Passme1234", 30, new[] { "Staff", "Teacher", "DepartmentHead" }),
            new ("Alfred Wiedermann", "a.wiedermann@htl-leonding.ac.at", "Passme1234", 30, new[] { "Staff", "Teacher", "DepartmentHead" }),
            new ("Gerhard Gehrer", "g.gehrer@htl-leonding.ac.at", "Passme1234", 30, new[] { "Staff", "Teacher" }),

            new ("Student.One", "student.one@htl-leonding.ac.at", "Passme1234", 30, new[] { "Student" }),
            new ("Student.Two", "student.two@htl-leonding.ac.at", "Passme1234", 30, new[] { "Student" }),
            new ("Student.Three", "student.three@htl-leonding.ac.at", "Passme1234", 30, new[] { "Student" }),
        };

        foreach (var account in accounts)
        {
            await AddAppAccessAsync(SAEmail, SAPwd, account.UserName, account.Email, account.Password, account.Timeout, account.Roles);
        }
    }).Wait();
}
```

**Zugriff konfigurieren:**

Erstelle eine partielle Klasse `partial class EntitySet<TEntity>` im Namespace `SETemplate.Logic.DataContext`. Konfiguriere im Constructor die Zugriffsrechte:

```csharp
//@AiCode
#if GENERATEDCODE_ON && ACCOUNT_ON

namespace SETemplate.Logic.DataContext;

using SETemplate.Logic.DataContext.App;

partial class EntitySet<TEntity>
{
    static partial void ClassConstructed()
    {
        var appAuthorize = new Modules.Security.AuthorizeAttribute("SysAdmin", "AppAdmin");
        var generalAuthorize = appAuthorize.Clone("Student", "Teacher", "DepartmentHead", "Director");
        var createAuthorize = appAuthorize.Clone("Student");
        var updateAuthorize = appAuthorize.Clone("Teacher", "DepartmentHead", "Director");

        SetAuthorization(typeof(EntitySet<TEntity>), appAuthorize);

        SetAuthorization4Read(typeof(EntitySet<TEntity>), generalAuthorize);
        SetAuthorization4Create(typeof(AbsenceRequestSet), createAuthorize);
        SetAuthorization4Update(typeof(AbsenceRequestSet), updateAuthorize);

        SetAuthorization4Read(typeof(ApprovalStepSet), appAuthorize.Clone("Teacher", "DepartmentHead", "Director"));
        SetAuthorization4Create(typeof(ApprovalStepSet), appAuthorize.Clone("Teacher", "DepartmentHead", "Director"));
        SetAuthorization4Update(typeof(ApprovalStepSet), appAuthorize.Clone("Teacher", "DepartmentHead", "Director"));
    }
}
#endif
```

Ein anderes Beispiel ist die `EmployeeSet`-Entity, die nur von Benutzern mit den Rollen `SysAdmin` oder `AppAdmin` verwaltet werden kann, während alle authentifizierten Benutzer Lesezugriff haben.

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
        var allUsersAuthorize = new Modules.Security.AuthorizeAttribute(); // Alle authentifizierten Benutzer

        // Employee: SysAdmin und AppAdmin für alle Operationen, außer Read (alle Benutzer)
        SetAuthorization(typeof(EmployeeSet), appAdminAuthorize);
        SetAuthorization4Read(typeof(EmployeeSet), allUsersAuthorize);
    }
}
#endif
```

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

Es stehen drei Datenbankoptionen zur Verfügung: PostgreSQL, MSSQL Server und SQLite. Die Auswahl der Datenbank erfolgt über das TemplateTools-Projekt. Standardmäßig ist SQLite eingestellt.

```bash
# Datenbank Postgres verwenden
dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x

# Datenbank MSSQL Server verwenden
dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x

# Datenbank Sqlite verwenden
dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x
```

Überprüfe die `SETemplate.ConApp`-Projektdatei, um sicherzustellen, dass die gewünschten Datenbank-Defines gesetzt sind:

```xml
  <PropertyGroup>
    <DefineConstants>ACCOUNT_OFF;..;POSTGRES_OFF;SQLSERVER_OFF;SQLITE_ON;...</DefineConstants>
  </PropertyGroup>
```

## Entity-Entwicklung

### Grundregeln (KRITISCH!)

1. **Benennung:** Alle Entities **IMMER** auf Englisch
2. **Modifier:** Klassen sind `public` und `partial`
3. **Vererbung:** Klassen erben von `EntityObject`
4. **Dokumentation:** XML-Kommentare für alle Properties (Englisch)
5. **Namespaces:** `SETemplate.Logic.Entities[.SubFolder]` als Basis
6. **Enums:** Enums werden in eigenen Dateien und im `SETemplate.Common.Enums[.SubFolder]` abgelegt

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

### Import Template

1. **Namespace:** Der Namespace `SETemplate.ConApp.Apps` ist verpflichtend und darf nicht geändert werden.
2. **Dateiname:** Der Import-Code wird in der Datei `StarterApp.Import.cs` implementiert.
3. **Dateistruktur:** Alle CSV-Dateien werden im Unterordner `data` gespeichert.
4. **CSV-Datei:** Jede Entität hat eine eigene CSV-Datei, benannt nach der Entität, z.B. `entityName_set.csv`.
   - Die erste Zeile der CSV-Datei enthält die Spaltennamen.
   - Die Datenzeilen folgen dem Format: `Property1;Property2;...;PropertyN`.
   - Kommentare in der CSV-Datei beginnen mit `#` und werden ignoriert.
   - Alle CSV-Dateien werden automatisch in das Ausführungsverzeichnis kopiert.
5. **Code-Struktur:**  
   - Die Import-Logik wird als **partial-Methode** in der Klasse `StarterApp` implementiert.  
   - **Jede Entität** erhält ihren eigenen Import-Block in der Datei `StarterApp.Import.cs`.  
6. **Fehlerbehandlung:**  
   - Alle Importvorgänge sind asynchron (`async/await`).  
   - Fehler werden per `try/catch` behandelt, mit Rollback über `RejectChangesAsync()`.  

```csharp
//@AiCode
#if GENERATEDCODE_ON

using System.Reflection;

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
            var filePath = Path.Combine(AppContext.BaseDirectory, "data", "entityName_set.csv");

            foreach (var line in File.ReadLines(filePath).Skip(1).Where(l => !l.StartsWith('#')))
            {
                var parts = line.Split(';');
                var entity = new Logic.Entities.EntityName
                {
                    PropertyName = parts[0],
                    // ... weitere Properties
                };
                try
                {
                    await context.EntityNameSet.AddAsync(entity);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    await context.RejectChangesAsync();
                    Console.WriteLine($"Error in line: {ex.Message}");
                }
            }
        }
    }
}
#endif
```

### CSV Format
```csv
#Name;Description
Developer;Software Developer
Manager;Project Manager
```

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

### Beispiel: REST-API Client

**Dateiname:** `SETemplate.Logic/Modules/ApiClients/WeatherApiClient.cs`

```csharp
//@AiCode
namespace SETemplate.Logic.Modules.ApiClients
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Client for accessing external weather API.
    /// </summary>
    public partial class WeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        public WeatherApiClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        /// <summary>
        /// Gets the current weather for a specific location.
        /// </summary>
        /// <param name="city">The city name.</param>
        /// <returns>Weather data.</returns>
        public async Task<WeatherData?> GetCurrentWeatherAsync(string city)
        {
            var url = $"https://api.weather.com/v1/current?city={city}&apikey={_apiKey}";
            
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<WeatherData>();
            }
            catch (HttpRequestException ex)
            {
                // Log error
                Console.WriteLine($"Error fetching weather data: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// Represents weather data from the API.
    /// </summary>
    public record WeatherData
    {
        public string City { get; init; } = string.Empty;
        public double Temperature { get; init; }
        public string Description { get; init; } = string.Empty;
    }
}
```

### Beispiel: Business Logic Service

**Dateiname:** `SETemplate.Logic/Modules/Services/OrderProcessingService.cs`

```csharp
//@AiCode
namespace SETemplate.Logic.Modules.Services
{
    using SETemplate.Logic.Contracts;
    using SETemplate.Logic.Entities.App;
    using SETemplate.Logic.Modules.Exceptions;

    /// <summary>
    /// Service for processing orders.
    /// </summary>
    public partial class OrderProcessingService
    {
        private readonly IContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderProcessingService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public OrderProcessingService(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Processes an order with business logic.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <returns>True if successful.</returns>
        public async Task<bool> ProcessOrderAsync(IdType orderId)
        {
            var handled = false;
            BeforeProcessOrder(ref handled, orderId);
            
            if (!handled)
            {
                // Business logic here
                var order = await GetOrderAsync(orderId);
                
                if (order == null)
                {
                    throw new BusinessRuleException($"Order with ID {orderId} not found.");
                }

                // Validate order
                ValidateOrder(order);
                
                // Process order
                order.Status = "Processed";
                order.ProcessedDate = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
            }
            
            AfterProcessOrder(orderId);
            return true;
        }

        /// <summary>
        /// Validates the order.
        /// </summary>
        private void ValidateOrder(Order order)
        {
            if (order.Status == "Cancelled")
            {
                throw new BusinessRuleException("Cannot process a cancelled order.");
            }
        }

        /// <summary>
        /// Gets an order by ID.
        /// </summary>
        private async Task<Order?> GetOrderAsync(IdType orderId)
        {
            // Implementation
            return null; // Placeholder
        }

        #region partial methods
        partial void BeforeProcessOrder(ref bool handled, IdType orderId);
        partial void AfterProcessOrder(IdType orderId);
        #endregion partial methods
    }
}
```

### Beispiel: Helper-Klasse

**Dateiname:** `SETemplate.Logic/Modules/Helpers/StringHelper.cs`

```csharp
//@AiCode
namespace SETemplate.Logic.Modules.Helpers
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class for string operations.
    /// </summary>
    public static partial class StringHelper
    {
        /// <summary>
        /// Validates if a string is a valid email address.
        /// </summary>
        /// <param name="email">The email to validate.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        /// <summary>
        /// Converts a string to title case.
        /// </summary>
        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
    }
}
```

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

### Registrierung von Services

Services müssen in der Dependency Injection registriert werden:

**In `Program.cs` oder `Startup.cs`:**
```csharp
// HttpClient für API-Clients
builder.Services.AddHttpClient<WeatherApiClient>();

// Custom Services
builder.Services.AddScoped<OrderProcessingService>();
```

---

## Angular Komponenten

Die spezifischen Anweisungen für die Erstellung und Strukturierung der Angular-Komponenten befinden sich im Ordner SETemplate.AngularApp/.github/copilot-instructions.md.

## Entwicklungs-Workflow

**WICHTIG: Zu Beginn stelle dich kurz vor und erkläre, dass du diese Anweisungen befolgst, um konsistenten und qualitativ hochwertigen Code zu gewährleisten.**

### 1. Datenbank auswählen (WICHTIG!)
1. **IMMER** Zu Beginn den Benutzer fragen, welche Datenbank verwendet werden soll
2. Optionen:
   - PostgreSQL: `dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x`
   - MSSQL Server: `dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x`
   - SQLite: `dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x` (Standard)
3. Nach der Auswahl die entsprechende Konfiguration ausführen

### 2. Authentifizierung einstellen (WICHTIG!)
1. **IMMER** Zu Beginn den Benutzer fragen, ob er die Authentifizierung benötigt
2. Die Standard-Einstellung ist ohne Authentifizierung. 
3. Frage den Benutzer, ob Authentifizierung benötigt wird.
4. Wenn ja, dann Authentifizierung ausführen: `dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x`

### 3. Entity erstellen
1. Lösche generierte Klassen (falls vorhanden): `dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x`
2. Entity-Klasse in `Logic/Entities/{Data|App}/` erstellen
3. Das Entity-Modell mit dem Benutzer abklären und bestätigen lassen.
4. Prüfe, ob das Projekt 'Logic' erfolgreich gebaut werden kann.
  - Falls nicht, behebe alle Fehler bevor du fortfährst.

### 4. Überprüfung des Modells
1. Führe mit dem Benutzer eine Überprüfung des Modells durch.
2. Stelle sicher, dass alle Entitäten und Beziehungen korrekt definiert sind.
3. Warte auf die Bestätigung des Benutzers, bevor du fortfährst.

### 5. Validierung erstellen
1. Nach Bestätigung des Modells: Validierung in separater `.Validation.cs` Datei erstellen
2. Implementiere alle notwendigen Validierungsregeln
3. Prüfe erneut, ob das Projekt 'Logic' erfolgreich gebaut werden kann.

### 6. Code-Generierung
1. Code-Generierung ausführen: `dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x`

### 7. Daten-Import
1. CSV-Datei in `ConApp/data/entityname_set.csv` erstellen
2. Einstellen, dass die CSV-Datei ins Ausgabeverzeichnis kopiert wird
3. Import-Logic in `StarterApp.Import.cs` hinzufügen
4. Console-App ausführen und Import starten

### 8. Datenbank erstellen und Import starten
1. Datenbank erstellen: `dotnet run --project SETemplate.ConApp -- AppArg=1,2,x`

### 9. Anpassungen
- Custom Code in `//@CustomCode` Bereichen
- Separate `.Custom.cs` Dateien für erweiterte Logik
- `editMode` boolean für Create/Edit-Unterscheidung

### 10. Änderungen und Erweiterungen
- Änderungen die die Entitäten betreffen
  - Zuerst die generierten Klassen entfernen:
    1. Delete generierte Klassen: 
    `dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x`
- Danach starte wieder beim Workflow bei Punkt 3.

## Konventionen

### Naming
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

### Häufige Probleme und Lösungen

| Problem | Ursache | Lösung |
|---------|---------|--------|
| Build-Fehler nach Entity-Änderung | Generierter Code ist veraltet | `dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x` |
| "System.Object not defined" | Fehlende Referenzen | Logic-Projekt neu bauen |
| Import-Fehler | CSV-Format oder Beziehungen | CSV-Datei und Fremdschlüssel prüfen |
| PostgreSQL DateTime-Fehler | `DateTime.Now` verwendet | Immer `DateTime.UtcNow` verwenden |
| Controller nicht gefunden | Routing fehlt | Code-Generierung ausführen |
| Angular Komponente fehlt | Nicht generiert | Code-Generierung ausführen |

### Debugging-Tipps

- **Generated Code identifizieren:** Suche nach `//@GeneratedCode` oder `//@AiCode` Markern
- **Custom Code isolieren:** Nutze separate `.Custom.cs` Dateien
- **Datenbank testen:** Nutze `SETemplate.ConApp` für direkte DB-Tests
- **API testen:** Swagger UI unter `https://localhost:xxxx/swagger`
- **Logs prüfen:** Konsolenausgabe bei `dotnet run` beachten

### Checkliste bei Problemen

1. ✅ Ist das Logic-Projekt fehlerfrei kompilierbar?
2. ✅ Wurde die Code-Generierung nach Entity-Änderungen ausgeführt?
3. ✅ Sind alle Fremdschlüssel korrekt definiert (Typ `IdType`)?
4. ✅ Verwenden alle DateTime-Properties `DateTime.UtcNow`?
5. ✅ Haben berechnete Properties (`[NotMapped]`) einen Setter?
6. ✅ Ist die richtige Datenbank konfiguriert?

---

## Anhang: Vollständige Workflow-Checkliste

```
□ Benutzer nach Datenbank fragen (PostgreSQL/MSSQL/SQLite)
□ Benutzer nach Authentifizierung fragen (Ja/Nein)
□ Benutzer nach CSV-Import fragen (Ja/Nein)
□ Generierte Klassen löschen (falls Entity-Änderungen)
□ Entities erstellen in Logic/Entities/{Data|App}/
□ Logic-Projekt bauen und Fehler beheben
□ Entity-Modell mit Benutzer überprüfen und bestätigen lassen
□ Nach Bestätigung: Validierung in separater .Validation.cs Datei erstellen
□ Logic-Projekt erneut bauen
□ Code-Generierung ausführen
□ CSV-Dateien erstellen (falls gewünscht)
□ Import-Logik implementieren (falls gewünscht)
□ Datenbank erstellen und migrieren
□ Anwendung testen
```
