# SETemplate – Quickstart

> Schnelleinstieg für die Entwicklung mit SETemplate. Für Details zu jedem Schritt die verlinkten Prompts verwenden.

---

## Vorbereitung (einmalig pro Projekt)

### 1. Datenbank wählen

| Datenbank | Kommando |
|-----------|----------|
| **SQLite** (Standard) | `dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x` |
| **PostgreSQL** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x` |
| **MSSQL Server** | `dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x` |

→ Details: `.github/prompts/setup-database.prompt.md`

### 2. Authentifizierung einstellen

```bash
# Umschalten (ON ↔ OFF):
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x
```

→ Details: `.github/prompts/toggle-auth.prompt.md`

---

## Entity erstellen

### 3. Cleanup – vor jeder Entity-Änderung

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
dotnet build SETemplate.sln
```

→ Details: `.github/prompts/cleanup.prompt.md`

### 4. Entity-Klasse anlegen

Speicherort: `SETemplate.Logic/Entities/{Data|App}/EntityName.cs`

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
    public partial class EntityName : EntityObject
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
```

**Wichtigste Regeln:**
- Klassen: `public partial`, erben von `EntityObject`
- Fremdschlüssel: Typ `IdType`
- Strings required: `= string.Empty`
- Datum: **immer** `DateTime.UtcNow`
- Berechnete Properties (`[NotMapped]`): Setter **muss** vorhanden sein
- Keine Navigation Properties für `Identity`

→ Details: `.github/prompts/new-entity.prompt.md`

### 5. Build prüfen

```bash
dotnet build SETemplate.Logic/SETemplate.Logic.csproj
```

Alle Fehler beheben, bevor du weitermachst.

### 6. Validierung hinzufügen

Datei: `SETemplate.Logic/Entities/{Data|App}/EntityName.Validation.cs`

→ Details: `.github/prompts/add-validation.prompt.md`

---

## Code generieren

### 7. Code-Generierung ausführen

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

→ Details: `.github/prompts/generate.prompt.md`

---

## Datenbank & Daten

### 8. Datenbank erstellen

```bash
dotnet run --project SETemplate.ConApp -- AppArg=1,2,x
```

→ Details: `.github/prompts/create-database.prompt.md`

### 9. CSV-Import (optional)

CSV-Datei: `SETemplate.ConApp/data/entityname_set.csv`

```csv
#Name;Description
ExampleOne;First entry
ExampleTwo;Second entry
```

→ Details: `.github/prompts/csv-import.prompt.md`

---

## Autorisierung konfigurieren (optional, nur bei `ACCOUNT_ON`)

→ Details: `.github/prompts/configure-authorization.prompt.md`

---

## Kurzreferenz – alle Kommandos

| Aktion | Kommando |
|--------|----------|
| Generierte Klassen löschen | `dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x` |
| Code generieren | `dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x` |
| Authentifizierung umschalten | `dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x` |
| SQLite aktivieren | `dotnet run --project TemplateTools.ConApp -- AppArg=3,10,x,x` |
| PostgreSQL aktivieren | `dotnet run --project TemplateTools.ConApp -- AppArg=3,8,x,x` |
| MSSQL aktivieren | `dotnet run --project TemplateTools.ConApp -- AppArg=3,9,x,x` |
| Datenbank erstellen | `dotnet run --project SETemplate.ConApp -- AppArg=1,2,x` |

---

## Code-Marker

| Marker | Bedeutung |
|--------|-----------|
| `//@GeneratedCode` | Automatisch generiert – **nicht bearbeiten** |
| `//@CustomCode` | Manuell angepasst – wird nie überschrieben |
| `//@AiCode` | Von AI erstellt – wird nie überschrieben |
