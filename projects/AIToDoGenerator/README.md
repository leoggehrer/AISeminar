# AIToDoGenerator

Ein template-basiertes To-Do-Verwaltungssystem auf Basis von .NET und Angular mit Clean Architecture. Das Projekt demonstriert die automatische Code-Generierung aus Entity-Definitionen und die KI-gestützte Generierung von Aufgabenlisten.

## Projekt-Idee

### KI-gestützte To-Do-Generierung

To-Do-Listen werden nicht manuell erstellt, sondern automatisch aus einer freien Beschreibung eines Vorhabens generiert. Zwei n8n-Workflows übernehmen die KI-Kommunikation:

1. **ToDoGeneratorAgent** – Empfängt eine freie Beschreibung per REST-Webhook und generiert mithilfe eines LangChain-AI-Agenten (OpenAI GPT-4o-mini) eine strukturierte, priorisierte To-Do-Liste mit Kategorien und optionalen Notizen je nach gewählter Detailtiefe.

2. **ToDoAIAgentChat** – Ein konversationsbasierter Chat-Agent, der Aufgaben aus dem Gespräch erkennt, sammelt, priorisiert und auf einen Speicher-Befehl hin als strukturierten JSON-Block ausgibt, der direkt ins System übernommen wird.

## Datenstruktur im User-Prompt (ToDoGeneratorAgent)

```json
{
  "listId": "string",
  "description": "freie Beschreibung des Vorhabens",
  "count": 8,
  "detail": "kurz | mittel | detailliert",
  "timestamp": "ISO string"
}
```

### Response-Format (ToDoGeneratorAgent)

```json
{
  "tasks": [
    {
      "text": "Aufgabenbeschreibung",
      "note": "Optionaler Hinweis / Tipp",
      "priority": "Low | Medium | High",
      "category": "Kategoriename",
      "sortOrder": 1
    }
  ]
}
```

## Entities

Copilot generiert mit den Angaben im User-Prompt die Entities im Projekt. Die Definition der Entitäten ist in den nachfolgenden Abschnitten aufgeführt.

### TodoList Entity

| Feld            | Typ                    | Beschreibung                                              |
| --------------- | ---------------------- | --------------------------------------------------------- |
| `Id`          | `int`                | Primärschlüssel (auto, von `EntityObject`)            |
| `Title`       | `string` (max. 200)  | Titel der To-Do-Liste                                     |
| `Description` | `string` (max. 1000) | Freie Beschreibung des Vorhabens (Prompt für die KI)      |
| `TaskCount`   | `int`                | Gewünschte Anzahl zu generierender Aufgaben (Standard: 8) |
| `DetailLevel` | `DetailLevel`        | Detailtiefe der generierten Aufgaben                      |
| `Status`      | `TodoStatus`         | Aktueller Generierungsstatus                              |
| `CreatedAt`   | `DateTime`           | Erstellungszeitpunkt (UTC)                                |
| `TodoTasks`   | `List<TodoTask>`     | Navigations-Property zur Liste der zugehörigen Aufgaben   |

### TodoTask Entity

| Feld          | Typ                   | Beschreibung                                              |
| ------------- | --------------------- | --------------------------------------------------------- |
| `Id`        | `int`               | Primärschlüssel (auto, von `EntityObject`)            |
| `TodoListId`| `IdType`            | Fremdschlüssel zur übergeordneten `TodoList`          |
| `Text`      | `string` (max. 500) | Aufgabenbeschreibung                                      |
| `Note`      | `string?` (max. 1000)| Optionale Notiz mit zusätzlichen Informationen / Tipps   |
| `Priority`  | `TaskPriority`      | Prioritätsstufe der Aufgabe                              |
| `Category`  | `string` (max. 100) | KI-generierte Kategorie                                   |
| `IsDone`    | `bool`              | Gibt an, ob die Aufgabe erledigt ist                      |
| `SortOrder` | `int`               | Sortierreihenfolge innerhalb der Liste                    |
| `TodoList`  | `TodoList?`         | Navigations-Property zur übergeordneten Liste             |

### Enums

**`DetailLevel`**

| Wert          | Beschreibung                                               |
| ------------- | ---------------------------------------------------------- |
| `Short`     | Kurze, prägnante Aufgaben (max. 6 Wörter, keine Notizen) |
| `Medium`    | Klare Aufgaben mit kurzer Notiz als Hilfestellung (1 Satz) |
| `Detailed`  | Ausführliche Aufgaben mit Tipps, Tools und Zeitschätzungen |

**`TaskPriority`**

| Wert       | Beschreibung         |
| ---------- | -------------------- |
| `Low`    | Niedrige Priorität  |
| `Medium` | Mittlere Priorität  |
| `High`   | Hohe Priorität      |

**`TodoStatus`**

| Wert           | Beschreibung                              |
| -------------- | ----------------------------------------- |
| `Generating` | KI-Generierung läuft                     |
| `Ready`      | Liste erfolgreich generiert und bereit    |
| `Error`      | Fehler bei der Generierung aufgetreten    |


## Lösung

Die fertige Lösung des Projektes finden Sie auf:

https://github.com/leoggehrer/SEeAIToDoGenerator

