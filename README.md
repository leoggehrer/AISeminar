---
marp: true
theme: default
paginate: true
---

<!-- _class: lead -->
# AI-Seminar

**Moderne Softwareentwicklung mit KI und Automatisierung**

---

## Agenda

1. Software Entwicklung mit Copilot und `copilot-instructions.md`
2. Workflow Systeme mit n8n
3. Hybride Software Entwicklung mit n8n
4. MCP-Server mit n8n verbinden

---

## 1. Software Entwicklung mit Copilot

> GitHub Copilot unterstützt Entwickler durch KI-gestützte Codevorschläge direkt im Editor.

- **Was ist GitHub Copilot?**
  - KI-Assistent auf Basis großer Sprachmodelle (LLMs)
  - Unterstützt bei Code, Tests, Dokumentation und Refactoring
- **copilot-instructions.md**
  - Projektspezifische Regeln für Copilot definieren
  - Konventionen, Architekturvorgaben, Coding-Standards
  - Wiederverwendbare Prompts für das gesamte Team

---

## 1. Copilot – Demo & Praxis

- Code-Vervollständigung im Editor
- Chat-basierte Unterstützung (`Cmd+I`)
- Eigene `copilot-instructions.md` erstellen
- Agenten-Modus für komplexe Aufgaben

---

## 2. Workflow Systeme mit n8n

> n8n ist ein Open-Source-Tool zur visuellen Workflow-Automatisierung.

- **Kernfunktionen**
  - Visuelle Drag-and-Drop-Oberfläche
  - 400+ integrierte Konnektoren (APIs, Datenbanken, Services)
  - Selbst-hostbar oder als Cloud-Variante
- **Typische Anwendungsfälle**
  - Daten zwischen Systemen synchronisieren
  - Benachrichtigungen und Berichte automatisieren
  - API-Integrationen ohne Code

---

## 2. n8n – Grundkonzepte

- **Trigger** – startet einen Workflow (Webhook, Timer, Event)
- **Nodes** – einzelne Verarbeitungsschritte
- **Connections** – Datenfluss zwischen Nodes
- **Credentials** – sichere Speicherung von API-Keys

---

## 3. Hybride Software Entwicklung mit n8n

> Kombination aus klassischer Entwicklung und visueller Automatisierung.

- **Warum hybrid?**
  - Komplexe Geschäftslogik bleibt im Code (C#, TypeScript, …)
  - Integrationen und Prozesse werden in n8n abgebildet
  - Schnellere Iteration bei Prozessänderungen
- **Vorteile**
  - Klare Trennung von Verantwortlichkeiten
  - Entwickler + Fachbereich arbeiten gemeinsam
  - Weniger Boilerplate-Code für Integrationen

---

## 3. Hybride Entwicklung – Architektur

- Backend (WebAPI / Logic Layer) stellt Endpunkte bereit
- n8n orchestriert Aufrufe und Datentransformationen
- KI-Modelle werden als Nodes eingebunden
- Ergebnis: flexible, wartbare Gesamtlösung

---

## 4. MCP-Server mit n8n verbinden

> Das Model Context Protocol (MCP) standardisiert die Kommunikation zwischen KI-Modellen und externen Tools.

- **Was ist MCP?**
  - Offenes Protokoll von Anthropic
  - Verbindet KI-Agenten mit Datenquellen & Werkzeugen
  - Ermöglicht strukturierte Tool-Aufrufe durch LLMs
- **MCP in n8n**
  - MCP-Server als Node einbinden
  - KI-Agenten mit Echtzeit-Daten versorgen
  - Komplexe Agenten-Workflows aufbauen

---

## 4. MCP – Praxisbeispiel

- n8n-Workflow als MCP-Server bereitstellen
- GitHub Copilot / Claude greift auf lokale Daten zu
- Beispiel: Produktkatalog, ToDoGenerator, PulseCheck
- Live-Demo: MCP-Server verbinden & testen

---

<!-- _class: lead -->

## Zusammenfassung

| Thema | Technologie | Nutzen |
|-------|-------------|--------|
| Code-Assistent | GitHub Copilot | Produktivitätssteigerung |
| Automatisierung | n8n | Prozessintegration |
| Hybride Entwicklung | Code + n8n | Flexibilität |
| KI-Agenten | MCP + n8n | Intelligente Workflows |

---

<!-- _class: lead -->

# Fragen & Diskussion

**Danke für eure Aufmerksamkeit!**