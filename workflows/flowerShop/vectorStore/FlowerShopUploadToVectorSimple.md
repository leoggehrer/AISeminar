# Workflow: FlowerShopUploadToVectorSimple

| Eigenschaft                 | Wert                 |
| --------------------------- | -------------------- |
| **ID**                | `X4nHx3FOgR22YlQN` |
| **Status**            | Inaktiv              |
| **Erstellt**          | 2026-03-22           |
| **Zuletzt geändert** | 2026-03-22           |

## Beschreibung

Vereinfachte Variante von `FlowerShopUploadToVector`. Ermöglicht den Upload einer strukturierten Konfigurationsdatei (z. B. Blumensträuße mit Artikeln, Preisen, Tags) über ein Webformular. Die Datei wird geparst, in einzelne Produktblöcke aufgeteilt, vektorisiert (OpenAI Embeddings) und in eine PostgreSQL-Vektordatenbank (`flowerShop_doc_vectors`) geschrieben. Im Unterschied zur komplexen Variante entfällt die Prüfung, ob die Zieltabelle existiert — der DELETE-Schritt wird immer ausgeführt.

---

## Flow-Übersicht

```
FormUpload → ExtractFromFile → SetFileId → DeleteExistItems → SplitConfigBlocks
                                                                       ↓
                                                            ParseConfigBlocks → PGVectorStore
                                                                                    ↑
                                                                      Embeddings (ai_embedding)
                                                                      DefaultDataLoader (ai_document)
                                                                           ↑
                                                               CharacterTextSplitter (ai_textSplitter)
```

---

## Nodes

### 1. FormUpload

| Eigenschaft              | Wert                                                                                  |
| ------------------------ | ------------------------------------------------------------------------------------- |
| **Name**           | `FormUpload`                                                                        |
| **Type**           | `n8n-nodes-base.formTrigger`                                                        |
| **Version**        | 2.3                                                                                   |
| **Position**       | 1808, 2848                                                                            |
| **Formular-Titel** | `FlowerShop – Datei zu Vector`                                                     |
| **Beschreibung**   | Laden Sie eine Konfigurationsdatei hoch, um sie in den Vektorspeicher zu importieren. |
| **Webhook-ID**     | `13579e96-f6e7-4493-bfdb-342531d872cd`                                              |

**Formular-Felder:**

| Feld  | Typ  | Pflichtfeld |
| ----- | ---- | ----------- |
| Datei | file | ja          |

**Optionen:** Attribution deaktiviert (`appendAttribution: false`)

---

### 2. ExtractFromFile

| Eigenschaft               | Wert                               |
| ------------------------- | ---------------------------------- |
| **Name**            | `ExtractFromFile`                |
| **Type**            | `n8n-nodes-base.extractFromFile` |
| **Version**         | 1.1                                |
| **Position**        | 2032, 2848                         |
| **Operation**       | `text`                           |
| **Binary Property** | `Datei`                          |
| **Zielfeld**        | `text`                           |

**Aufgabe:** Extrahiert den Textinhalt aus der hochgeladenen Binärdatei und speichert ihn im Feld `text`.

---

### 3. SetFileId

| Eigenschaft        | Wert                   |
| ------------------ | ---------------------- |
| **Name**     | `SetFileId`          |
| **Type**     | `n8n-nodes-base.set` |
| **Version**  | 3.4                    |
| **Position** | 2256, 2848             |

**Zuweisungen:**

| Variable      | Wert                                                  | Typ    |
| ------------- | ----------------------------------------------------- | ------ |
| `file_id`   | `{{ $('FormUpload').item.json.Datei[0].filename }}` | string |
| `file_type` | `{{ $('FormUpload').item.json.Datei[0].mimetype }}` | string |

**Aufgabe:** Speichert Dateiname und MIME-Typ der hochgeladenen Datei für die spätere Verwendung als Metadaten und zur Duplikat-Erkennung.

---

### 4. DeleteExistItems

| Eigenschaft          | Wert                        |
| -------------------- | --------------------------- |
| **Name**       | `DeleteExistItems`        |
| **Type**       | `n8n-nodes-base.postgres` |
| **Version**    | 2.6                         |
| **Position**   | 2512, 2848                  |
| **Operation**  | `executeQuery`            |
| **Credential** | `PGVector`                |

**SQL-Query:**

```sql
DELETE FROM flowerShop_doc_vectors
WHERE metadata#>>'{file_id}'='{{$('SetFileId').item.json.file_id}}';
```

**Aufgabe:** Löscht alle bestehenden Vektoren, die zur aktuellen Datei gehören (anhand der `file_id` in den Metadaten). Verhindert Duplikate bei erneutem Upload derselben Datei. Wird im Gegensatz zur komplexen Variante immer ausgeführt — ohne vorherige Tabellenexistenzprüfung.

---

### 5. SplitConfigBlocks

| Eigenschaft        | Wert                    |
| ------------------ | ----------------------- |
| **Name**     | `SplitConfigBlocks`   |
| **Type**     | `n8n-nodes-base.code` |
| **Version**  | 2                       |
| **Position** | 2880, 2848              |
| **Sprache**  | JavaScript              |

**Aufgabe:** Splittet den extrahierten Gesamttext anhand des Trennzeichens `---` in einzelne Konfigurationsblöcke. Jeder Block wird als separates Item zurückgegeben.

**JavaScript-Code:**

```javascript
const fullText = $('ExtractFromFile').first().json.text;

const blocks = fullText
  .split('---')
  .map(b => b.trim())
  .filter(b => b.length > 0);

return blocks.map(block => ({ json: { block } }));
```

---

### 6. ParseConfigBlocks

| Eigenschaft        | Wert                    |
| ------------------ | ----------------------- |
| **Name**     | `ParseConfigBlocks`   |
| **Type**     | `n8n-nodes-base.code` |
| **Version**  | 2                       |
| **Position** | 3120, 2848              |
| **Sprache**  | JavaScript              |

**Aufgabe:** Parst jeden Konfigurationsblock und extrahiert die strukturierten Felder. Erstellt einen zusammengesetzten Text (`textForEmbedding`) für die Vektorisierung.

**JavaScript-Code:**

```javascript
const items = $input.all();
const output = [];

for (const item of items) {
  const block = item.json.block;

  const get = (key) => {
    const m = block.match(new RegExp(`^${key}:\\s*(.+)$`, 'm'));
    return m ? m[1].trim() : '';
  };

  const name = get('name');
  if (!name) continue;

  const priceCategory = get('kategorie');
  const price = parseFloat(get('preis')) || null;
  const tags = get('tags').split(',').map(t => t.trim()).filter(Boolean);
  const occasions = get('anlass').split(',').map(o => o.trim()).filter(Boolean);
  const description = get('beschreibung');

  // Artikel: "B1018 Sonnenblume 3×; B1075 Tulpe Gelb 5×"
  const articles = get('artikel').split(';').map(a => {
    const m = a.trim().match(/^(B\d+)\s+(.+?)\s+(\d+)×$/);
    return m ? { articleId: m[1], productName: m[2].trim(), quantity: parseInt(m[3], 10) } : null;
  }).filter(Boolean);

  const articleIds = articles.map(a => a.articleId);
  const articleSummary = articles.map(a => `${a.articleId} ${a.productName} (${a.quantity}×)`).join(', ');

  const textForEmbedding = `Name: ${name}. Preiskategorie: ${priceCategory}. Beschreibung: ${description}. Tags: ${tags.join(', ')}. Anlass: ${occasions.join(', ')}. Artikel: ${articleSummary}. Preis: ${price} Euro.`;

  output.push({
    json: { name, priceCategory, description, price, articles, articleIds, tags, occasions, textForEmbedding },
  });
}

return output;
```

**Geparste Felder:**

| Feld             | Beschreibung                                                           |
| ---------------- | ---------------------------------------------------------------------- |
| `name`         | Produktname (Pflichtfeld)                                              |
| `kategorie`    | Preiskategorie                                                         |
| `preis`        | Preis (als Float)                                                      |
| `tags`         | Kommagetrennte Schlagwörter                                           |
| `anlass`       | Kommagetrennte Anlässe                                                |
| `beschreibung` | Produktbeschreibung                                                    |
| `artikel`      | Artikelliste im Format `B1018 Sonnenblume 3×; B1075 Tulpe Gelb 5×` |

**Output-Format:**

```json
{
  "name": "string",
  "priceCategory": "string",
  "description": "string",
  "price": 0.00,
  "articles": [{ "articleId": "B1018", "productName": "Sonnenblume", "quantity": 3 }],
  "articleIds": ["B1018"],
  "tags": ["tag1", "tag2"],
  "occasions": ["Geburtstag"],
  "textForEmbedding": "Name: ... Preiskategorie: ... Beschreibung: ... Tags: ... Anlass: ... Artikel: ... Preis: ... Euro."
}
```

---

### 7. PGVectorStore

| Eigenschaft          | Wert                                             |
| -------------------- | ------------------------------------------------ |
| **Name**       | `PGVectorStore`                                |
| **Type**       | `@n8n/n8n-nodes-langchain.vectorStorePGVector` |
| **Version**    | 1.3                                              |
| **Position**   | 3392, 2848                                       |
| **Modus**      | `insert`                                       |
| **Tabelle**    | `flowerShop_doc_vectors`                       |
| **Credential** | `PGVector`                                     |

**Sub-Nodes (Connections):**

- `Embeddings` → als `ai_embedding`
- `DefaultDataLoader` → als `ai_document`

---

### 8. Embeddings

| Eigenschaft          | Wert                                          |
| -------------------- | --------------------------------------------- |
| **Name**       | `Embeddings`                                |
| **Type**       | `@n8n/n8n-nodes-langchain.embeddingsOpenAi` |
| **Version**    | 1.2                                           |
| **Position**   | 3264, 3216                                    |
| **Credential** | `OpenAiISO`                                 |
| **Connection** | `ai_embedding` → PGVectorStore             |

**Aufgabe:** Erzeugt Embedding-Vektoren (Standardmodell) für die Textdaten, die anschließend im PGVectorStore gespeichert werden.

---

### 9. DefaultDataLoader

| Eigenschaft              | Wert                                                   |
| ------------------------ | ------------------------------------------------------ |
| **Name**           | `DefaultDataLoader`                                  |
| **Type**           | `@n8n/n8n-nodes-langchain.documentDefaultDataLoader` |
| **Version**        | 1.1                                                    |
| **Position**       | 3488, 3216                                             |
| **JSON-Modus**     | `expressionData`                                     |
| **Datenquelle**    | `{{ $json.textForEmbedding }}`                       |
| **Text Splitting** | Custom (via CharacterTextSplitter)                     |
| **Connection**     | `ai_document` → PGVectorStore                       |

**Metadaten:**

| Name          | Wert                                          |
| ------------- | --------------------------------------------- |
| `file_id`   | `{{ $('SetFileId').first().json.file_id }}` |
| `name`      | `{{ $json.name }}`                          |
| `price`     | `{{ $json.price }}`                         |
| `articles`  | `{{ $json.articles }}`                      |
| `occasions` | `{{ $json.occasions }}`                     |

---

### 10. CharacterTextSplitter

| Eigenschaft          | Wert                                                           |
| -------------------- | -------------------------------------------------------------- |
| **Name**       | `CharacterTextSplitter`                                      |
| **Type**       | `@n8n/n8n-nodes-langchain.textSplitterCharacterTextSplitter` |
| **Version**    | 1                                                              |
| **Position**   | 3552, 3472                                                     |
| **Connection** | `ai_textSplitter` → DefaultDataLoader                       |

**Aufgabe:** Splittet die Dokument-Texte für den DefaultDataLoader.

**Einstellungen (n8n-Standardwerte, da keine überschrieben):**

| Eigenschaft             | Standardwert |
| ----------------------- | ------------ |
| **Chunk Size**    | 1000 Zeichen |
| **Chunk Overlap** | 0 Zeichen    |

---

## Verbindungen (Connections)

| Von                   | Nach              | Connection-Typ  |
| --------------------- | ----------------- | --------------- |
| FormUpload            | ExtractFromFile   | main            |
| ExtractFromFile       | SetFileId         | main            |
| SetFileId             | DeleteExistItems  | main            |
| DeleteExistItems      | SplitConfigBlocks | main            |
| SplitConfigBlocks     | ParseConfigBlocks | main            |
| ParseConfigBlocks     | PGVectorStore     | main            |
| Embeddings            | PGVectorStore     | ai_embedding    |
| DefaultDataLoader     | PGVectorStore     | ai_document     |
| CharacterTextSplitter | DefaultDataLoader | ai_textSplitter |

---

## Einstellungen

| Eigenschaft               | Wert     |
| ------------------------- | -------- |
| **Execution Order** | v1       |
| **Binary Mode**     | separate |
| **Available in MCP**| false    |

---

## Unterschiede zu FlowerShopUploadToVector

| Aspekt                      | FlowerShopUploadToVector          | FlowerShopUploadToVectorSimple     |
| --------------------------- | --------------------------------- | ---------------------------------- |
| Tabellenprüfung             | `CheckTableExists` + `TableExists?` IF-Node | Nicht vorhanden              |
| DELETE-Ausführung           | Nur wenn Tabelle existiert        | Immer                              |
| Anzahl Nodes (Verarbeitung) | 12                                | 10                                 |
| Flow-Typ                    | Bedingt (branch)                  | Linear                             |
