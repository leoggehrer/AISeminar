# Workflow: FlowerShopChatWithVectorSimple

| Eigenschaft                 | Wert                 |
| --------------------------- | -------------------- |
| **ID**                | `CGFlHrOTROvMAR4M` |
| **Status**            | Inaktiv              |
| **Erstellt**          | 2026-03-22           |
| **Zuletzt geändert** | 2026-03-22           |

## Beschreibung

KI-gestützter Chat-Assistent für den FlowerShop. Nimmt Benutzeranfragen über einen Chat-Trigger entgegen, bereitet die Eingabedaten auf und leitet sie an einen LangChain-Agenten weiter. Der Agent verwendet `gpt-4.1-mini` als Sprachmodell, speichert den Gesprächsverlauf in einer PostgreSQL-Datenbank (`ChatMemory`) und hat Zugriff auf einen PGVector-Wissensspeicher mit vordefinierten Blumenkonfigurationen (`flowershop_doc_vectors`). Der Agent führt den Benutzer durch den Bestellprozess: Produktempfehlung, Verfügbarkeitsprüfung (`FlowerWarehouse`), Bestellerfassung (`InsertOrder`) und Bestätigungs-Email (`SendOrder`).

---

## Flow-Übersicht

```
Chat → SetInputData → AI Agent
                          ↑              ↑             ↑
                     ChatModel     ChatMemory    PGVectorStore
                    (ai_languageModel) (ai_memory)  (ai_tool)
                                                        ↑
                                               OpenAI Embeddings
                                                  (ai_embedding)
```

---

## Nodes

### 1. Chat

| Eigenschaft        | Wert                                              |
| ------------------ | ------------------------------------------------- |
| **Name**     | `Chat`                                          |
| **Type**     | `@n8n/n8n-nodes-langchain.chatTrigger`          |
| **Version**  | 1.4                                               |
| **Position** | 9840, 1600                                        |
| **Webhook-ID** | `b00f8884-b897-40ff-98f5-0e2f80d16d7a`        |

**Aufgabe:** Startet den Workflow bei jeder eingehenden Chat-Nachricht. Liefert `chatInput` und `sessionId` an den nächsten Node.

---

### 2. SetInputData

| Eigenschaft        | Wert                    |
| ------------------ | ----------------------- |
| **Name**     | `SetInputData`        |
| **Type**     | `n8n-nodes-base.code` |
| **Version**  | 2                       |
| **Position** | 10064, 1600             |
| **Sprache**  | JavaScript              |

**Aufgabe:** Bereitet die Eingabedaten für den AI Agent auf. Generiert eine neue GUID, übernimmt `sessionId` und `chatInput` aus dem Chat-Trigger und setzt die `email`-Variable auf einen leeren String (wird später vom Agenten erfragt).

**JavaScript-Code:**

```javascript
function generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

return {
  "guid": generateGuid(),
  "sessionId": $input.first().json.sessionId,
  "chatInput": ($input.first().json.chatInput),
  "email": ''
};
```

**Output-Felder:**

| Feld          | Beschreibung                                  |
| ------------- | --------------------------------------------- |
| `guid`      | Neu generierte UUID (v4-Format)               |
| `sessionId` | Session-ID aus dem Chat-Trigger               |
| `chatInput` | Eingabetext des Benutzers                     |
| `email`     | E-Mail-Adresse (initial leer)                 |

---

### 3. AI Agent

| Eigenschaft        | Wert                                    |
| ------------------ | --------------------------------------- |
| **Name**     | `AI Agent`                            |
| **Type**     | `@n8n/n8n-nodes-langchain.agent`      |
| **Version**  | 3                                       |
| **Position** | 10416, 1600                             |
| **Prompt**   | `define`                              |

**User-Prompt-Template:**

```
Aktuelle Zeit: {{ $now }}
Email: {{ $json.email ?? '' }}

{{ $json.chatInput }}
```

**Verbundene Sub-Nodes:**

| Sub-Node           | Connection-Typ      |
| ------------------ | ------------------- |
| `ChatModel`      | `ai_languageModel` |
| `ChatMemory`     | `ai_memory`        |
| `PGVectorStore`  | `ai_tool`          |

**Verwendete Tools (aus System-Prompt):**

| Tool               | Funktion                                           |
| ------------------ | -------------------------------------------------- |
| `PGVectorStore`  | Wissensabfrage für Blumenempfehlungen nach Anlass |
| `FlowerWarehouse`| Lagerbestandsprüfung je Artikel                  |
| `InsertOrder`    | Bestellung in der Datenbank anlegen               |
| `SendOrder`      | Bestätigungs-Email an den Kunden versenden        |

**System-Prompt (Zusammenfassung):**

Der Agent agiert als freundlicher Flower-Assistent für den FlowerShop. Die wichtigsten Verhaltensregeln:

- **Sprache & Ton:** Deutsch, freundlich, professionell, keine technischen Details
- **Verbotene Informationen:** Einkaufspreise, Lagerbestände (`ProductStock`), Tool-Namen, Datenbankstrukturen
- **Begrüßung:** Tageszeit-abhängig (06:00–17:59 Uhr: „Guten Tag", 18:00–05:59 Uhr: „Guten Abend")

**Bestellprozess (4 Schritte):**

| Schritt | Aktion                                                                                          |
| ------- | ----------------------------------------------------------------------------------------------- |
| 1       | Bestellübersicht erstellen (Artikelnummer, Menge, Einzelpreis, Gesamtbetrag)                   |
| 2       | `FlowerWarehouse` aufrufen – Lagerbestand je Artikel prüfen (PFLICHT vor Bestätigung)         |
| 3       | E-Mail-Adresse erfragen und validieren (Format: `@` + `.` erforderlich)                       |
| 4       | Bestellübersicht zeigen und vom Benutzer bestätigen lassen                                      |

**Bestellabschluss (Reihenfolge zwingend):**

1. `InsertOrder` aufrufen mit: `CartNumber` (Format `ORD-YYYYMMDD-HHMMSS`), `Email`, `GuestName`, `TotalAmount`, `Notes`, `OrderDate`
2. `SendOrder` aufrufen mit: `To`, `Subject` (`FlowerShop Bestellung vom [DD.MM.YYYY]`), `Message` (HTML-Template mit Bestellübersicht)

**Fehlerbehandlung:**

| Fehlertyp                        | Aktion                                                                                    |
| -------------------------------- | ----------------------------------------------------------------------------------------- |
| `FlowerWarehouse`: nicht gefunden | „Diese Artikelnummer konnte ich nicht finden. Bitte überprüfen."                         |
| `FlowerWarehouse`: zu wenig Lager | „Leider ist dieser Artikel aktuell nur begrenzt verfügbar." (Lagermenge nie nennen!)     |
| `InsertOrder` fehlgeschlagen     | STOPP – keine Email senden, Benutzer informieren                                          |
| `SendOrder` fehlgeschlagen       | Benutzer informieren, erneuten Versand anbieten                                           |
| Ungültige Artikelnummer          | „Diese Artikelnummer konnte ich nicht finden. Bitte überprüfen."                         |
| Leere Bestellung                 | „Ihre Bestellung enthält noch keine Artikel. Was darf ich notieren?"                     |

---

### 4. ChatModel

| Eigenschaft          | Wert                                        |
| -------------------- | ------------------------------------------- |
| **Name**       | `ChatModel`                               |
| **Type**       | `@n8n/n8n-nodes-langchain.lmChatOpenAi`  |
| **Version**    | 1.3                                         |
| **Position**   | 10032, 2128                                 |
| **Modell**     | `gpt-4.1-mini`                            |
| **Credential** | `OpenAiISO`                               |
| **Connection** | `ai_languageModel` → AI Agent            |

**Aufgabe:** Stellt das Sprachmodell für den AI Agent bereit. Verwendet `gpt-4.1-mini` über die OpenAI-API.

---

### 5. ChatMemory

| Eigenschaft          | Wert                                                  |
| -------------------- | ----------------------------------------------------- |
| **Name**       | `ChatMemory`                                        |
| **Type**       | `@n8n/n8n-nodes-langchain.memoryPostgresChat`       |
| **Version**    | 1.3                                                   |
| **Position**   | 10192, 2128                                           |
| **Session-Key** | `={{ $json.sessionId }}`                           |
| **Credential** | `Postgres`                                          |
| **Connection** | `ai_memory` → AI Agent                            |

**Aufgabe:** Speichert und lädt den Gesprächsverlauf sitzungsbasiert aus einer PostgreSQL-Datenbank. Der `sessionKey` wird dynamisch aus der `sessionId` des aktuellen Requests gesetzt, sodass jede Chat-Sitzung einen eigenen Verlauf erhält.

---

### 6. PGVectorStore

| Eigenschaft              | Wert                                             |
| ------------------------ | ------------------------------------------------ |
| **Name**           | `PGVectorStore`                                |
| **Type**           | `@n8n/n8n-nodes-langchain.vectorStorePGVector` |
| **Version**        | 1.3                                              |
| **Position**       | 10480, 2144                                      |
| **Modus**          | `retrieve-as-tool`                             |
| **Tabelle**        | `flowershop_doc_vectors`                       |
| **Top K**          | 10                                               |
| **Credential**     | `PGVector`                                     |
| **Connection**     | `ai_tool` → AI Agent                          |

**Tool-Beschreibung (für den Agenten):**

> Durchsuche die Wissensdatenbank nach Blumenempfehlungen und Konfigurationen für besondere Anlässe.
>
> Verwende dieses Tool, wenn der Kunde nach Empfehlungen fragt für:
> - Hochzeiten (Brautsträuße, Tischdeko, Kirchenschmuck)
> - Geburtstage und Jubiläen
> - Trauerfeiern und Beerdigungen
> - Valentinstag und Muttertag
> - Firmenfeiern und Geschäftseröffnungen
> - Saisonale Anlässe (Ostern, Weihnachten)

**Aufgabe:** Führt eine semantische Ähnlichkeitssuche in der Vektordatenbank durch und gibt die 10 relevantesten Einträge zurück. Wird vom AI Agent als Tool aufgerufen, wenn Produktempfehlungen benötigt werden.

---

### 7. OpenAI Embeddings

| Eigenschaft          | Wert                                          |
| -------------------- | --------------------------------------------- |
| **Name**       | `OpenAI Embeddings`                         |
| **Type**       | `@n8n/n8n-nodes-langchain.embeddingsOpenAi` |
| **Version**    | 1.2                                           |
| **Position**   | 10480, 2288                                   |
| **Credential** | `OpenAiISO`                                 |
| **Connection** | `ai_embedding` → PGVectorStore             |

**Aufgabe:** Erzeugt Embedding-Vektoren für die Suchanfrage des PGVectorStore, um die semantische Ähnlichkeitssuche in `flowershop_doc_vectors` durchführen zu können.

---

## Verbindungen (Connections)

| Von                  | Nach             | Connection-Typ      |
| -------------------- | ---------------- | ------------------- |
| Chat                 | SetInputData     | main                |
| SetInputData         | AI Agent         | main                |
| ChatModel            | AI Agent         | ai_languageModel    |
| ChatMemory           | AI Agent         | ai_memory           |
| PGVectorStore        | AI Agent         | ai_tool             |
| OpenAI Embeddings    | PGVectorStore    | ai_embedding        |

---

## Einstellungen

| Eigenschaft               | Wert     |
| ------------------------- | -------- |
| **Execution Order** | v1       |
| **Binary Mode**     | separate |
| **Available in MCP**| false    |

---

## Sticky Notes

| Name            | Farbe | Beschreibung                                                                       |
| --------------- | ----- | ---------------------------------------------------------------------------------- |
| `Sticky Note2` | 4     | „Chat zum Testen innerhalb von n8n"                                               |
| `Sticky Note5` | 3     | „Chat Model und Memory"                                                            |
| `Sticky Note7` | 7     | „Wissensabfrage zu vordefinierten Konfigurationen. Blumensträuße für Anlässe."   |
| `Sticky Note8` | –     | „Aufbereitung der Verarbeitungsdaten"                                              |
