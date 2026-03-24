Erstelle eine neue Entity-Klasse in `SETemplate.Logic/Entities/` nach den Projektkonventionen.

Frage den Benutzer nach: Entity-Name, Properties, Beziehungen zu anderen Entities.

## Pflichtregeln
- Klassen IMMER auf Englisch benennen
- Modifier: `public partial class`
- Vererbung: `: EntityObject`
- `Id`-Property NICHT deklarieren (kommt von EntityObject)
- Fremdschlüssel immer Typ `IdType`
- IMMER `DateTime.UtcNow` (niemals DateTime.Now)
- XML-Dokumentation für alle Properties
- Navigation Properties vollqualifiziert deklarieren
- Berechnete Properties (`[NotMapped]`) müssen einen (leeren) Setter haben

## Dateistruktur
| Typ | Pfad |
|-----|------|
| Stammdaten | `SETemplate.Logic/Entities/Data/EntityName.cs` |
| Anwendungsdaten | `SETemplate.Logic/Entities/App/EntityName.cs` |
| Views (ReadOnly) | `SETemplate.Logic/Entities/Views/EntityName.cs` |

## Table-Attribut
```csharp
#if SQLITE_ON
    [Table("EntityNames")]
#else
    [Table("EntityNames", Schema = "app")]
#endif
```

Verwende das Entity-Template aus der CLAUDE.md. Beachte alle Property-Regeln, Navigation-Property-Regeln und Using-Regeln.

## Nächste Schritte nach der Entity-Erstellung
1. Logic-Projekt bauen: `dotnet build SETemplate.Logic/SETemplate.Logic.csproj`
2. Entity-Modell mit Benutzer überprüfen
3. Validierung erstellen → `/add-validation`
4. Code-Generierung → `/generate`
