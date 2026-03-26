# Workflow: CreditCheckerAgent

| Eigenschaft | Wert |
|---|---|
| **ID** | `P0MoHhCGyVjZNjMk` |
| **Status** | Inaktiv |
| **Erstellt** | 2026-03-08 |
| **Zuletzt geändert** | 2026-03-09 |

## Beschreibung

Interaktiver Chat-Workflow für Kreditprüfungen. Ein AI-Agent führt ein Gespräch mit dem Nutzer, sammelt alle nötigen Informationen und ruft anschließend über einen MCP-Client das `CheckCredit`-Tool auf einem lokalen Backend-Server auf. Der Agent behält den Gesprächsverlauf über einen Memory-Buffer.

---

## Flow-Übersicht

```
Chat → AI Agent
          ↑
ChatModel   (ai_languageModel)
MCP Client  (ai_tool)
Simple Memory (ai_memory)
```

---

## Nodes

### 1. Chat

| Eigenschaft | Wert |
|---|---|
| **Name** | `Chat` |
| **Type** | `@n8n/n8n-nodes-langchain.chatTrigger` |
| **Version** | 1.4 |
| **Position** | 0, 0 |
| **Webhook-ID** | `fe4f8ed1-8e9b-4471-9e83-2ba333cf6bb3` |

**Aufgabe:** Startet den Workflow bei eingehender Chat-Nachricht und leitet diese an den AI Agent weiter.

---

### 2. AI Agent

| Eigenschaft | Wert |
|---|---|
| **Name** | `AI Agent` |
| **Type** | `@n8n/n8n-nodes-langchain.agent` |
| **Version** | 3.1 |
| **Position** | 208, 0 |

**System-Message:**
> Du bist ein Kreditberater. Nutze das CheckCredit-Tool um Kreditanfragen zu prüfen. Frage den Nutzer nach allen nötigen Informationen bevor du das Tool aufrufst.

**Sub-Nodes (Connections):**
- `ChatModel` → als `ai_languageModel`
- `MCP Client` → als `ai_tool`
- `Simple Memory` → als `ai_memory`

---

### 3. ChatModel

| Eigenschaft | Wert |
|---|---|
| **Name** | `ChatModel` |
| **Type** | `@n8n/n8n-nodes-langchain.lmChatOpenAi` |
| **Version** | 1.3 |
| **Position** | 208, 224 |
| **Modell** | `gpt-4.1-mini` |
| **Responses API** | deaktiviert |
| **Credential** | `OpenAiISO` |
| **Connection** | `ai_languageModel` → AI Agent |

---

### 4. MCP Client

| Eigenschaft | Wert |
|---|---|
| **Name** | `MCP Client` |
| **Type** | `@n8n/n8n-nodes-langchain.mcpClientTool` |
| **Version** | 1.2 |
| **Position** | 352, 224 |
| **Endpoint URL** | `http://localhost:5250/mcp` |
| **Connection** | `ai_tool` → AI Agent |

**Aufgabe:** Stellt dem AI Agent das `CheckCredit`-Tool über das Model Context Protocol (MCP) bereit. Der Agent ruft dieses Tool auf, sobald alle nötigen Informationen vom Nutzer gesammelt wurden.

---

### 5. Simple Memory

| Eigenschaft | Wert |
|---|---|
| **Name** | `Simple Memory` |
| **Type** | `@n8n/n8n-nodes-langchain.memoryBufferWindow` |
| **Version** | 1.3 |
| **Position** | 304, 368 |
| **Connection** | `ai_memory` → AI Agent |

**Aufgabe:** Speichert den Gesprächsverlauf im Arbeitsspeicher (sliding window buffer), sodass der Agent vorherige Nachrichten im Kontext behält.

---

## Verbindungen (Connections)

| Von | Nach | Connection-Typ |
|---|---|---|
| Chat | AI Agent | main |
| ChatModel | AI Agent | ai_languageModel |
| MCP Client | AI Agent | ai_tool |
| Simple Memory | AI Agent | ai_memory |

---

## Einstellungen

| Eigenschaft | Wert |
|---|---|
| **Execution Order** | v1 |
| **Binary Mode** | separate |
| **Available in MCP** | false |

---

## Voraussetzungen

- Lokaler MCP-Server muss unter `http://localhost:5250/mcp` erreichbar sein und das Tool `CheckCredit` bereitstellen
- OpenAI Credential `OpenAiISO` muss in n8n konfiguriert sein
