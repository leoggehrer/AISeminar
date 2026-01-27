# Marp Präsentation - Anleitung

## Installation

### Marp Extension für VS Code
1. Öffnen Sie VS Code
2. Installieren Sie die Extension: **Marp for VS Code** (ID: `marp-team.marp-vscode`)
3. Oder via Command Line:
   ```bash
   code --install-extension marp-team.marp-vscode
   ```

### Marp CLI (Optional - für Export außerhalb VS Code)
```bash
npm install -g @marp-team/marp-cli
```

## Verwendung in VS Code

### Präsentation anzeigen
1. Öffnen Sie `README.md` in VS Code
2. Klicken Sie auf das Marp-Icon in der oberen rechten Ecke
3. Oder: `Cmd+Shift+P` → "Marp: Open Preview"

### Export als PDF/HTML/PPTX

#### In VS Code:
1. `Cmd+Shift+P` → "Marp: Export Slide Deck"
2. Format wählen: PDF, HTML, oder PPTX

#### Via CLI:
```bash
# Als PDF exportieren
marp README.md.md -o presentation.pdf

# Als HTML exportieren
marp README.md.md -o presentation.html

# Als PowerPoint exportieren
marp README.md.md -o presentation.pptx

# Mit eigenem Theme
marp README.md.md --theme custom-theme.css -o presentation.pdf
```

## Marp-Features in dieser Präsentation

### YAML Front Matter
```yaml
---
marp: true                # Aktiviert Marp
theme: default            # Theme (default, gaia, uncover)
paginate: true            # Seitenzahlen
header: 'Text'            # Header auf jeder Folie
footer: 'Text'            # Footer auf jeder Folie
style: |                  # Custom CSS
  section { ... }
---
```

### Spezielle Direktiven

#### `<!-- _class: lead -->`
Zentriert den Inhalt vertikal und horizontal - ideal für Titelfolien

#### `<!-- _paginate: false -->`
Versteckt Seitenzahl auf dieser Folie

#### `<!-- fit -->`
Macht Text so groß wie möglich (für kurze, wichtige Aussagen)

### Slide-Trennung
Neue Folie beginnt immer mit `---`

## Tipps für beste Ergebnisse

### 1. Nicht zu viel Text pro Folie
- Maximal 5-7 Bulletpoints
- Kurze Sätze
- Große Schrift

### 2. Code-Blöcke
```python
# Code wird automatisch schön formatiert
def hello():
    print("Hello Marp!")
```

### 3. Tabellen
Werden automatisch formatiert, aber Schriftgröße kann angepasst werden im CSS

### 4. Bilder
```markdown
![width:500px](image.png)       # Feste Breite
![height:300px](image.png)      # Feste Höhe
![bg](background.jpg)           # Als Hintergrund
![bg right](image.png)          # Rechte Hälfte
```

### 5. Zweispalten-Layout
```markdown
<div class="columns">
<div>

Linke Spalte

</div>
<div>

Rechte Spalte

</div>
</div>
```

## Themes anpassen

### Eigenes Theme erstellen
Erstellen Sie eine CSS-Datei (z.B. `custom-theme.css`):

```css
/* @theme custom */

section {
  background-color: #1e1e1e;
  color: #ffffff;
}

h1 {
  color: #4ec9b0;
}

/* ... weitere Styles */
```

Dann im YAML Front Matter:
```yaml
theme: custom
```

## Präsentationsmodus

### Während der Präsentation:
- **Pfeiltasten**: Vor/Zurück
- **F**: Fullscreen
- **P**: Presenter View (mit Notizen)
- **ESC**: Beenden

### Speaker Notes
Fügen Sie Notizen hinzu, die nur Sie sehen:
```markdown
---

# Meine Folie

<!-- 
Notizen für den Vortragenden:
- Punkt 1 erwähnen
- Frage ans Publikum
-->
```

## Nächste Schritte

1. ✅ Marp Extension installieren
2. ✅ `README.md.md` in VS Code öffnen
3. ✅ Preview anschauen (Marp-Icon klicken)
4. ✅ Als PDF exportieren für Backup
5. ✅ Presenter View testen

## Mermaid-Diagramme in Marp

Marp unterstützt Mermaid **nicht nativ**, aber es gibt mehrere Workarounds:

### Option 1: HTML Export mit Mermaid (Empfohlen)

1. **HTML aktivieren im Front Matter:**
```yaml
---
marp: true
html: true
---
```

2. **Mermaid via CDN einbinden:**
```markdown
<script type="module">
  import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
  mermaid.initialize({ startOnLoad: true });
</script>

<div class="mermaid">
graph TD
    A[Start] --> B{Entscheidung}
    B -->|Ja| C[Ergebnis 1]
    B -->|Nein| D[Ergebnis 2]
</div>
```

⚠️ **Wichtig:** Funktioniert nur im HTML-Export, nicht in PDF/PPTX!

### Option 2: Mermaid als Bild exportieren

1. **Online-Tool nutzen:**
   - https://mermaid.live/
   - Diagramm erstellen und als PNG/SVG exportieren

2. **In Folie einbinden:**
```markdown
![width:800px](diagrams/workflow.png)
```

### Option 3: Mermaid CLI

```bash
# Mermaid CLI installieren
npm install -g @mermaid-js/mermaid-cli

# Diagramm generieren
mmdc -i diagram.mmd -o diagram.png -w 1920 -H 1080 -b transparent
```

**diagram.mmd:**
```
graph LR
    A[Python] --> B[n8n]
    B --> C[OpenAI]
    C --> D[Email]
```

### Option 4: VS Code Extension (Beste Lösung für Workflow)

Installieren Sie: **Markdown Preview Extrajs**

```bash
code --install-extension morish000.vscode-markdown-extrajs
```

Diese Extension fügt automatisch Mermaid-Unterstützung zu Markdown und Marp hinzu!

### Beispiel-Diagramme für Ihre Präsentation

#### Workflow-Diagramm (für n8n-Teil)
```markdown
<div class="mermaid">
graph TD
    A[Trigger: Neue Email] --> B[PDF herunterladen]
    B --> C[Text extrahieren]
    C --> D[AI-Analyse]
    D --> E[Feedback generieren]
    E --> F[Email senden]
</div>
```

#### Architektur-Diagramm (für Hybrid-Teil)
```markdown
<div class="mermaid">
graph TB
    subgraph Frontend
        A[React App]
    end
    subgraph Orchestration
        B[n8n Workflows]
    end
    subgraph Backend
        C[FastAPI]
        D[PostgreSQL]
    end
    subgraph AI
        E[OpenAI]
        F[Ollama]
    end
    A --> B
    B --> C
    B --> E
    B --> F
    C --> D
</div>
```

#### Sequenz-Diagramm (für AI-Prompt-Flow)
```markdown
<div class="mermaid">
sequenceDiagram
    participant U as User
    participant N as n8n
    participant AI as OpenAI
    participant E as Email
    
    U->>N: Upload Hausaufgabe
    N->>AI: Sende Prompt + Text
    AI->>N: Feedback JSON
    N->>E: Formatiere Email
    E->>U: Sende Feedback
</div>
```

### Empfehlung für Ihre Präsentation

**Für Live-Präsentation:**
- Nutzen Sie vorher generierte PNG-Bilder (Option 2)
- Garantiert, dass alles funktioniert
- Sieht in PDF/PPTX gleich aus

**Für HTML-Version:**
- Nutzen Sie Mermaid mit HTML (Option 1)
- Interaktiv und modern
- Gut für Online-Veröffentlichung

### Quick-Start Script

Erstellen Sie `generate-diagrams.sh`:

```bash
#!/bin/bash
mkdir -p diagrams

# Workflow-Diagramm
cat > diagrams/workflow.mmd << 'EOF'
graph TD
    A[Google Drive] --> B[PDF Download]
    B --> C[Text Extraktion]
    C --> D[AI Analyse]
    D --> E[Feedback Email]
EOF

# Generieren
mmdc -i diagrams/workflow.mmd -o diagrams/workflow.png -w 1920 -H 1080 -b transparent

echo "Diagramme generiert in ./diagrams/"
```

```bash
chmod +x generate-diagrams.sh
./generate-diagrams.sh
```

## Ressourcen

- **Marp Dokumentation**: https://marpit.marp.app/
- **Marp CLI**: https://github.com/marp-team/marp-cli
- **VS Code Extension**: https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode
- **Theme Gallery**: https://github.com/marp-team/marp-themes
- **Mermaid Live Editor**: https://mermaid.live/
- **Mermaid CLI**: https://github.com/mermaid-js/mermaid-cli
- **Markdown Preview Extrajs**: https://marketplace.visualstudio.com/items?itemName=morish000.vscode-markdown-extrajs

## Troubleshooting

### Preview zeigt nichts an
- Prüfen Sie, ob `marp: true` im Front Matter steht
- Extension neu laden: `Cmd+Shift+P` → "Reload Window"

### Export funktioniert nicht
- Marp CLI installieren: `npm install -g @marp-team/marp-cli`
- Chromium wird für PDF-Export benötigt (wird automatisch installiert)

### Styles werden nicht angewendet
- Prüfen Sie die YAML-Syntax im Front Matter
- CSS im `style: |` Block muss korrekt eingerückt sein

### Mermaid-Diagramme werden nicht angezeigt
- **In Preview:** Installieren Sie "Markdown Preview Extrajs" Extension
- **Im HTML-Export:** Stellen Sie sicher, dass `html: true` im Front Matter steht
- **In PDF/PPTX:** Mermaid funktioniert nicht - nutzen Sie vorher generierte PNG-Bilder

Viel Erfolg mit Ihrer Marp-Präsentation! 🎯
