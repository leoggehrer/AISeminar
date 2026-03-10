# Software-Entwicklung mit GitHub Copilot (`copilot-instructions.md`)

## Was ist GitHub Copilot?

GitHub Copilot ist ein **KI-gestützter Coding-Assistent**, der direkt in der IDE integriert ist.

- Vervollständigt Code in Echtzeit (inline suggestions)
- Generiert ganze Funktionen, Tests und Dokumentation
- Unterstützt alle gängigen Sprachen & Frameworks
- Basiert auf OpenAI-Modellen (GPT-4 / Codex)

> 💡 Copilot lernt aus dem Kontext – je mehr Kontext, desto bessere Vorschläge.

---

## Copilot im Entwickleralltag

### Wo Copilot hilft

| Aufgabe | Copilot-Unterstützung |
|---|---|
| Neue Funktion schreiben | Autovervollständigung & Codegenerierung |
| Unit Tests erstellen | Automatische Testgenerierung |
| Code erklären lassen | Inline-Kommentare & Chat |
| Refactoring | Umstrukturierungsvorschläge |
| Dokumentation | JSDoc / Docstrings generieren |

### Typischer Workflow

```
1. Kommentar schreiben → Copilot schlägt Implementierung vor
2. Funktionssignatur tippen → Copilot vervollständigt den Body
3. Im Chat fragen → Copilot erklärt oder debuggt
```

---

## Das Problem: Copilot kennt euer Projekt nicht

Copilot sieht nur den **aktuellen Dateikontext** – es weiß nichts über:

- 📁 Projektstruktur und Architektur
- 🧩 Verwendete Bibliotheken und Frameworks
- 📏 Code-Konventionen und Naming-Regeln
- 🔒 Sicherheits- oder Compliance-Anforderungen
- 🗂️ Bevorzugte Patterns und Anti-Patterns

**Lösung:** `copilot-instructions.md`

---

## Was ist `copilot-instructions.md`?

Eine **Konfigurationsdatei im Repository**, die GitHub Copilot projektspezifischen Kontext gibt.

```
.github/
└── copilot-instructions.md   ← hier liegt die Datei
```

- Wird automatisch von Copilot Chat gelesen
- Gilt für alle Entwickler:innen im Team
- Versioniert im Git-Repository
- Kein Plugin oder Setup nötig

> ✅ Einmal gepflegt – wirkt für das gesamte Team.

---

## Aufbau einer `copilot-instructions.md`

```markdown
# Copilot Instructions

## Projektüberblick
Dieses Projekt ist eine REST-API für ein E-Commerce-System.
Backend: Node.js (TypeScript) mit Express.
Datenbank: PostgreSQL via Prisma ORM.

## Code-Konventionen
- Variablen und Funktionen: camelCase
- Klassen: PascalCase
- Konstanten: SCREAMING_SNAKE_CASE
- Async/Await bevorzugen, kein .then()/.catch()

## Architektur
- Controller → Service → Repository Pattern
- Kein Business-Logik in Controllern
- Fehlerbehandlung über zentralen ErrorHandler

## Testing
- Jest für Unit- und Integrationstests
- Mindest-Coverage: 80 %
- Mocks im Ordner __mocks__/

## Verbotene Patterns
- Kein `any` in TypeScript
- Kein direkter DB-Zugriff in Controllern
- Keine synchronen Datei-Operationen
```

---

## Wirkung auf Copilot-Vorschläge

### Ohne `copilot-instructions.md`

```typescript
// Generischer Vorschlag – ignoriert Projektkontext
app.get('/users', (req, res) => {
  db.query('SELECT * FROM users').then(result => {
    res.json(result);
  });
});
```

### Mit `copilot-instructions.md`

```typescript
// Vorschlag folgt Projektkonventionen
app.get('/users', async (req, res, next) => {
  try {
    const users = await userService.findAll();
    res.json(users);
  } catch (error) {
    next(error); // zentraler ErrorHandler
  }
});
```

---

## Best Practices für die Instructions-Datei

### ✅ DO

- **Konkret sein** – "Verwende async/await" statt "schreib guten Code"
- **Beispiele einbauen** – Kurze Code-Snippets verdeutlichen Regeln
- **Regelmäßig aktualisieren** – Datei wie Code behandeln (Reviews, PRs)
- **Team einbeziehen** – Gemeinsam definierte Standards erhöhen Akzeptanz

### ❌ DON'T

- Zu lang machen – Fokus auf das Wesentliche
- Redundante Informationen aus der Doku wiederholen
- Geheime Informationen (Tokens, Passwörter) eintragen
- Widersprüchliche Regeln aufführen

---

## Erweiterte Möglichkeiten

### Mehrere Instruction-Dateien (VS Code)

In VS Code können zusätzlich `.github/prompts/*.prompt.md`-Dateien für spezifische Aufgaben angelegt werden:

```
.github/
├── copilot-instructions.md        # Globale Regeln
└── prompts/
    ├── create-component.prompt.md # React-Komponente erstellen
    ├── write-test.prompt.md       # Tests schreiben
    └── api-endpoint.prompt.md     # REST-Endpoint erstellen
```

### Kombination mit `.editorconfig` und Linting

```
copilot-instructions.md  →  Kontext & Architektur
.editorconfig            →  Formatierung
eslint / prettier        →  Code-Qualität (automatisch)
```

---

## Zusammenfassung

```
GitHub Copilot  +  copilot-instructions.md
      │                      │
  KI-Assistent         Projektkontext
      │                      │
      └──────────┬───────────┘
                 │
         🚀 Konsistenter, projektkonformer Code
            für das gesamte Entwicklerteam
```

**Die drei wichtigsten Takeaways:**

1. `copilot-instructions.md` liegt unter `.github/` und wird automatisch eingelesen
2. Konventionen, Architektur und Verbote klar und konkret formulieren
3. Die Datei regelmäßig wie Code reviewen und aktualisieren

---

*Erstellt mit GitHub Copilot & Claude · 2026*
