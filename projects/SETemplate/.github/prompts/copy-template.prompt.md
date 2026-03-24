# Template kopieren und in neuer VS Code Instanz öffnen

Dieser Prompt kopiert das SETemplate in ein neues Projektverzeichnis und öffnet es in einer neuen VS Code Instanz.

## Voraussetzung

Frage den Benutzer nach folgenden Angaben, bevor du fortfährst:

| Parameter | Beschreibung | Beispiel |
|-----------|-------------|---------|
| **Zielverzeichnis** | Übergeordneter Ordner, in dem das neue Projekt erstellt wird | `/Users/name/source/repos/MyCompany` |
| **Projektname** | Name des neuen Projekts (wird als Ordner- und Solution-Name verwendet) | `MyNewProject` |

---

## Schritt 1 – Template kopieren

Ersetze `<ZIELVERZEICHNIS>` und `<PROJEKTNAME>` durch die Angaben des Benutzers:

```bash
dotnet run --project TemplateTools.ConApp -- \
  TargetSolutionSubPath=<ZIELVERZEICHNIS> \
  TargetSolutionName=<PROJEKTNAME> \
  AppArg=copier,start,exit,exit
```

**Beispiel:**
```bash
dotnet run --project TemplateTools.ConApp -- \
  TargetSolutionSubPath=/Users/name/source/repos/MyCompany \
  TargetSolutionName=MyNewProject \
  AppArg=copier,start,exit,exit
```

> Der Kopiervorgang schlägt fehl, wenn das Zielverzeichnis bereits existiert.  
> Soll ein bestehendes Verzeichnis überschrieben werden, füge `Force=True` hinzu.

---

## Schritt 2 – Neues Projekt in VS Code öffnen

Nach erfolgreichem Kopieren das neue Projekt in einer **neuen VS Code Instanz** öffnen.

### Standard (macOS, zuverlässig)

```bash
TARGET="$(cd <ZIELVERZEICHNIS>/<PROJEKTNAME> && pwd)"
"/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code" --new-window "$TARGET"
```

**Beispiel:**
```bash
TARGET="$(cd /Users/name/source/repos/MyCompany/MyNewProject && pwd)"
"/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code" --new-window "$TARGET"
```

### Alternative (wenn `code` im PATH verfügbar ist)

```bash
code --new-window <ZIELVERZEICHNIS>/<PROJEKTNAME>
```

**Beispiel:**
```bash
code --new-window /Users/name/source/repos/MyCompany/MyNewProject
```

---

## Hinweise

- Das kopierte Projekt ist ein vollständiges SETemplate mit allen Projekten (Logic, WebApi, ConApp, AngularApp, etc.)
- Alle Dateien mit `//@BaseCode`-Marker werden exakt kopiert
- Nach dem Öffnen empfiehlt sich der **full-workflow** Prompt, um das neue Projekt korrekt einzurichten (Datenbank, Auth, erste Entity)
- Die `TemplateTools.ConApp` muss im Quell-Workspace (`SETemplateDeploment`) ausgeführt werden
- Falls `zsh: command not found: code` erscheint, in VS Code den Befehl **Shell Command: Install 'code' command in PATH** ausführen
