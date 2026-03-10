# n8n Schulung: Fortgeschrittene Workflow-Automatisierung

**Inhaltsübersicht**

1. [Architektur & Deployment-Strategien](#1-architektur--deployment-strategien)
2. [Erweiterte Node-Konzepte](#2-erweiterte-node-konzepte)
3. [Fehlerbehandlung & Resilienz](#3-fehlerbehandlung--resilienz)
4. [Custom Code: JavaScript & Python in n8n](#4-custom-code-javascript--python-in-n8n)
5. [AI-Integration & Agenten-Workflows](#5-ai-integration--agenten-workflows)
6. [Sicherheit & Secrets Management](#6-sicherheit--secrets-management)
7. [Performance-Optimierung](#7-performance-optimierung)
8. [Versionskontrolle & Team-Workflows](#8-versionskontrolle--team-workflows)
9. [Praxisprojekte & Übungen](#9-praxisprojekte--übungen)
10. [Checklisten & Referenz](#10-checklisten--referenz)

---

## 1. Architektur & Deployment-Strategien

### 1.1 Self-Hosted vs. n8n Cloud

| Kriterium | Self-Hosted | n8n Cloud |
|---|---|---|
| Datenkontrolle | ✅ Vollständig | ⚠️ EU/US-Region wählbar |
| Setup-Aufwand | Hoch | Minimal |
| Skalierung | Manuell (Docker/K8s) | Automatisch |
| Custom Nodes | ✅ Ja | ❌ Nein |
| Kosten | Infrastruktur | Execution-basiert |

### 1.2 Docker-Deployment (Empfohlen für Produktion)

```bash
# docker-compose.yml – Produktions-Setup
version: '3.8'
services:
  n8n:
    image: n8nio/n8n
    restart: always
    ports:
      - "5678:5678"
    environment:
      - N8N_HOST=your-domain.com
      - N8N_PROTOCOL=https
      - N8N_ENCRYPTION_KEY=your-secret-key
      - DB_TYPE=postgresdb
      - DB_POSTGRESDB_HOST=postgres
      - DB_POSTGRESDB_DATABASE=n8n
      - DB_POSTGRESDB_USER=n8n
      - DB_POSTGRESDB_PASSWORD=your-db-password
      - EXECUTIONS_MODE=queue
      - QUEUE_BULL_REDIS_HOST=redis
    volumes:
      - n8n_data:/home/node/.n8n
    depends_on:
      - postgres
      - redis

  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: n8n
      POSTGRES_USER: n8n
      POSTGRES_PASSWORD: your-db-password
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    restart: always

volumes:
  n8n_data:
  postgres_data:
```

> **💡 Tipp:** Im Queue-Mode (`EXECUTIONS_MODE=queue`) können mehrere Worker-Instanzen parallel Workflows abarbeiten – ideal für hohe Last.

### 1.3 Wichtige Umgebungsvariablen

```bash
# Webhook-Konfiguration
WEBHOOK_URL=https://your-domain.com/

# Execution-Limits
EXECUTIONS_DATA_MAX_AGE=168          # Stunden bis Ausführungen gelöscht werden
EXECUTIONS_DATA_PRUNE=true

# Worker-Skalierung (Queue-Mode)
N8N_CONCURRENCY_PRODUCTION_LIMIT=10  # Parallele Executions pro Worker
```

---

## 2. Erweiterte Node-Konzepte

### 2.1 Der HTTP Request Node – Fortgeschrittene Nutzung

```json
// Beispiel: Pagination mit Loop
{
  "node": "HTTP Request",
  "settings": {
    "url": "https://api.example.com/items",
    "queryParameters": {
      "page": "={{ $json.currentPage }}",
      "limit": "100"
    },
    "authentication": "headerAuth",
    "header": "Authorization: Bearer {{ $credentials.token }}"
  }
}
```

**Pagination-Pattern mit SplitInBatches:**

```
[Trigger] → [HTTP Request] → [IF: hat nächste Seite?]
                                    ↓ Ja
                              [Increment Page] → zurück zu HTTP Request
                                    ↓ Nein
                              [Merge alle Ergebnisse]
```

### 2.2 Merge Node – Modi im Überblick

| Modus | Beschreibung | Anwendungsfall |
|---|---|---|
| **Append** | Alle Items hintereinander | Daten aus mehreren Quellen sammeln |
| **Merge By Index** | Items nach Position zusammenführen | Parallele Streams synchronisieren |
| **Merge By Key** | Items nach Feldwert joinen | Datenbank-ähnlicher JOIN |
| **Wait for Both** | Wartet auf beide Inputs | Synchronisationspunkt |

### 2.3 SubWorkflows – Modularisierung

```
Hauptworkflow:
[Trigger] → [Execute Workflow: "validate-input"] 
          → [Execute Workflow: "process-data"]
          → [Execute Workflow: "send-notification"]
```

> **Best Practice:** Lagere wiederverwendbare Logik (z.B. E-Mail-Versand, Fehlerbenachrichtigung) in SubWorkflows aus. Übergabe via `$json` Input-Parameter.

### 2.4 Expressions & Jinja-ähnliche Syntax

```javascript
// Auf vorherigen Node zugreifen
{{ $node["HTTP Request"].json.data.id }}

// Aktuellen Item-Index
{{ $itemIndex }}

// Alle Items des vorherigen Nodes
{{ $items("HTTP Request") }}

// Datum formatieren
{{ $now.toFormat('yyyy-MM-dd') }}

// Bedingter Ausdruck
{{ $json.status === 'active' ? 'Aktiv' : 'Inaktiv' }}

// Auf Workflow-Variablen zugreifen
{{ $vars.API_BASE_URL }}
```

---

## 3. Fehlerbehandlung & Resilienz

### 3.1 Error Trigger Workflow

Erstelle einen dedizierten **Error Handler Workflow**:

```
[Error Trigger] 
  → [Set Node: Fehler-Details aufbereiten]
     {
       "workflow": "={{ $json.workflow.name }}",
       "node": "={{ $json.execution.lastNodeExecuted }}",
       "error": "={{ $json.execution.error.message }}",
       "timestamp": "={{ $now.toISO() }}",
       "executionUrl": "=https://n8n.company.com/execution/{{ $json.execution.id }}"
     }
  → [Slack / E-Mail: Alert senden]
  → [Optionally: Retry-Logik]
```

**Aktivierung im Workflow-Setting:**
`Settings → Error Workflow → [Dein Error Handler auswählen]`

### 3.2 Try-Catch Pattern

```
[Riskanter Node]
  ├── Erfolg → [Weiterverarbeitung]
  └── Fehler (Continue on Error) 
        → [IF: $json.error existiert?]
              ↓ Ja
           [Fallback-Logik / Logging]
```

**Node-Einstellung:** `Settings → On Error → Continue`

### 3.3 Retry-Mechanismus

```javascript
// Im Function Node: Eigene Retry-Logik
const maxRetries = 3;
let attempt = 0;
let success = false;

while (attempt < maxRetries && !success) {
  try {
    // Logik hier
    success = true;
  } catch (error) {
    attempt++;
    if (attempt === maxRetries) throw error;
    await new Promise(resolve => setTimeout(resolve, 1000 * attempt)); // Exponential Backoff
  }
}
```

### 3.4 Resilienz-Checkliste

- [ ] Error Handler Workflow für jeden Produktions-Workflow aktiviert
- [ ] Kritische Nodes auf "Continue on Error" gesetzt
- [ ] Fallback-Werte für fehlende API-Responses definiert
- [ ] Alerting via Slack/E-Mail eingerichtet
- [ ] Execution-Logs regelmäßig geprüft

---

## 4. Custom Code: JavaScript & Python in n8n

### 4.1 Code Node (JavaScript) – Fortgeschrittene Patterns

```javascript
// Alle eingehenden Items verarbeiten
const results = [];

for (const item of $input.all()) {
  const data = item.json;
  
  // Komplexe Transformation
  const transformed = {
    id: data.id,
    fullName: `${data.firstName} ${data.lastName}`.trim(),
    emailDomain: data.email?.split('@')[1] ?? 'unknown',
    tags: data.tags?.filter(t => t.active).map(t => t.name) ?? [],
    processedAt: new Date().toISOString()
  };
  
  results.push({ json: transformed });
}

return results;
```

```javascript
// Daten aggregieren (Reduce-Pattern)
const items = $input.all();

const summary = items.reduce((acc, item) => {
  const category = item.json.category ?? 'unknown';
  acc[category] = (acc[category] ?? 0) + item.json.amount;
  return acc;
}, {});

// Als einzelnes Item zurückgeben
return [{ json: { summary, total: Object.values(summary).reduce((a, b) => a + b, 0) } }];
```

### 4.2 Python im Code Node

```python
# Python-Unterstützung (ab n8n v1.0+)
import json
from datetime import datetime

items = _input.all()
results = []

for item in items:
    data = item['json']
    
    processed = {
        'id': data.get('id'),
        'score': round(data.get('raw_score', 0) * 100, 2),
        'category': 'high' if data.get('value', 0) > 1000 else 'low',
        'processed_at': datetime.now().isoformat()
    }
    results.append({'json': processed})

return results
```

### 4.3 Externe Module nutzen (Self-Hosted)

```bash
# In docker-compose.yml Environment:
NODE_FUNCTION_ALLOW_EXTERNAL=axios,lodash,moment,uuid

# Dann im Code Node:
const axios = require('axios');
const _ = require('lodash');
const { v4: uuidv4 } = require('uuid');
```

---

## 5. AI-Integration & Agenten-Workflows

### 5.1 AI-Node Architektur in n8n

n8n nutzt **LangChain** unter der Haube für alle KI-Nodes:

```
[Trigger / Chat Message]
  → [AI Agent Node]
        ├── Chat Model: OpenAI GPT-4 / Claude / Gemini
        ├── Memory: Window Buffer / Postgres / Redis
        └── Tools:
              ├── [HTTP Request Tool]
              ├── [Code Tool]
              ├── [n8n Workflow Tool]
              └── [Calculator / Web Search]
  → [Response]
```

### 5.2 Einfacher KI-Agent

```json
// AI Agent Konfiguration
{
  "agent": "conversationalAgent",
  "model": "gpt-4o",
  "systemMessage": "Du bist ein hilfreicher Assistent für das Support-Team. Beantworte Fragen auf Basis der bereitgestellten Kundendaten. Antworte immer auf Deutsch.",
  "maxIterations": 10,
  "returnIntermediateSteps": false
}
```

### 5.3 RAG-Workflow (Retrieval Augmented Generation)

```
[Dokument hochladen / Trigger]
  → [Dokument laden (PDF/Text)]
  → [Text splitten: RecursiveCharacterTextSplitter]
      chunk_size: 1000, overlap: 200
  → [Embeddings erstellen: OpenAI Embeddings]
  → [Vektordatenbank: Pinecone / Supabase / Qdrant]
  
--- Abfrage-Workflow ---

[Nutzer-Frage]
  → [Embeddings der Frage erstellen]
  → [Ähnliche Dokumente aus Vektordatenbank holen]
  → [Kontext + Frage an LLM]
  → [Antwort ausgeben]
```

### 5.4 Human-in-the-Loop Pattern

```
[AI schlägt Aktion vor]
  → [Warte auf Genehmigung: Webhook / Form]
      Timeout: 24h
      ├── Genehmigt → [Aktion ausführen]
      └── Abgelehnt → [Alternative / Eskalation]
```

### 5.5 MCP (Model Context Protocol) Integration

```bash
# n8n als MCP-Server konfigurieren
# Erlaubt Claude, Cursor etc. direkten Zugriff auf n8n-Workflows

# In n8n Settings → MCP → Enable
# Endpoint: https://your-n8n.com/mcp
```

---

## 6. Sicherheit & Secrets Management

### 6.1 Credentials Best Practices

```
✅ DO:
- Alle API-Keys als n8n Credentials speichern
- Credentials nie direkt in Expressions hartkodieren
- Separaten n8n-User pro Integrations-Team anlegen
- Credentials regelmäßig rotieren

❌ DON'T:
- API-Keys als Workflow-Variablen speichern
- Credentials in Workflow-Namen oder Beschreibungen schreiben
- Admin-Account für automatisierte Workflows nutzen
```

### 6.2 External Secrets Store

```bash
# AWS Secrets Manager Integration
EXTERNAL_SECRETS_PROVIDER=aws
AWS_REGION=eu-central-1
# IAM-Rolle für n8n-Instanz konfigurieren (kein Key/Secret nötig)

# HashiCorp Vault
EXTERNAL_SECRETS_PROVIDER=vault
VAULT_SERVER=https://vault.company.com
VAULT_TOKEN=your-vault-token
```

### 6.3 RBAC (Role-Based Access Control)

| Rolle | Workflows sehen | Workflows bearbeiten | Credentials | Admin |
|---|---|---|---|---|
| **Viewer** | ✅ (eigene) | ❌ | ❌ | ❌ |
| **Member** | ✅ | ✅ (eigene) | ✅ (eigene) | ❌ |
| **Admin** | ✅ (alle) | ✅ (alle) | ✅ (alle) | ✅ |

### 6.4 Webhook-Sicherheit

```javascript
// Webhook mit HMAC-Signatur validieren (im Code Node)
const crypto = require('crypto');

const signature = $headers['x-signature'];
const body = JSON.stringify($json);
const secret = $credentials.webhookSecret;

const expectedSignature = crypto
  .createHmac('sha256', secret)
  .update(body)
  .digest('hex');

if (signature !== `sha256=${expectedSignature}`) {
  throw new Error('Ungültige Webhook-Signatur');
}

return $input.all();
```

---

## 7. Performance-Optimierung

### 7.1 Batch-Verarbeitung

```
// SplitInBatches Node für große Datensätze
[1000 Items]
  → [SplitInBatches: batchSize=50]
  → [Verarbeitung (50 Items parallel)]
  → [Loop zurück bis alle verarbeitet]
  → [Merge: alle Ergebnisse]
```

**Konfiguration:**
```json
{
  "batchSize": 50,
  "options": {
    "reset": false
  }
}
```

### 7.2 Caching-Pattern

```javascript
// Einfaches In-Memory-Caching via Static Data
const cache = $getWorkflowStaticData('global');
const cacheKey = `api_result_${$json.id}`;
const cacheTTL = 3600; // 1 Stunde in Sekunden

const now = Date.now() / 1000;

if (cache[cacheKey] && (now - cache[cacheKey].timestamp) < cacheTTL) {
  // Cache-Hit
  return [{ json: { ...cache[cacheKey].data, fromCache: true } }];
}

// Cache-Miss: Daten holen (im nächsten Node)
// Nach dem API-Call:
cache[cacheKey] = {
  data: $json,
  timestamp: now
};

return $input.all();
```

### 7.3 Performance-Checkliste

- [ ] Unnötige Nodes entfernt (jeder Node = Overhead)
- [ ] Batch-Größe an API-Limits angepasst
- [ ] Parallele Executions für unabhängige Zweige genutzt
- [ ] Alte Executions automatisch bereinigt (`EXECUTIONS_DATA_PRUNE=true`)
- [ ] Queue-Mode für Produktions-Workloads aktiviert
- [ ] Monitoring eingerichtet (Grafana, Datadog etc.)

---

## 8. Versionskontrolle & Team-Workflows

### 8.1 Git-Integration einrichten

```bash
# In n8n Settings → Version Control

# Git-Repository verbinden:
REPOSITORY_URL=git@github.com:company/n8n-workflows.git
GIT_BRANCH=main
```

**Workflow:**
```
Änderungen in n8n vornehmen
  → "Push" in n8n UI
  → Commit im Git-Repo
  → Code Review via Pull Request
  → Merge → Deployment auf Prod-Instanz via "Pull"
```

### 8.2 Environments verwalten

```
Empfohlene Branch-Strategie:
  main        → Produktion
  staging     → Test/Staging
  dev         → Entwicklung

n8n-Instanzen:
  n8n-prod    → Branch: main     (automatischer Pull bei Merge)
  n8n-staging → Branch: staging
  n8n-dev     → Branch: dev
```

### 8.3 Workflow-Dokumentation

```json
// In jedem Workflow pflegen:
{
  "name": "customer-onboarding-v2",
  "description": "Verarbeitet neue Kunden-Registrierungen. Trigger: Webhook von CRM. Output: Account in DB + Willkommens-E-Mail. Owner: @max.mustermann",
  "tags": ["onboarding", "crm", "email", "produktion"],
  "settings": {
    "notes": "Deployed: 2025-03-01. Abhängigkeiten: CRM-API, Mailjet, PostgreSQL."
  }
}
```

### 8.4 Testing-Strategie

| Test-Typ | Methode | Wann |
|---|---|---|
| **Unit** | Code Node mit Test-Daten | Bei Code-Änderungen |
| **Integration** | Test-Trigger mit Staging-Credentials | Vor Deployment |
| **End-to-End** | Vollständige Ausführung in Staging | Vor Prod-Release |
| **Monitoring** | Execution-Logs + Alerting | Kontinuierlich |

---

## 9. Praxisprojekte & Übungen

### Projekt 1: Intelligentes Ticket-Routing System ⭐⭐

**Ziel:** Eingehende Support-Tickets automatisch kategorisieren und routen.

```
[Webhook: Neues Ticket]
  → [AI Node: Kategorie & Priorität ermitteln]
     Prompt: "Analysiere dieses Support-Ticket und bestimme:
              - Kategorie: bug/feature/billing/general
              - Priorität: low/medium/high/critical
              Antworte nur als JSON."
  → [Switch Node: nach Kategorie]
       ├── bug      → [Jira: Issue erstellen]
       ├── billing  → [Slack: #billing-team benachrichtigen]
       └── feature  → [Notion: Feature-Backlog eintragen]
  → [E-Mail: Bestätigung an Kunden]
```

**Übungsaufgaben:**
1. Füge einen Human-in-the-Loop-Schritt für "critical"-Tickets hinzu
2. Implementiere eine Eskalation nach 4 Stunden ohne Reaktion
3. Baue ein Dashboard mit Ticket-Statistiken (Notion/Google Sheets)

---

### Projekt 2: Automatisierte Daten-Pipeline ⭐⭐⭐

**Ziel:** Täglich Daten aus mehreren Quellen aggregieren, transformieren und in eine Datenbank laden.

```
[Cron: täglich 02:00 Uhr]
  → [Parallel: Daten aus 3 APIs holen]
       ├── [API 1: Salesforce Deals]
       ├── [API 2: Google Analytics]
       └── [API 3: Stripe Revenue]
  → [Merge: alle Daten zusammenführen]
  → [Code Node: Datentransformation & Aggregation]
  → [PostgreSQL: Daten upserten]
  → [IF: Anomalie erkannt?]
       ├── Nein → [Slack: Erfolgs-Notification]
       └── Ja  → [Slack: Alert + Detail-Report per E-Mail]
```

**Übungsaufgaben:**
1. Füge Datenvalidierung mit aussagekräftigen Fehlermeldungen hinzu
2. Implementiere idempotente Verarbeitung (doppelter Lauf = gleiche Daten)
3. Baue ein Retry-Mechanism für einzelne API-Failures

---

### Projekt 3: KI-gestützter Content-Workflow ⭐⭐⭐

**Ziel:** Aus einem Keyword automatisch Blog-Post erstellen, optimieren und veröffentlichen.

```
[Formular: Keyword eingeben]
  → [AI: Outline generieren]
  → [Warte auf Freigabe (Human-in-the-Loop)]
  → [AI: Vollständigen Artikel schreiben]
  → [AI: SEO-Analyse & Optimierung]
  → [Bild: DALL-E / Midjourney via API]
  → [WordPress / CMS: Als Entwurf speichern]
  → [Slack: "Bereit zur Überprüfung" + Link]
```

---

## 10. Checklisten & Referenz

### ✅ Produktions-Readiness Checkliste

**Workflow-Qualität:**
- [ ] Sinnvoller Workflow-Name (kebab-case, z.B. `customer-invoice-generator`)
- [ ] Beschreibung mit Owner, Trigger, Output vorhanden
- [ ] Tags für Kategorisierung gesetzt
- [ ] Alle Nodes beschriftet

**Fehlerbehandlung:**
- [ ] Error Handler Workflow zugewiesen
- [ ] Kritische Nodes: "Continue on Error" konfiguriert
- [ ] Fallback-Logik für externe API-Ausfälle implementiert
- [ ] Alerting bei Workflow-Fehlern eingerichtet

**Sicherheit:**
- [ ] Alle Secrets als Credentials gespeichert
- [ ] Webhooks mit Authentifizierung gesichert
- [ ] Minimale Berechtigungen für Credentials genutzt

**Performance:**
- [ ] Batch-Verarbeitung für >100 Items implementiert
- [ ] Keine unnötigen Wait-Nodes oder Delays
- [ ] Execution-Retention-Policy konfiguriert

**Deployment:**
- [ ] In Staging getestet
- [ ] Git-Commit mit aussagekräftiger Message
- [ ] Rollback-Plan definiert

---

### 📚 Weiterführende Ressourcen

| Ressource | URL | Beschreibung |
|---|---|---|
| Offizielle Docs | docs.n8n.io | Vollständige Dokumentation |
| Community Forum | community.n8n.io | Fragen & Antworten |
| Workflow-Templates | n8n.io/workflows | 7.000+ fertige Workflows |
| YouTube-Kanal | youtube.com/@n8n-io | Video-Tutorials |
| GitHub | github.com/n8n-io/n8n | Source Code & Issues |

---

### 🔑 Wichtige Expressions Quick-Reference

```javascript
// Input-Daten
$json                          // Aktuelles Item
$input.all()                   // Alle Items
$input.first()                 // Erstes Item
$input.last()                  // Letztes Item

// Auf andere Nodes zugreifen
$node["NodeName"].json         // Daten eines spezifischen Nodes
$items("NodeName")             // Alle Items eines Nodes

// Workflow-Kontext
$workflow.name                 // Workflow-Name
$execution.id                  // Aktuelle Execution-ID
$now                           // Aktueller Zeitpunkt (Luxon-Objekt)

// Datum & Zeit
$now.toISO()                   // ISO-String: "2025-03-05T10:30:00.000Z"
$now.toFormat('dd.MM.yyyy')    // Deutsch: "05.03.2025"
$now.minus({ days: 7 })        // Vor 7 Tagen
$now.plus({ hours: 2 })        // In 2 Stunden

// String-Operationen
$json.text?.toLowerCase()      // Optional chaining
$json.name?.split(' ')[0]      // Vorname extrahieren
`Hallo ${$json.name}!`         // Template-String

// Zahlen
Math.round($json.price * 1.19) // MwSt-Berechnung
Math.max(...$items().map(i => i.json.value)) // Maximum
```

---

*Schulungsinhalt erstellt für n8n v1.x – Stand März 2025*  
*Alle Beispiele sind produktionserprobt und für Fortgeschrittene konzipiert.*
