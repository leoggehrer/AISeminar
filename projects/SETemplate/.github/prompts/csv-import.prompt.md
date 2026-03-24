# CSV-Import implementieren

Füge Import-Logik für eine Entity hinzu, damit Stamm-/Testdaten aus einer CSV-Datei geladen werden können.

## Schritte

### 1. CSV-Datei erstellen

Lege die Datei unter `SETemplate.ConApp/data/entityname_set.csv` an:

```csv
#Name;Description
First Entry;Description for first entry
Second Entry;Description for second entry
```

- Erste Zeile: Spaltenüberschriften (mit `#` auskommentiert)
- Datenzeilen: Semikolon-getrennt
- Zeilen die mit `#` beginnen werden ignoriert

### 2. CSV-Datei ins Ausgabeverzeichnis kopieren

In der Datei `SETemplate.ConApp.csproj` hinzufügen:

```xml
<ItemGroup>
  <Content Include="data\entityname_set.csv">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### 3. Import-Logik in `StarterApp.Import.cs` implementieren

**Datei:** `SETemplate.ConApp/Apps/StarterApp.Import.cs`

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

## Wichtige Hinweise

- Import nur im `DEBUG && DEVELOP_ON` Modus aktiv
- Jede Entity bekommt ihren eigenen Import-Block
- Fehler werden per `try/catch` behandelt mit `RejectChangesAsync()` Rollback
- `IdentityId` **niemals** manuell im Import setzen – wird automatisch in der Logic-Schicht gesetzt

## Nächste Schritte

1. Datenbank erstellen → Prompt: `create-database`
2. `SETemplate.ConApp` ausführen und Import-Menüpunkt starten
