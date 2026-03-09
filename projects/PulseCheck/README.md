# PulseCheck

Ein template-basiertes Produktkatalog-System auf Basis von .NET 8 und Angular mit Clean Architecture. Das Projekt demonstriert die automatische Code-Generierung aus Entity-Definitionen und die KI-gestützte Bewertung von Kunden-Feedbacks.

## Projekt-Idee

### KI-Anreicherung von Produktdaten

Kunden-Feedbacks werden nicht nur verwaltet, sondern automatisch mit einem KI-Assistenten bewertet. 

## Datenstruktur im User-Prompt

```json
  id: number;
  author: string;
  category: 'product' | 'service' | 'support' | 'general';
  text: string;
  rating: 1|2|3|4|5;
  sentiment?: 'positive' | 'neutral' | 'negative';
  keywords?: string[];
  summary?: string;
  createdAt: Date;
```

## Entities

Copilot generiert mit den Angabe im User-Prompt die Entities im Projekt. Die Definition der Entität ist im nachfolgendem Abschnitt aufgeführt.

### Product Feedback

| Feld                  | Typ                     | Beschreibung                                                  |
| --------------------- | ----------------------- | ------------------------------------------------------------- |
| `Id`                  | `int`                   | Primärschlüssel (auto, von `EntityObject`)                    |
| `Author`              | `string` (max. 100)     | Name des Autors.                                              |
| `Category`            | `FeedbackCategory`      | Kategorie (Enum)                                              |
| `Text`                | `string` (max. 2000)    | Feedback vom Benutzer                                         |
| `Rating`              | `int`                   | Benutzer Rating                                               |
| `Sentiment`           | `Sentiment`             | Sentiment (Enum)                                              |
| `Keywords`            | `string?` (max. 500)    | KI-generierte Schlüsselwörter                                 |
| `Summary`             | `string?` (max. 1000)   | KI-generierte Zusammenfassung                                 |
| `CreatedOn`           | `DateTime`              | Erstellungszeitpunkt (UTC)                                    |
| `FeedbackStatus`      | `FeedbackStatus`        | FeedbackStatus (Enum)                                         |

### Enums

**`FeedbackCategory`**

| Wert            | Bedeutung  |
| --------------- | ---------- |
| `Product`       | Produktbezogen |
| `Service`       | Servicebezogen |
| `Support`       | Supportbezogen |
| `General`       | Allgemeines Feedback |

**`Sentiment`**

| Wert          | Beschreibung                                 |
| ------------- | -------------------------------------------- |
| `Pending`   | Sentiment-Analyse steht noch aus             |
| `Positive`  | Positives Sentiment erkannt                  |
| `Neutral`   | Neutrales Sentiment erkannt                  |
| `Negative`  | Negatives Sentiment erkannt                  |

**`FeedbackStatus`**

| Wert           | Bedeutung                         |
| -------------- | --------------------------------- |
| `Pending`    | Feedback wartet auf Bearbeitung   |
| `Reviewed`   | Feedback wurde geprüft            |
| `Approved`   | Feedback wurde freigegeben        |
| `Rejected`   | Feedback wurde abgelehnt          |

### n8n Webhook – Request-Format

Der n8n-Workflow empfängt Feedback-Daten unter `POST /rate-feedback-ai`.
Im Frontend wird dafür `environment.N8N_RATE_FEEDBACK_AI_URL` verwendet (standardmäßig: `http://localhost:5678/webhook/rate-feedback-ai`).

```json
{
  "feedback": {
    "id": 1,
    "author": "Max Mustermann",
    "category": 1,
    "text": "Das Produkt ist gut, aber die Lieferung war langsam.",
    "rating": 3,
    "timestamp": "2026-03-09T10:15:00Z",
    "status": 1,
    "sentiment": 0,
    "keywords": null,
    "summary": null,
    "sentimentScore": null,
    "priority": null,
    "response": null,
    "createdOn": "2026-03-09T10:15:00Z"
  },
  "timestamp": "2026-03-09T10:15:00Z"
}
```

### n8n Webhook – Response-Format

```json
{
  "sentiment": "positive | neutral | negative",
  "keywords": ["Schlagwort 1", "Schlagwort 2"],
  "summary": "Kurze Zusammenfassung auf Deutsch.",
  "feedbackId": 1
}
```

Hinweis: Optional zusätzliche Felder wie `response`, `sentimentScore` und `priority` werden vom Frontend ebenfalls verarbeitet, falls sie vom Webhook geliefert werden.


## Lösung

Die fertige Lösung des Projektes finden Sie auf:

https://github.com/leoggehrer/SEePulseCheck

