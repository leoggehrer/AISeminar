# Workflow: SimpleNoteAgent

| Eigenschaft | Wert |
|---|---|
| **ID** | `WUBlzoKHJ670yCY5` |
| **Status** | Inaktiv |
| **Erstellt** | 2026-03-25 |
| **Zuletzt geändert** | 2026-03-25 |

## Beschreibung

Einfacher Chat-basierter Notiz-Agent: Empfängt eine Freitexteingabe über das Chat-Interface, leitet sie direkt an einen LangChain-AI-Agenten (OpenAI GPT-4.1-mini) weiter und generiert daraus eine strukturierte Beschreibung auf Deutsch. Eine einfache In-Memory-Konversationshistorie ermöglicht kontextbezogene Nachfragen im selben Chat-Verlauf.

---

## Flow-Übersicht

```
Chat → PrepareInput → AIAgent
                         ↑
          ChatModel (ai_languageModel)
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
| **Position** | 352, 800 |
| **Webhook-ID** | `357e68f5-5dd3-49a1-bccf-2dd56c20ee70` |

**Hinweis:** Startet den Workflow über das n8n-eigene Chat-Interface. Die Eingabe des Nutzers steht unter `$json.chatInput` zur Verfügung.

---

### 2. PrepareInput

| Eigenschaft | Wert |
|---|---|
| **Name** | `PrepareInput` |
| **Type** | `n8n-nodes-base.code` |
| **Version** | 2 |
| **Position** | 560, 800 |
| **Sprache** | JavaScript |

**Aufgabe:** Liest die Chat-Eingabe direkt aus und bereitet das Input-Objekt für den AI-Agenten vor. Kategorie und Schlagwörter werden mit Standardwerten vorbelegt.

**Code:**
```js
let prompt = $input.first().json.chatInput;
let keywords = [];
let category = 'allgemein';
return [{ json: { prompt, keywords, category } }];
```

**Output:** `{ prompt: string, keywords: [], category: 'allgemein' }`

---

### 3. AIAgent

| Eigenschaft | Wert |
|---|---|
| **Name** | `AIAgent` |
| **Type** | `@n8n/n8n-nodes-langchain.agent` |
| **Version** | 1.7 |
| **Position** | 768, 800 |
| **Prompt-Typ** | `define` |
| **Text** | `={{ $json.prompt }}` |
| **Output-Parser** | deaktiviert |

**System-Message:**
```text
Du bist ein erfahrener Redakteur und Fachautor. Deine Aufgabe ist es, aus stichwortartigen Eingaben klare, ausführliche und gut strukturierte Beschreibungen auf Deutsch zu erstellen. Du antwortest AUSSCHLIESSLICH mit validem JSON - kein Erklaerungstext, keine Markdown-Backticks.

# KATEGORIEN
 - technisch: Präzise, sachlich, fachlich fundiert
 - kreativ: Lebendig, bildhaft, inspirierend
 - geschaeftlich: Professionell, nutzenorientiert, formell
 - allgemein: Verständlich, ausgewogen, informativ

# REGELN
 - Alle Stichwörter sinnvoll in die Beschreibung einbinden
 - Zusammenhänge zwischen den Stichwörtern herstellen
 - Fehlende Kontextinformationen logisch ergänzen
 - Sachlich korrekt bleiben, keine Erfindungen
 - Vermeide eine Anrede mit DU oder Hey
````

**Sub-Nodes (Connections):**
- `ChatModel` → als `ai_languageModel`
- `Simple Memory` → als `ai_memory`

---

### 4. ChatModel

| Eigenschaft | Wert |
|---|---|
| **Name** | `ChatModel` |
| **Type** | `@n8n/n8n-nodes-langchain.lmChatOpenAi` |
| **Version** | 1 |
| **Position** | 768, 1024 |
| **Modell** | `gpt-4.1-mini` |
| **Max Tokens** | 800 |
| **Temperature** | 0.7 |
| **Credential** | `OpenAiISO` |
| **Connection** | `ai_languageModel` → AIAgent |

---

### 5. Simple Memory

| Eigenschaft | Wert |
|---|---|
| **Name** | `Simple Memory` |
| **Type** | `@n8n/n8n-nodes-langchain.memoryBufferWindow` |
| **Version** | 1.3 |
| **Position** | 880, 1024 |
| **Connection** | `ai_memory` → AIAgent |

**Hinweis:** Hält den bisherigen Chat-Verlauf im Arbeitsspeicher (Buffer Window Memory). Ermöglicht kontextbezogene Folgefragen innerhalb einer Sitzung. Die Daten sind nicht persistent und gehen beim Neustart verloren.

---

## Verbindungen (Connections)

| Von | Nach | Connection-Typ |
|---|---|---|
| Chat | PrepareInput | main |
| PrepareInput | AIAgent | main |
| ChatModel | AIAgent | ai_languageModel |
| Simple Memory | AIAgent | ai_memory |

---

## Einstellungen

| Eigenschaft | Wert |
|---|---|
| **Execution Order** | v1 |
| **Binary Mode** | separate |
| **Available in MCP** | false |

## Beispiele

```text
Schüler Hans Moser macht keine Aufgabe, ist unpünktlich und ständig am Handy;
```

```text
Schüler Gerhard Gehrer hat unentschuldigte Fehlstunden über 30; Androhung Abmeldung; Klassenbucheintrag
```