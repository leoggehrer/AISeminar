# Workflow: ToDoGeneratorAgent

| Eigenschaft | Wert |
|---|---|
| **ID** | `qNxbxXfbyPw1wzpR` |
| **Status** | Aktiv |
| **Erstellt** | 2026-03-08 |
| **Zuletzt geändert** | 2026-03-08 |

## Beschreibung

Empfängt eine freie Beschreibung eines Vorhabens per POST-Request und generiert mithilfe eines LangChain-AI-Agenten (OpenAI GPT-4o-mini) eine strukturierte, priorisierte ToDo-Liste auf Deutsch — mit Kategorien und optionalen Notizen/Tipps je nach gewählter Detailtiefe.

---

## Flow-Übersicht

```
Webhook → PrepareInput → AIAgent → ValidateOutput → Response
                            ↑
              ChatModel (ai_languageModel)
              OutputParser (ai_outputParser)
```

---

## Nodes

### 1. Webhook

| Eigenschaft | Wert |
|---|---|
| **Name** | `Webhook` |
| **Type** | `n8n-nodes-base.webhook` |
| **Version** | 2 |
| **Position** | 416, -48 |
| **HTTP-Methode** | POST |
| **Pfad** | `/generate-todo-ai` |
| **Response-Modus** | `responseNode` |
| **Allowed Origins** | `*` |
| **Webhook-ID** | `e7f7d25d-4bfb-4c50-9c77-31e4067ab0d1` |

**Hinweis:** Empfängt POST-Request mit folgendem Body:
```json
{
  "listId": "string",
  "description": "string – freie Beschreibung des Vorhabens",
  "count": "number – gewünschte Anzahl Aufgaben",
  "detail": "kurz | mittel | detailliert",
  "timestamp": "ISO string"
}
```

---

### 2. PrepareInput

| Eigenschaft | Wert |
|---|---|
| **Name** | `PrepareInput` |
| **Type** | `n8n-nodes-base.code` |
| **Version** | 2 |
| **Position** | 608, -48 |
| **Sprache** | JavaScript |

**Aufgabe:** Liest die Eingabedaten aus dem Webhook-Body, wählt anhand des `detail`-Feldes die passende Detailtiefe-Anweisung und baut daraus einen LLM-Prompt.

**Detailtiefe-Anweisungen:**

| Wert | Beschreibung |
|---|---|
| `kurz` | Kurze, prägnante Aufgaben (max 6 Wörter pro Aufgabe, keine Notizen) |
| `mittel` | Klare Aufgaben mit kurzer Notiz als Hilfestellung (1 Satz) |
| `detailliert` | Ausführliche Aufgaben mit konkreten Tipps, Tools und Zeitschätzungen als Notiz |

**Output:** `{ prompt: string, listId: string }`

---

### 3. AIAgent

| Eigenschaft | Wert |
|---|---|
| **Name** | `AIAgent` |
| **Type** | `@n8n/n8n-nodes-langchain.agent` |
| **Version** | 1.7 |
| **Position** | 800, -48 |
| **Prompt-Typ** | `define` |
| **Text** | `={{ $json.prompt }}` |
| **Output-Parser** | aktiviert |

**System-Message:**
> Du bist ein erfahrener Projektmanager und Produktivitäts-Experte. Du erstellst präzise, handlungsorientierte ToDo-Listen die wirklich umsetzbar sind. Keine vagen Aufgaben, keine Redundanzen. Antworte AUSSCHLIESSLICH mit validem JSON – kein Markdown, keine Erklärungen, keine Backticks.

**Sub-Nodes (Connections):**
- `ChatModel` → als `ai_languageModel`
- `OutputParser` → als `ai_outputParser`

---

### 4. ChatModel

| Eigenschaft | Wert |
|---|---|
| **Name** | `ChatModel` |
| **Type** | `@n8n/n8n-nodes-langchain.lmChatOpenAi` |
| **Version** | 1 |
| **Position** | 800, 176 |
| **Modell** | `gpt-4o-mini` |
| **Max Tokens** | 1500 |
| **Temperature** | 0.6 |
| **Credential** | `OpenAiISO` |
| **Connection** | `ai_languageModel` → AIAgent |

---

### 5. OutputParser

| Eigenschaft | Wert |
|---|---|
| **Name** | `OutputParser` |
| **Type** | `@n8n/n8n-nodes-langchain.outputParserStructured` |
| **Version** | 1.2 |
| **Position** | 944, 176 |
| **Connection** | `ai_outputParser` → AIAgent |

**JSON-Schema (Beispiel):**
```json
{
  "title": "Kurzer Listentitel",
  "todos": [
    {
      "text": "Aufgabe im Imperativ",
      "note": "Hilfreiche Notiz oder Tipp",
      "priority": "high",
      "category": "Planung"
    }
  ]
}
```

---

### 6. ValidateOutput

| Eigenschaft | Wert |
|---|---|
| **Name** | `ValidateOutput` |
| **Type** | `n8n-nodes-base.code` |
| **Version** | 2 |
| **Position** | 1136, -48 |
| **Sprache** | JavaScript |

**Aufgabe:** Validiert und normalisiert den AI-Output:
- Liest `output`, `text`, `response` oder `message.content` aus dem Agenten-Ergebnis
- Bereinigt ggf. Markdown-Backticks und parst JSON
- Stellt sicher, dass `todos` ein Array ist und jeder Eintrag die Pflichtfelder enthält
- Normalisiert `priority` auf `high` / `medium` / `low` (Fallback: `medium`)
- Setzt `category` auf `Allgemein` wenn kein Wert vorhanden
- Filtert leere Aufgaben heraus
- Gibt `listId` aus dem `PrepareInput`-Node wieder mit durch

**Output:**
```json
{
  "title": "string",
  "todos": [
    {
      "text": "string",
      "note": "string",
      "priority": "high | medium | low",
      "category": "string"
    }
  ],
  "listId": "string"
}
```

---

### 7. Response

| Eigenschaft | Wert |
|---|---|
| **Name** | `Response` |
| **Type** | `n8n-nodes-base.respondToWebhook` |
| **Version** | 1 |
| **Position** | 1328, -48 |
| **Respond With** | JSON |
| **Response Code** | 200 |

**Response-Headers:**

| Header | Wert |
|---|---|
| `Content-Type` | `application/json` |
| `Access-Control-Allow-Origin` | `*` |

---

## Verbindungen (Connections)

| Von | Nach | Connection-Typ |
|---|---|---|
| Webhook | PrepareInput | main |
| PrepareInput | AIAgent | main |
| ChatModel | AIAgent | ai_languageModel |
| OutputParser | AIAgent | ai_outputParser |
| AIAgent | ValidateOutput | main |
| ValidateOutput | Response | main |

---

## Einstellungen

| Eigenschaft | Wert |
|---|---|
| **Execution Order** | v1 |
| **Binary Mode** | separate |
| **Available in MCP** | false |
