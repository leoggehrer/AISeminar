# AISeminar – n8n Workflow-Automatisierung mit RAG

Dieses Repository enthält Schulungsunterlagen und praktische Beispiele zur **Workflow-Automatisierung mit n8n** in Kombination mit **Retrieval-Augmented Generation (RAG)**. Als durchgängiges Praxisbeispiel dient ein Blumenshop, dessen Produktkatalog über Vektor-Embeddings semantisch durchsuchbar gemacht wird.

## Inhaltsverzeichnis

- [Projektstruktur](#projektstruktur)
- [Voraussetzungen](#voraussetzungen)
- [Installation & Start](#installation--start)
- [Dokumentation](#dokumentation)
- [Workflows](#workflows)
- [Technologien](#technologien)

## Projektstruktur

```
AISeminar/
├── docker/                        # Docker-Konfiguration für den n8n-Stack
│   ├── compose.yml                # Docker Compose (n8n, PostgreSQL, pgvector)
│   ├── compose_commands.md        # Docker-CLI-Referenz
│   └── compose_credential.md     # Datenbank-Verbindungsdaten & Credentials
├── n8n/                           # Schulungsunterlagen (deutsch)
│   ├── n8n_Einfuehrung.md        # Einführung in n8n
│   ├── n8n_Grundlagen.md         # Fortgeschrittene Konzepte & Best Practices
│   └── n8n_RAG.md                # RAG-Integration mit n8n
├── workflows/                     # n8n-Workflow-Definitionen
│   └── flowerShop/               # Blumenshop-Beispielanwendung
│       ├── chatAgent/            # Chat-Agent-Workflows (JSON)
│       └── vectorStore/          # Vektor-Upload-Workflows & Produktdaten
└── README.md
```

## Voraussetzungen

- [Docker](https://www.docker.com/) & Docker Compose
- [OpenAI API-Key](https://platform.openai.com/) (für Embeddings und LLM-Zugriff)

## Installation & Start

1. **Repository klonen:**

   ```bash
   git clone https://github.com/leoggehrer/AISeminar.git
   cd AISeminar
   ```

2. **Docker-Container starten:**

   ```bash
   cd docker
   docker compose up -d
   ```

   Damit werden drei Services gestartet:

   | Service        | Port  | Beschreibung                          |
   |----------------|-------|---------------------------------------|
   | n8n            | 5678  | Workflow-Editor & Runtime             |
   | PostgreSQL     | 5442  | Datenbank für n8n                     |
   | pgvector       | 5443  | Vektor-Datenbank für RAG-Embeddings   |

3. **n8n öffnen:** [http://localhost:5678](http://localhost:5678)

4. **Credentials einrichten:** Siehe [compose_credential.md](docker/compose_credential.md) für die Datenbankverbindungen und OpenAI-API-Key-Konfiguration.

## Dokumentation

Die Schulungsunterlagen sind in deutscher Sprache verfasst:

| Dokument | Inhalt |
|----------|--------|
| [n8n Einführung](n8n/n8n_Einfuehrung.md) | Grundbegriffe, UI-Überblick, erste Workflows, Vergleich mit Zapier/Make |
| [n8n Grundlagen](n8n/n8n_Grundlagen.md) | Architektur, Error-Handling, Custom Code, AI-Integration, Security, Performance |
| [n8n RAG](n8n/n8n_RAG.md) | RAG-Konzept, relevante n8n-Nodes, Use-Cases, Best Practices |

## Workflows

### Blumenshop – Upload to Vector Store

Ein produktionsnaher RAG-Workflow, der Blumenprodukt-Konfigurationen verarbeitet:

1. **Datei-Upload** über ein Webformular
2. **Text-Extraktion** und Parsing strukturierter Produktdaten (Name, Preis, Artikel, Tags, Anlässe)
3. **Embedding-Erzeugung** via OpenAI
4. **Speicherung** in pgvector für semantische Suche

Details: [FlowerShopUploadToVector.md](workflows/flowerShop/vectorStore/FlowerShopUploadToVector.md)

### Blumenshop – Chat Agent

Chat-basierte Workflows, die den Vektor-Store für intelligente Produktempfehlungen nutzen.

## Technologien

- **[n8n](https://n8n.io/)** – Open-Source Workflow-Automatisierung
- **PostgreSQL** – Relationale Datenbank
- **[pgvector](https://github.com/pgvector/pgvector)** – Vektor-Erweiterung für PostgreSQL
- **OpenAI API** – Embeddings & LLM
- **Docker** – Containerisierung
