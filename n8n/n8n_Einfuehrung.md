# n8n – Workflow-Automatisierung für alle

**Inhaltsübersicht**
- [n8n – Workflow-Automatisierung für alle](#n8n--workflow-automatisierung-für-alle)
  - [Was ist n8n?](#was-ist-n8n)
  - [Warum Automatisierung?](#warum-automatisierung)
  - [n8n im Vergleich zu anderen Tools](#n8n-im-vergleich-zu-anderen-tools)
  - [Die wichtigsten Begriffe](#die-wichtigsten-begriffe)
    - [🔄 Workflow](#-workflow)
    - [🟦 Node (Knoten)](#-node-knoten)
    - [🔗 Connection (Verbindung)](#-connection-verbindung)
    - [⚡ Trigger](#-trigger)
    - [▶️ Execution (Ausführung)](#️-execution-ausführung)
  - [Die Benutzeroberfläche im Überblick](#die-benutzeroberfläche-im-überblick)
    - [Der Workflow-Editor (Canvas)](#der-workflow-editor-canvas)
  - [Welche Integrationen gibt es?](#welche-integrationen-gibt-es)
    - [Kommunikation](#kommunikation)
    - [Projektmanagement](#projektmanagement)
    - [Cloud-Speicher](#cloud-speicher)
    - [Datenbanken](#datenbanken)
    - [CRM \& Marketing](#crm--marketing)
    - [Entwicklung \& IT](#entwicklung--it)
    - [KI \& Maschinelles Lernen](#ki--maschinelles-lernen)
  - [Praxisbeispiel 1: Automatische Benachrichtigung](#praxisbeispiel-1-automatische-benachrichtigung)
  - [Praxisbeispiel 2: Täglicher Bericht](#praxisbeispiel-2-täglicher-bericht)
  - [Praxisbeispiel 3: KI-gestützte Verarbeitung](#praxisbeispiel-3-ki-gestützte-verarbeitung)
  - [Einen Workflow erstellen – Schritt für Schritt](#einen-workflow-erstellen--schritt-für-schritt)
    - [Schritt 1: Neuen Workflow anlegen](#schritt-1-neuen-workflow-anlegen)
    - [Schritt 2: Trigger auswählen](#schritt-2-trigger-auswählen)
    - [Schritt 3: Nodes hinzufügen](#schritt-3-nodes-hinzufügen)
    - [Schritt 4: Nodes konfigurieren](#schritt-4-nodes-konfigurieren)
    - [Schritt 5: Testen](#schritt-5-testen)
    - [Schritt 6: Aktivieren](#schritt-6-aktivieren)
  - [Wichtige Node-Typen](#wichtige-node-typen)
    - [Trigger-Nodes (Auslöser)](#trigger-nodes-auslöser)
    - [Aktions-Nodes](#aktions-nodes)
    - [Logik-Nodes](#logik-nodes)
  - [Expressions \& Datenfluss](#expressions--datenfluss)
  - [Credentials (Zugangsdaten)](#credentials-zugangsdaten)
  - [Fehlerbehandlung](#fehlerbehandlung)
    - [Error Workflow](#error-workflow)
    - [Retry on Fail](#retry-on-fail)
    - [Error Output](#error-output)
  - [Hosting-Optionen](#hosting-optionen)
    - [Self-Hosted (eigener Server)](#self-hosted-eigener-server)
    - [n8n Cloud](#n8n-cloud)
  - [Best Practices](#best-practices)
    - [Benennung \& Organisation](#benennung--organisation)
    - [Sicherheit](#sicherheit)
    - [Entwicklung \& Test](#entwicklung--test)
    - [Performance](#performance)
  - [Nützliche Ressourcen](#nützliche-ressourcen)
  - [Zusammenfassung](#zusammenfassung)
  - [Nächste Schritte](#nächste-schritte)

---

## Was ist n8n?

n8n (ausgesprochen „nodemation") ist eine **Open-Source-Plattform zur Workflow-Automatisierung**. Sie ermöglicht es, verschiedene Apps, Dienste und Systeme miteinander zu verbinden – ganz ohne oder mit nur wenig Programmierung.

**Das Grundprinzip:** Wenn *dies* passiert, dann tue *das*.

> **Beispiel:** Wenn eine neue E-Mail eingeht → extrahiere den Anhang → speichere ihn in Google Drive → sende eine Slack-Nachricht an das Team.

---

## Warum Automatisierung?

Viele alltägliche Aufgaben in Unternehmen sind **repetitiv und zeitraubend**:

- Daten manuell von einem System in ein anderes übertragen
- Regelmäßige Berichte zusammenstellen und versenden
- Benachrichtigungen bei bestimmten Ereignissen manuell auslösen
- Dateien zwischen verschiedenen Cloud-Diensten synchronisieren

**Automatisierung spart Zeit, reduziert Fehler und schafft Freiraum für wertschöpfende Arbeit.**

---

## n8n im Vergleich zu anderen Tools

| Eigenschaft | n8n | Zapier | Make (Integromat) |
|---|---|---|---|
| **Open Source** | ✅ Ja | ❌ Nein | ❌ Nein |
| **Self-Hosting möglich** | ✅ Ja | ❌ Nein | ❌ Nein |
| **Kostenlose Nutzung** | ✅ Unbegrenzt (Self-Hosted) | ⚠️ Stark begrenzt | ⚠️ Begrenzt |
| **Datenschutz / DSGVO** | ✅ Volle Kontrolle | ⚠️ US-Server | ⚠️ EU-Option verfügbar |
| **Code-Erweiterbar** | ✅ JavaScript/Python | ❌ Eingeschränkt | ⚠️ Eingeschränkt |
| **Komplexe Workflows** | ✅ Sehr gut | ⚠️ Eingeschränkt | ✅ Gut |

**Fazit:** n8n bietet maximale Flexibilität und Datenkontrolle – ideal für Unternehmen mit hohen Datenschutzanforderungen.

---

## Die wichtigsten Begriffe

### 🔄 Workflow
Ein Workflow ist eine automatisierte Abfolge von Schritten. Er definiert, **was** passieren soll und **in welcher Reihenfolge**.

### 🟦 Node (Knoten)
Ein Node ist ein **einzelner Baustein** innerhalb eines Workflows. Jeder Node führt eine bestimmte Aktion aus – z. B. eine E-Mail senden, Daten aus einer Datenbank lesen oder eine Berechnung durchführen.

### 🔗 Connection (Verbindung)
Connections verbinden Nodes miteinander und bestimmen den **Datenfluss** – also welche Daten von einem Schritt zum nächsten weitergegeben werden.

### ⚡ Trigger
Ein Trigger ist ein spezieller Node, der den Workflow **auslöst** – z. B. zu einer bestimmten Uhrzeit, beim Eingang einer E-Mail oder beim Empfang eines Webhooks.

### ▶️ Execution (Ausführung)
Jedes Mal, wenn ein Workflow durchläuft, ist das eine Execution. Man kann vergangene Ausführungen einsehen und Fehler analysieren.

---

## Die Benutzeroberfläche im Überblick

### Der Workflow-Editor (Canvas)

Der Workflow-Editor ist das Herzstück von n8n. Hier baut man Workflows visuell zusammen:

```
┌──────────────────────────────────────────────────────────┐
│    Workflow-Name                           Execute       │
├──────────────────────────────────────────────────────────┤
│                                                          │
│   ┌─────────┐     ┌──────────┐     ┌─────────────┐       │
│   │ Trigger │────▶│  Node 1  │────▶│   Node 2    │       │
│   │ (Start) │     │(Aktion 1)│     │ (Aktion 2)  │       │
│   └─────────┘     └──────────┘     └─────────────┘       │
│                                                          │
│                                          ┌───────────┐   │
│                                     ────▶│  Node 3   │   │
│                                          │(Aktion 3) │   │
│                                          └───────────┘   │
│                                                          │
├──────────────────────────────────────────────────────────┤
│  Node hinzufügen     Suchen      Executions              │
└──────────────────────────────────────────────────────────┘
```

**Wichtige Bereiche:**
- **Canvas (Arbeitsfläche):** Hier platziert und verbindet man Nodes per Drag & Drop.
- **Node-Panel:** Über das „+"-Symbol öffnet sich die Bibliothek aller verfügbaren Nodes.
- **Ausführungs-Log:** Zeigt vergangene Workflow-Ausführungen mit Details und Fehlern.

---

## Welche Integrationen gibt es?

n8n bietet über **400+ Integrationen** (und es werden ständig mehr). Hier einige Kategorien:

### Kommunikation
- Slack, Microsoft Teams, Discord
- Gmail, Outlook, IMAP/SMTP
- Telegram, WhatsApp

### Projektmanagement
- Jira, Asana, Trello, Notion
- Monday.com, ClickUp

### Cloud-Speicher
- Google Drive, OneDrive, Dropbox
- AWS S3, FTP/SFTP

### Datenbanken
- MySQL, PostgreSQL, MongoDB
- Google Sheets, Airtable

### CRM & Marketing
- Salesforce, HubSpot
- Mailchimp, ActiveCampaign

### Entwicklung & IT
- GitHub, GitLab
- HTTP Request (jede REST-API)
- Webhook (empfängt externe Daten)

### KI & Maschinelles Lernen
- OpenAI / ChatGPT
- Google AI, Hugging Face

---

## Praxisbeispiel 1: Automatische Benachrichtigung

**Szenario:** Bei jedem neuen Eintrag in einer Google-Sheets-Tabelle soll automatisch eine Slack-Nachricht gesendet werden.

```
┌───────────────┐     ┌───────────────┐     ┌──────────────┐
│ Google Sheets │────▶│  Nachricht    │────▶│    Slack     │
│   Trigger     │     │  formatieren  │     │  Nachricht   │
│               │     │               │     │   senden     │
│ "Neuer Eintrag│     │ "Neuer Lead:  │     │  #vertrieb   │
│  erkannt"     │     │  {{Name}}"    │     │              │
└───────────────┘     └───────────────┘     └──────────────┘
```

**Schritte im Workflow:**
1. **Trigger:** Google Sheets überwacht die Tabelle auf neue Zeilen.
2. **Verarbeitung:** Die Daten werden zu einer lesbaren Nachricht formatiert.
3. **Aktion:** Die Nachricht wird im Slack-Kanal #vertrieb gepostet.

---

## Praxisbeispiel 2: Täglicher Bericht

**Szenario:** Jeden Morgen um 8:00 Uhr soll ein Bericht aus einer Datenbank erstellt und per E-Mail versendet werden.

```
┌───────────────┐     ┌───────────────┐     ┌───────────────┐
│   Schedule    │────▶│   Datenbank   │────▶│    E-Mail     │
│   Trigger     │     │   Abfrage     │     │   versenden   │
│               │     │               │     │               │
│ "Täglich      │     │ "SELECT ...   │     │ "Täglicher    │
│  08:00 Uhr"   │     │  FROM ..."    │     │  Bericht"     │
└───────────────┘     └───────────────┘     └───────────────┘
```

---

## Praxisbeispiel 3: KI-gestützte Verarbeitung

**Szenario:** Eingehende Kunden-E-Mails werden automatisch mit KI analysiert, kategorisiert und an die richtige Abteilung weitergeleitet.

```
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────────┐
│  E-Mail  │───▶│  OpenAI  │───▶│  IF/ELSE │───▶│ Weiterleitung│
│  Trigger │    │ Analyse  │    │ Routing  │    │ an Abteilung │
│          │    │          │    │          │    │              │
│"Neue     │    │"Kategorie│    │Beschwerde│    │ Support-Team │
│ E-Mail"  │    │ erkennen"│    │? Anfrage?│    │ oder Vertrieb│
└──────────┘    └──────────┘    └──────────┘    └──────────────┘
```

---

## Einen Workflow erstellen – Schritt für Schritt

### Schritt 1: Neuen Workflow anlegen
- Auf **„New Workflow"** klicken
- Dem Workflow einen aussagekräftigen Namen geben

### Schritt 2: Trigger auswählen
- Den ersten Node hinzufügen – das ist immer der **Trigger**
- Beispiele: Schedule (Zeitplan), Webhook, E-Mail-Eingang, Formular

### Schritt 3: Nodes hinzufügen
- Über das **„+"**-Symbol weitere Nodes hinzufügen
- Nodes per Drag & Drop auf der Arbeitsfläche anordnen
- Nodes durch Ziehen miteinander verbinden

### Schritt 4: Nodes konfigurieren
- Jeden Node anklicken und die **Parameter einstellen**
- Zugangsdaten (Credentials) für externe Dienste hinterlegen
- Datenfelder aus vorherigen Nodes per **Expressions** referenzieren

### Schritt 5: Testen
- Auf **„Execute Workflow"** klicken
- Die Ausgabe jedes einzelnen Nodes überprüfen
- Bei Fehlern: die betroffenen Nodes anpassen

### Schritt 6: Aktivieren
- Den Workflow über den **Toggle-Schalter** aktivieren
- Ab jetzt läuft der Workflow automatisch bei jedem Trigger-Ereignis

---

## Wichtige Node-Typen

### Trigger-Nodes (Auslöser)
| Node | Funktion |
|---|---|
| **Schedule Trigger** | Workflow zu bestimmten Zeiten starten |
| **Webhook** | Workflow durch externen HTTP-Aufruf starten |
| **Email Trigger (IMAP)** | Workflow bei neuer E-Mail starten |
| **App-Trigger** | z. B. „Neues Issue in GitHub" |

### Aktions-Nodes
| Node | Funktion |
|---|---|
| **HTTP Request** | Beliebige API aufrufen |
| **Send Email** | E-Mail versenden |
| **Set** | Daten setzen oder transformieren |
| **Code** | Eigenen JavaScript/Python-Code ausführen |

### Logik-Nodes
| Node | Funktion |
|---|---|
| **IF** | Bedingung prüfen (Wenn/Dann) |
| **Switch** | Mehrere Bedingungen prüfen |
| **Merge** | Daten aus mehreren Quellen zusammenführen |
| **Loop Over Items** | Über eine Liste iterieren |

---

## Expressions & Datenfluss

Expressions ermöglichen es, Daten zwischen Nodes weiterzugeben. Die Syntax nutzt doppelte geschweifte Klammern:

```
{{ $json.feldname }}
```

**Beispiele:**

| Expression | Beschreibung |
|---|---|
| `{{ $json.email }}` | E-Mail-Adresse aus dem vorherigen Node |
| `{{ $json.name.toUpperCase() }}` | Name in Großbuchstaben |
| `{{ $now.format('dd.MM.yyyy') }}` | Aktuelles Datum formatiert |
| `{{ $json.betrag > 1000 }}` | Prüfung ob Betrag größer als 1000 |

**Tipp:** Im Node-Editor gibt es einen **Expression-Editor** mit Autovervollständigung – man muss die Syntax nicht auswendig kennen!

---

## Credentials (Zugangsdaten)

Um externe Dienste zu nutzen, müssen Zugangsdaten hinterlegt werden:

1. In den **Einstellungen** → **Credentials** navigieren
2. **Neue Credentials anlegen** für den gewünschten Dienst
3. Die Authentifizierung durchführen (z. B. OAuth, API-Key)
4. Die Credentials werden **verschlüsselt** gespeichert
5. Im Node die gespeicherten Credentials auswählen

**Wichtig:** Credentials werden zentral verwaltet und können in mehreren Workflows wiederverwendet werden.

---

## Fehlerbehandlung

n8n bietet mehrere Möglichkeiten, mit Fehlern umzugehen:

### Error Workflow
Ein spezieller Workflow, der bei Fehlern in anderen Workflows ausgelöst wird. Damit kann man sich z. B. per E-Mail oder Slack über Fehler benachrichtigen lassen.

### Retry on Fail
Nodes können so konfiguriert werden, dass sie bei einem Fehler **automatisch erneut versucht** werden – mit konfigurierbarer Anzahl und Wartezeit.

### Error Output
Seit neueren Versionen können Nodes einen **separaten Fehler-Ausgang** haben, um Fehler gezielt im Workflow zu behandeln, statt den gesamten Workflow abzubrechen.

---

## Hosting-Optionen

### Self-Hosted (eigener Server)
- **Volle Kontrolle** über Daten und Infrastruktur
- Ideal für Unternehmen mit strengen Datenschutzanforderungen (DSGVO)
- Installation via Docker, npm oder direkt auf einem Server
- Kostenlos in der Community Edition

### n8n Cloud
- **Gehostet von n8n** – kein Server-Management nötig
- Schneller Einstieg ohne technisches Setup
- Verschiedene Preispläne je nach Nutzungsumfang
- EU-Hosting verfügbar

---

## Best Practices

### Benennung & Organisation
- Workflows und Nodes **aussagekräftig benennen**
- Zusammengehörige Workflows mit **Tags** organisieren
- Komplexe Workflows mit **Sticky Notes** dokumentieren

### Sicherheit
- Credentials niemals im Workflow-Code hartcodieren
- **Zugriffsrechte** für Workflows und Credentials einschränken
- Sensible Daten möglichst nicht in Logs speichern

### Entwicklung & Test
- Workflows erst **manuell testen**, bevor man sie aktiviert
- Einen **Error Workflow** einrichten für Benachrichtigungen bei Fehlern
- Für komplexe Workflows: in **kleinere Sub-Workflows** aufteilen

### Performance
- **Batch-Verarbeitung** nutzen statt einzelner Elemente
- Unnötige Nodes vermeiden – jeder Node kostet Ausführungszeit
- Bei großen Datenmengen: Daten **filtern** bevor sie weiterverarbeitet werden

---

## Nützliche Ressourcen

| Ressource | Link |
|---|---|
| **Offizielle Dokumentation** | docs.n8n.io |
| **n8n Community Forum** | community.n8n.io |
| **Workflow-Templates** | n8n.io/workflows |
| **YouTube-Kanal** | youtube.com/@n8n-io |
| **GitHub Repository** | github.com/n8n-io/n8n |

---

## Zusammenfassung

- **n8n** ist ein leistungsstarkes, Open-Source-Tool zur Workflow-Automatisierung
- Workflows werden **visuell** per Drag & Drop erstellt – keine Programmierung nötig
- Über **400+ Integrationen** verbinden nahezu alle gängigen Tools und Dienste
- **Self-Hosting** ermöglicht volle Datenkontrolle und DSGVO-Konformität
- Die **Community Edition** ist kostenlos nutzbar
- Ideal für Teams, die wiederkehrende Aufgaben automatisieren und Zeit sparen möchten

---

## Nächste Schritte

1. **Ausprobieren:** n8n Cloud-Account erstellen oder lokal installieren
2. **Templates erkunden:** Fertige Workflow-Vorlagen als Inspiration nutzen
3. **Ersten Workflow bauen:** Mit einem einfachen Anwendungsfall starten
4. **Community beitreten:** Im Forum Fragen stellen und von anderen lernen
5. **Erweitern:** Schritt für Schritt komplexere Workflows aufbauen

---

*Viel Erfolg mit n8n! 🚀*
