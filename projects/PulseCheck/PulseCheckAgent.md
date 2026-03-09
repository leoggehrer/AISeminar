# Workflow: PulseCheckAgent

| Eigenschaft | Wert |
|---|---|
| **ID** | `mk1afsl4vLmTHSm5` |
| **Status** | Aktiv |
| **Erstellt** | 2026-03-08 |
| **Zuletzt geändert** | 2026-03-08 |

## Beschreibung

Empfängt Kundenfeedback per POST-Request, analysiert es mithilfe eines LangChain-AI-Agenten (OpenAI GPT-4.1-mini) und gibt Sentiment, relevante Keywords sowie eine kurze Zusammenfassung auf Deutsch als JSON zurück.

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
| **Position** | 208, -144 |
| **HTTP-Methode** | POST |
| **Pfad** | `/rate-feedback-ai` |
| **Response-Modus** | `responseNode` |
| **Allowed Origins** | `*` |
| **Webhook-ID** | `9154b2a5-756f-49b0-8c8f-f3ac02a16ea2` |

**Hinweis:** Empfängt POST-Request mit folgendem Body:
```json
{
  "feedback": {
    "id": "number",
    "author": "string",
    "category": "product | service | support | general",
    "text": "string",
    "rating": "1-5"
  },
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
| **Position** | 416, -144 |
| **Sprache** | JavaScript |

**Aufgabe:** Liest die Feedback-Daten aus dem Webhook-Body und baut daraus einen LLM-Prompt mit den Sentiment-Regeln.

**Sentiment-Regeln (im Prompt):**

| Sentiment | Bedingung |
|---|---|
| `positive` | Bewertung 4–5 UND überwiegend positiver Ton |
| `negative` | Bewertung 1–2 ODER starke Kritik/Frustration |
| `neutral` | Bewertung 3 oder gemischte Aussagen |

**Output:** `{ prompt: string, feedbackId: number }`

---

### 3. AIAgent

| Eigenschaft | Wert |
|---|---|
| **Name** | `AIAgent` |
| **Type** | `@n8n/n8n-nodes-langchain.agent` |
| **Version** | 1.7 |
| **Position** | 608, -144 |
| **Prompt-Typ** | `define` |
| **Text** | `={{ $json.prompt }}` |
| **Output-Parser** | aktiviert |

**System-Message:**
> Du bist ein präziser Sentiment-Analyse Assistent für Kundenfeedback. Analysiere den Text, klassifiziere das Sentiment, extrahiere relevante Keywords und erstelle eine kurze Zusammenfassung auf Deutsch. Antworte AUSSCHLIESSLICH mit validem JSON – kein Erklärungstext, keine Markdown-Backticks.

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
| **Position** | 608, 112 |
| **Modell** | `gpt-4.1-mini` |
| **Max Tokens** | 300 |
| **Temperature** | 0.1 |
| **Credential** | `OpenAiISO` |
| **Connection** | `ai_languageModel` → AIAgent |

---

### 5. OutputParser

| Eigenschaft | Wert |
|---|---|
| **Name** | `OutputParser` |
| **Type** | `@n8n/n8n-nodes-langchain.outputParserStructured` |
| **Version** | 1.2 |
| **Position** | 752, 112 |
| **Connection** | `ai_outputParser` → AIAgent |

**JSON-Schema (Beispiel):**
```json
{
  "sentiment": "positive",
  "keywords": ["Schlüsselwort 1", "Schlüsselwort 2"],
  "summary": "Ein Satz Zusammenfassung auf Deutsch."
}
```

---

### 6. ValidateOutput

| Eigenschaft | Wert |
|---|---|
| **Name** | `ValidateOutput` |
| **Type** | `n8n-nodes-base.code` |
| **Version** | 2 |
| **Position** | 944, -144 |
| **Sprache** | JavaScript |

**Aufgabe:** Validiert und normalisiert den AI-Output:
- Liest `output`, `text`, `response` oder `message.content` aus dem Agenten-Ergebnis
- Bereinigt ggf. Markdown-Backticks und parst JSON
- Stellt sicher, dass `sentiment` ein gültiger Wert (`positive` / `neutral` / `negative`) ist
- Stellt sicher, dass `keywords` ein Array (max. 5 Einträge, alle Strings) ist
- Stellt sicher, dass `summary` ein String ist
- Gibt `feedbackId` aus dem `PrepareInput`-Node wieder mit durch

**Output:** `{ sentiment: string, keywords: string[], summary: string, feedbackId: number }`

---

### 7. Response

| Eigenschaft | Wert |
|---|---|
| **Name** | `Response` |
| **Type** | `n8n-nodes-base.respondToWebhook` |
| **Version** | 1 |
| **Position** | 1136, -144 |
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
