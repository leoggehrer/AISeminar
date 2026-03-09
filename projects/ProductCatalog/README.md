# ProductCatalog

Ein template-basiertes Produktkatalog-System auf Basis von .NET 8 und Angular mit Clean Architecture. Das Projekt demonstriert die automatische Code-Generierung aus Entity-Definitionen und die KI-gestützte Anreicherung von Produktdaten.

## Projekt-Idee

### KI-Anreicherung von Produktdaten

Produkte werden nicht nur verwaltet, sondern automatisch mit verkaufsstarken Beschreibungen und Tags versehen. Ein n8n-Workflow nimmt rohe Produktdaten (Name, Kategorie, Preis) und generiert via GPT-4.1-mini drei tonale Varianten der Beschreibung (luxury, casual, technical).

## Datenstruktur im User-Prompt

```json
  id: number;
  name: string;
  category: 'elektronik' | 'mode' | 'haushalt' | 'sport' | 'beauty';
  price: number;
  stock: number;
  description?: string;      // KI-generiert
  tags?: string[];           // KI-generiert
  tone?: 'luxury' | 'casual' | 'technical';
  descriptionStatus: 'none' | 'generating' | 'done';
  createdAt: Date;
```

## Entities

Copilot generiert mit den Angabe im User-Prompt die Entities im Projekt. Die Definition der Entität ist im nachfolgendem Abschnitt aufgeführt.

### Product Entity

| Feld                  | Typ                     | Beschreibung                                                  |
| --------------------- | ----------------------- | ------------------------------------------------------------- |
| `Id`                | `int`                 | Primärschlüssel (auto, von `EntityObject`)                |
| `Name`              | `string` (max. 200)   | Produktname, eindeutig                                        |
| `Category`          | `ProductCategory`     | Kategorie (Enum)                                              |
| `Price`             | `decimal (18,2)`      | Verkaufspreis                                                 |
| `Stock`             | `int`                 | Lagerbestand                                                  |
| `Description`       | `string?` (max. 4000) | KI-generierte Beschreibung                                    |
| `TagData`           | `string?` (max. 4000) | Kommagetrennte Tags (Persistenz)                              |
| `Tags`              | `string[]?`           | KI-generierte Tags (nicht gemappt, berechnet aus `TagData`) |
| `Tone`              | `ProductTone?`        | Gewünschter Schreibton für die Generierung                  |
| `DescriptionStatus` | `DescriptionStatus`   | Generierungsstatus                                            |
| `CreatedAt`         | `DateTime`            | Erstellungszeitpunkt (UTC)                                    |

### Enums

**`ProductCategory`**

| Wert            | Bedeutung  |
| --------------- | ---------- |
| `Electronics` | Elektronik |
| `Fashion`     | Mode       |
| `Household`   | Haushalt   |
| `Sports`      | Sport      |
| `Beauty`      | Schönheit |

**`ProductTone`**

| Wert          | Beschreibung                                 |
| ------------- | -------------------------------------------- |
| `Luxury`    | Elegant, exklusiv, gehobene Sprache          |
| `Casual`    | Locker, sympathisch, direkte Anrede          |
| `Technical` | Technisch präzise, Spezifikationen & Fakten |

**`DescriptionStatus`**

| Wert           | Bedeutung                         |
| -------------- | --------------------------------- |
| `None`       | Noch keine Beschreibung generiert |
| `Generating` | Generierung läuft                |
| `Done`       | Beschreibung fertig               |

### n8n Webhook – Request-Format

Der n8n-Workflow empfängt Produktdaten unter `POST /product-description-ai`:

```json
{
  "product": {
    "id": "...",
    "name": "...",
    "category": "...",
    "price": 0.00,
    "stock": 0,
    "tone": "luxury | casual | technical"
  },
  "timestamp": "..."
}
```

### n8n Webhook – Response-Format

```json
{
  "description": "2-3 Sätze Produktbeschreibung auf Deutsch.",
  "tags": ["Schlagwort 1", "Schlagwort 2", "Schlagwort 3"],
  "productId": "..."
}
```

## Lösung

Die fertige Lösung des Projektes finden Sie auf:

https://github.com/leoggehrer/SEeProductCatalog

