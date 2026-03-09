# Workflow: ProductCatalogAgent

| Eigenschaft | Wert |
|---|---|
| **ID** | `Euutsl9TuwzcLsDS` |
| **Status** | Inaktiv |
| **Erstellt** | 2026-03-07 |
| **Zuletzt geändert** | 2026-03-07 |

## Beschreibung

Empfängt Produktdaten per POST-Request, generiert mithilfe eines LangChain-AI-Agenten (OpenAI GPT-4.1-mini) eine verkaufsstarke Produktbeschreibung auf Deutsch sowie passende Schlagwörter (Tags) — im gewünschten Tonfall (luxury / casual / technical) — und gibt das Ergebnis als JSON zurück.

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
| **Position** | 640, -64 |
| **HTTP-Methode** | POST |
| **Pfad** | `/product-description-ai` |
| **Response-Modus** | `responseNode` |
| **Allowed Origins** | `*` |
| **Webhook-ID** | `761578e1-61b3-49e0-a733-739ad2a60944` |

**Hinweis:** Empfängt POST-Request mit folgendem Body:
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

---

### 2. PrepareInput

| Eigenschaft | Wert |
|---|---|
| **Name** | `PrepareInput` |
| **Type** | `n8n-nodes-base.code` |
| **Version** | 2 |
| **Position** | 864, -64 |
| **Sprache** | JavaScript |

**Aufgabe:** Liest die Produktdaten aus dem Webhook-Body, wählt anhand des `tone`-Feldes eine Ton-Anweisung und baut daraus einen strukturierten LLM-Prompt.

**Ton-Anweisungen:**

| Ton | Beschreibung |
|---|---|
| `luxury` | Elegant, exklusiv, gehobene Sprache |
| `casual` | Locker, sympathisch, direkte Anrede (du) |
| `technical` | Technisch präzise, Spezifikationen & Fakten |

**Output:** `{ prompt: string, productId: string }`

---

### 3. AIAgent

| Eigenschaft | Wert |
|---|---|
| **Name** | `AIAgent` |
| **Type** | `@n8n/n8n-nodes-langchain.agent` |
| **Version** | 1.7 |
| **Position** | 1056, -64 |
| **Prompt-Typ** | `define` |
| **Text** | `={{ $json.prompt }}` |
| **Output-Parser** | aktiviert |

**System-Message:**
> Du bist ein erfahrener E-Commerce Texter. Du generierst prägnante, verkaufsstarke Produktbeschreibungen auf Deutsch. Du antwortest AUSSCHLIESSLICH mit validem JSON – kein Erklärungstext, keine Markdown-Backticks. Vermeide eine Anrede mit DU oder Hey.

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
| **Position** | 1056, 160 |
| **Modell** | `gpt-4.1-mini` |
| **Max Tokens** | 400 |
| **Temperature** | 0.7 |
| **Credential** | `OpenAiISO` |
| **Connection** | `ai_languageModel` → AIAgent |

---

### 5. OutputParser

| Eigenschaft | Wert |
|---|---|
| **Name** | `OutputParser` |
| **Type** | `@n8n/n8n-nodes-langchain.outputParserStructured` |
| **Version** | 1.2 |
| **Position** | 1200, 160 |
| **Connection** | `ai_outputParser` → AIAgent |

**JSON-Schema (Beispiel):**
```json
{
  "description": "2-3 Sätze Produktbeschreibung auf Deutsch. Kein Markdown.",
  "tags": ["Schlagwort 1", "Schlagwort 2", "Schlagwort 3"]
}
```

---

### 6. ValidateOutput

| Eigenschaft | Wert |
|---|---|
| **Name** | `ValidateOutput` |
| **Type** | `n8n-nodes-base.code` |
| **Version** | 2 |
| **Position** | 1360, -64 |
| **Sprache** | JavaScript |

**Aufgabe:** Validiert und normalisiert den AI-Output:
- Liest `output`, `text`, `response` oder `message.content` aus dem Agenten-Ergebnis
- Bereinigt ggf. Markdown-Backticks und parst JSON
- Stellt sicher, dass `description` ein String und `tags` ein Array (max. 5 Einträge) ist
- Gibt `productId` aus dem `PrepareInput`-Node wieder mit durch

**Output:** `{ description: string, tags: string[], productId: string }`

---

### 7. Response

| Eigenschaft | Wert |
|---|---|
| **Name** | `Response` |
| **Type** | `n8n-nodes-base.respondToWebhook` |
| **Version** | 1 |
| **Position** | 1536, -64 |
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
