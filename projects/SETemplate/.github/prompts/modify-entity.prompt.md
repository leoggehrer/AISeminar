# Bestehende Entity ändern

Ändere eine oder mehrere bestehende Entities in `SETemplate.Logic/Entities/`. Die Änderungen können Properties, Navigation Properties, Validierungslogik oder Datenbankattribute betreffen.

Arbeite strikt in der angegebenen Reihenfolge und stoppe sofort beim ersten Fehler.

---

## Phase 1 – Vorbereitung (PFLICHT vor jeder Änderung)

### Schritt 1 – Generierte Klassen löschen (Cleanup)

**IMMER als erstes ausführen!** Verhindert Konflikte zwischen altem und neuem generierten Code.

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x
```

### Schritt 2 – Build-Prüfung nach Cleanup

Sicherstellen, dass die Solution nach dem Cleanup fehlerfrei baut:

```bash
dotnet build SETemplate.sln
```

> ⛔ Bei Fehlern: Erst beheben, bevor Entities geändert werden.

---

## Phase 2 – Entity-Änderungen durchführen

Führe alle Änderungen in `SETemplate.Logic/Entities/` durch. Typische Änderungen:

### Properties hinzufügen / ändern

- Neuen Typ: Fremdschlüssel immer `IdType`, DateTime immer `DateTime.UtcNow`
- Required-Strings mit `string.Empty` initialisieren
- Optionale Strings als `string?` ohne Initialisierung
- XML-Dokumentation für alle neuen Properties
- `[MaxLength(...)]` für Strings deklarieren
- Unique-Constraints via `[Index(nameof(Property), IsUnique = true)]` auf der Klasse

```csharp
/// <summary>
/// Gets or sets the new property.
/// </summary>
[MaxLength(200)]
public string? NewProperty { get; set; }
```

### Properties entfernen

- Property-Deklaration und zugehörige Backing-Fields entfernen
- Partielle Methoden (`OnXyzChanging`, `OnXyzChanged`) entfernen, wenn nicht mehr benötigt
- Navigation Properties und Fremdschlüssel gemeinsam entfernen

### Navigation Properties hinzufügen / ändern

- Fremdschlüssel (`IdType`) und Navigation Property **immer gemeinsam** definieren
- Navigation Property **vollqualifiziert** deklarieren:

```csharp
// Fremdschlüssel
public IdType RelatedEntityId { get; set; }

// Navigation Property (vollqualifiziert)
/// <summary>
/// Gets or sets the related entity.
/// </summary>
public SETemplate.Logic.Entities.App.RelatedEntity? RelatedEntity { get; set; }
```

- Für 1:n auf der "One"-Seite eine Collection ergänzen:

```csharp
/// <summary>
/// Gets or sets the list of child entities.
/// </summary>
public List<SETemplate.Logic.Entities.App.ChildEntity> ChildEntities { get; set; } = [];
```

### Datenbankattribute anpassen

- Tabellenname oder Schema geändert: `[Table(...)]`-Attribut aktualisieren
- Neuer Unique-Index: `[Index(nameof(Property), IsUnique = true)]` zur Klasse hinzufügen
- Index entfernen: Das entsprechende `[Index(...)]`-Attribut löschen

### Berechnete Properties (`[NotMapped]`)

Berechnete Properties **müssen** immer einen (leeren) Setter haben:

```csharp
/// <summary>
/// Gets a value indicating whether the item is valid.
/// </summary>
[NotMapped]
public bool IsValid
{
    get => /* Berechnung */;
    set { /* Required for serialization */ }
}
```

---

## Phase 3 – Build-Prüfung nach Änderungen

### Schritt 3 – Logic-Projekt bauen

```bash
dotnet build SETemplate.Logic/SETemplate.Logic.csproj
```

> ⛔ Bei Fehlern: Alle Fehler beheben, bevor du fortfährst. Der Generator arbeitet via **Reflection** und benötigt kompilierbaren Code.

### Schritt 4 – Änderungen mit dem Benutzer überprüfen

Fasse die durchgeführten Änderungen kompakt zusammen und warte auf Bestätigung:

- Welche Properties wurden hinzugefügt / geändert / entfernt?
- Welche Navigation Properties / Fremdschlüssel wurden angepasst?
- Datenbankattribute korrekt?
- **Warte auf Bestätigung des Benutzers.**

---

## Phase 4 – Validierung anpassen (falls erforderlich)

Wenn sich Properties geändert haben, die einer Validierung unterliegen, passe die Datei `EntityName.Validation.cs` entsprechend an:

- Neue Properties validieren (Pflichtfelder, Wertebereiche, Formate)
- Entfernte Properties aus der Validierung löschen
- Bestehende Regeln auf Korrektheit prüfen

→ Details: Prompt `add-validation`

Erneut bauen und Fehler beheben.

---

## Phase 5 – Code-Generierung

### Schritt 5 – Code generieren

Alle Änderungen sind abgeschlossen und das Logic-Projekt baut fehlerfrei → jetzt generieren:

```bash
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

### Schritt 6 – Abschließende Build-Prüfung

```bash
dotnet build SETemplate.sln
```

---

## Abschluss-Reporting

Melde nach der Ausführung kompakt:

| Schritt | Ergebnis |
|---------|----------|
| Cleanup (Schritt 1) | ✅ / ❌ |
| Build nach Cleanup (Schritt 2) | ✅ / ❌ |
| Änderungen durchgeführt | Zusammenfassung |
| Build Logic (Schritt 3) | ✅ / ❌ |
| Benutzerbestätigung | Erhalten / Ausstehend |
| Validierung angepasst | Ja / Nein / Nicht notwendig |
| Code-Generierung (Schritt 5) | ✅ / ❌ |
| Abschließender Build (Schritt 6) | ✅ / ❌ |

Bei Fehlern: ersten Fehler mit Datei/Projekt nennen und nächsten sinnvollen Fix-Schritt angeben.

---

## Wichtige Regeln (Checkliste)

- ✅ Generierte Klassen **IMMER** vor Änderungen löschen (Schritt 1)
- ✅ Logic-Projekt kompilierbar, bevor Code-Generierung startet
- ✅ Alle Properties und Kommentare auf **Englisch**
- ✅ `DateTime.UtcNow` (niemals `DateTime.Now`)
- ✅ `IdType` für alle Fremdschlüssel
- ✅ Navigation Properties vollqualifiziert deklarieren
- ✅ `[NotMapped]` Properties haben immer einen (leeren) Setter
- ❌ Keine Änderungen in Dateien mit `//@GeneratedCode`-Marker
- ❌ Keine Navigation Properties vom Typ `Identity` (ist `internal`)
- ❌ `Id`-Property nicht deklarieren (kommt von `EntityObject`)
