# Workflow: FlowerShopUploadToVector

| Eigenschaft                 | Wert                 |
| --------------------------- | -------------------- |
| **ID**                | `xnqngMngsGqYktkQ` |
| **Status**            | Inaktiv              |
| **Erstellt**          | 2026-03-19           |
| **Zuletzt geändert** | 2026-03-19           |

## Beschreibung

Ermöglicht den Upload einer strukturierten Konfigurationsdatei (z. B. Blumensträuße mit Artikeln, Preisen, Tags) über ein Webformular. Die Datei wird geparst, in einzelne Produktblöcke aufgeteilt, vektorisiert (OpenAI Embeddings) und in eine PostgreSQL-Vektordatenbank (`flowerShop_doc_vectors`) geschrieben. Bereits vorhandene Einträge derselben Datei werden vor dem Import gelöscht, um Duplikate zu vermeiden.

---

## Flow-Übersicht

```
FormUpload → ExtractFromFile → SetFileId → CheckTableExists → TableExists?
                                                                  ├─ true  → DeleteExistItems → SplitConfigBlocks
                                                                  └─ false → SplitConfigBlocks
                                                                                    ↓
                                                               ParseConfigBlocks → PGVectorStore
                                                                                       ↑
                                                                         Embeddings (ai_embedding)
                                                                         DefaultDataLoader (ai_document)
                                                                              ↑
                                                                    Character Text Splitter (ai_textSplitter)
```

---

## Nodes

### 1. FormUpload

| Eigenschaft              | Wert                                                                                  |
| ------------------------ | ------------------------------------------------------------------------------------- |
| **Name**           | `FormUpload`                                                                        |
| **Type**           | `n8n-nodes-base.formTrigger`                                                        |
| **Version**        | 2.3                                                                                   |
| **Position**       | 2416, 2336                                                                            |
| **Formular-Titel** | `FlowerShop – Datei zu Vector`                                                     |
| **Beschreibung**   | Laden Sie eine Konfigurationsdatei hoch, um sie in den Vektorspeicher zu importieren. |
| **Webhook-ID**     | `4a8a6093-488a-4e16-9e70-9a3bf89a6e68`                                              |

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
| **Position**        | 2624, 2336                         |
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
| **Position** | 2832, 2336             |

**Zuweisungen:**

| Variable      | Wert                                                  | Typ    |
| ------------- | ----------------------------------------------------- | ------ |
| `file_id`   | `{{ $('FormUpload').item.json.Datei[0].filename }}` | string |
| `file_type` | `{{ $('FormUpload').item.json.Datei[0].mimetype }}` | string |

**Aufgabe:** Speichert Dateiname und MIME-Typ der hochgeladenen Datei für die spätere Verwendung als Metadaten und zur Duplikat-Erkennung.

---

### 4. CheckTableExists

| Eigenschaft          | Wert                        |
| -------------------- | --------------------------- |
| **Name**       | `CheckTableExists`        |
| **Type**       | `n8n-nodes-base.postgres` |
| **Version**    | 2.6                         |
| **Position**   | 3040, 2336                  |
| **Operation**  | `executeQuery`            |
| **Credential** | `PGVector`                |

**SQL-Query:**

```sql
SELECT EXISTS (
  SELECT 1 FROM information_schema.tables
  WHERE table_schema = 'public'
  AND table_name = 'flowershop_doc_vectors'
) AS table_exists;
```

**Aufgabe:** Prüft, ob die Zieltabelle `flowershop_doc_vectors` bereits in der Datenbank existiert, bevor versucht wird, bestehende Einträge zu löschen.

---

### 5. TableExists?

| Eigenschaft        | Wert                  |
| ------------------ | --------------------- |
| **Name**     | `TableExists?`      |
| **Type**     | `n8n-nodes-base.if` |
| **Version**  | 2.2                   |
| **Position** | 3232, 2336            |

**Bedingung:**

| Linker Wert                  | Operator      | Rechter Wert |
| ---------------------------- | ------------- | ------------ |
| `{{ $json.table_exists }}` | boolean: true | —           |

**Routing:**

- **True** → `DeleteExistItems` (Tabelle existiert → alte Einträge löschen)
- **False** → `SplitConfigBlocks` (Tabelle existiert nicht → direkt weiter)

---

### 6. DeleteExistItems

| Eigenschaft          | Wert                        |
| -------------------- | --------------------------- |
| **Name**       | `DeleteExistItems`        |
| **Type**       | `n8n-nodes-base.postgres` |
| **Version**    | 2.6                         |
| **Position**   | 3424, 2208                  |
| **Operation**  | `executeQuery`            |
| **Credential** | `PGVector`                |

**SQL-Query:**

```sql
DELETE FROM flowerShop_doc_vectors
WHERE metadata#>>'{file_id}'='{{$('SetFileId').item.json.file_id}}';
```

**Aufgabe:** Löscht alle bestehenden Vektoren, die zur aktuellen Datei gehören (anhand der `file_id` in den Metadaten). Verhindert Duplikate bei erneutem Upload derselben Datei.

---

### 7. SplitConfigBlocks

| Eigenschaft        | Wert                    |
| ------------------ | ----------------------- |
| **Name**     | `SplitConfigBlocks`   |
| **Type**     | `n8n-nodes-base.code` |
| **Version**  | 2                       |
| **Position** | 3600, 2352              |
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

### 8. ParseConfigBlocks

| Eigenschaft        | Wert                    |
| ------------------ | ----------------------- |
| **Name**     | `ParseConfigBlocks`   |
| **Type**     | `n8n-nodes-base.code` |
| **Version**  | 2                       |
| **Position** | 3792, 2352              |
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
  const articleSummary = articles
    .map(a => `${a.articleId} ${a.productName} (${a.quantity}×)`)
    .join(', ');

  const textForEmbedding = `Name: ${name}. Preiskategorie: ${priceCategory}. `
    + `Beschreibung: ${description}. Tags: ${tags.join(', ')}. `
    + `Anlass: ${occasions.join(', ')}. Artikel: ${articleSummary}. `
    + `Preis: ${price} Euro.`;

  output.push({
    json: {
      name, priceCategory, description, price,
      articles, articleIds, tags, occasions, textForEmbedding
    },
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

### 9. PGVectorStore

| Eigenschaft          | Wert                                             |
| -------------------- | ------------------------------------------------ |
| **Name**       | `PGVectorStore`                                |
| **Type**       | `@n8n/n8n-nodes-langchain.vectorStorePGVector` |
| **Version**    | 1.3                                              |
| **Position**   | 3984, 2352                                       |
| **Modus**      | `insert`                                       |
| **Tabelle**    | `flowerShop_doc_vectors`                       |
| **Credential** | `PGVector`                                     |

**Sub-Nodes (Connections):**

- `Embeddings` → als `ai_embedding`
- `DefaultDataLoader` → als `ai_document`

---

### 10. Embeddings

| Eigenschaft          | Wert                                          |
| -------------------- | --------------------------------------------- |
| **Name**       | `Embeddings`                                |
| **Type**       | `@n8n/n8n-nodes-langchain.embeddingsOpenAi` |
| **Version**    | 1.2                                           |
| **Position**   | 3984, 2576                                    |
| **Credential** | `OpenAiISO`                                 |
| **Connection** | `ai_embedding` → PGVectorStore             |

**Aufgabe:** Erzeugt Embedding-Vektoren (Standardmodell) für die Textdaten, die anschließend im PGVectorStore gespeichert werden.

---

### 11. DefaultDataLoader

| Eigenschaft              | Wert                                                   |
| ------------------------ | ------------------------------------------------------ |
| **Name**           | `DefaultDataLoader`                                  |
| **Type**           | `@n8n/n8n-nodes-langchain.documentDefaultDataLoader` |
| **Version**        | 1.1                                                    |
| **Position**       | 4096, 2576                                             |
| **JSON-Modus**     | `expressionData`                                     |
| **Datenquelle**    | `{{ $json.textForEmbedding }}`                       |
| **Text Splitting** | Custom (via Character Text Splitter)                   |
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

### 12. Character Text Splitter

| Eigenschaft          | Wert                                                           |
| -------------------- | -------------------------------------------------------------- |
| **Name**       | `Character Text Splitter`                                    |
| **Type**       | `@n8n/n8n-nodes-langchain.textSplitterCharacterTextSplitter` |
| **Version**    | 1                                                              |
| **Position**   | 4096, 2784                                                     |
| **Connection** | `ai_textSplitter` → DefaultDataLoader                       |

**Aufgabe:** Splittet die Dokument-Texte für den DefaultDataLoader.

**Einstellungen (n8n-Standardwerte, da keine überschrieben):**

| Eigenschaft             | Standardwert |
| ----------------------- | ------------ |
| **Chunk Size**    | 1000 Zeichen |
| **Chunk Overlap** | 0 Zeichen    |

---

## Verbindungen (Connections)

| Von                     | Nach              | Connection-Typ  |
| ----------------------- | ----------------- | --------------- |
| FormUpload              | ExtractFromFile   | main            |
| ExtractFromFile         | SetFileId         | main            |
| SetFileId               | CheckTableExists  | main            |
| CheckTableExists        | TableExists?      | main            |
| TableExists? (true)     | DeleteExistItems  | main            |
| TableExists? (false)    | SplitConfigBlocks | main            |
| DeleteExistItems        | SplitConfigBlocks | main            |
| SplitConfigBlocks       | ParseConfigBlocks | main            |
| ParseConfigBlocks       | PGVectorStore     | main            |
| Embeddings              | PGVectorStore     | ai_embedding    |
| DefaultDataLoader       | PGVectorStore     | ai_document     |
| Character Text Splitter | DefaultDataLoader | ai_textSplitter |

---

## Einstellungen

| Eigenschaft               | Wert     |
| ------------------------- | -------- |
| **Execution Order** | v1       |
| **Binary Mode**     | separate |
