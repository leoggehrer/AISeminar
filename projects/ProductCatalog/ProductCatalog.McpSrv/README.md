# ProductCatalog.McpSrv - Model Context Protocol Server

Ein **Model Context Protocol (MCP) Server** für das ProductCatalog-Projekt, der AI-Assistenten direkten Zugriff auf Backend-Funktionen ermöglicht.

## 🎯 Zweck

Dieser MCP-Server stellt Tools bereit, die von AI-Assistenten (wie Claude, GitHub Copilot, etc.) genutzt werden können, um:
- Berechnungen durchzuführen
- Geschäftslogik auszuführen
- Auf Backend-Services zuzugreifen
- Validierungen und Operationen zu testen

## 🚀 Quickstart

```bash
# Server starten
dotnet run --project ProductCatalog.McpSrv

# Server läuft auf: http://localhost:5000/mcp
```

## 📦 Installation

### Erforderliche NuGet-Packages

```bash
# MCP SDK
dotnet add package ModelContextProtocol --version 0.5.0-preview.1

# ASP.NET Core Integration
dotnet add package ModelContextProtocol.AspNetCore --version 0.5.0-preview.1
```

### Projekt-Referenz

```bash
# ProductCatalog.Logic referenzieren
dotnet add reference ../ProductCatalog.Logic/ProductCatalog.Logic.csproj
```

## 📋 Verfügbare Tools

### calculate_grade
Berechnet eine Schülernote basierend auf Punkten.

**Parameter:**
- `points` (double) - Erreichte Punkte
- `maxPoints` (double) - Maximale Punkte

**Rückgabe:**
```json
{
  "percentage": 85.5,
  "grade": "2 (Good)"
}
```

## 🔧 Entwicklung

### Neues Tool hinzufügen

1. **Erstelle eine neue Klasse** in `Servers/`:

```csharp
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace ProductCatalog.McpSrv.Servers;

[McpServerToolType]
public static class MyTool
{
    [McpServerTool(Name = "my_tool")]
    [Description("Tool description for AI")]
    public static MyResult ExecuteTool(
        [Description("Parameter description")] string param1)
    {
        // Implementation
        return new MyResult();
    }
}
```

2. **Erstelle Model** in `Models/` (falls benötigt):

```csharp
namespace ProductCatalog.McpSrv.Models;

public class MyResult
{
    public string Message { get; set; } = string.Empty;
}
```

3. **Starte den Server neu** - Tools werden automatisch registriert

## 🏗️ Architektur

```
ProductCatalog.McpSrv
├── Program.cs              # Server-Konfiguration
├── Servers/                # MCP Tool-Implementierungen
│   └── GradCalculator.cs   # Beispiel-Tool
├── Models/                 # Datenmodelle für Requests/Responses
└── appsettings.json        # Konfiguration
```

## 📦 Abhängigkeiten

- **ModelContextProtocol** (0.5.0-preview.1) - MCP SDK
- **ModelContextProtocol.AspNetCore** - ASP.NET Core Integration
- **ProductCatalog.Logic** - Backend-Logik und Datenzugriff

## 🔌 Integration

### In Claude Desktop

```json
{
  "mcpServers": {
    "ProductCatalog": {
      "url": "http://localhost:5000/mcp"
    }
  }
}
```

### In VS Code (GitHub Copilot)

Der Server wird automatisch erkannt, wenn er läuft.

## 📚 Best Practices

✅ **DO:**
- Tools als statische Methoden implementieren
- Aussagekräftige Descriptions für AI verwenden
- Parameter validieren und Exceptions werfen
- XML-Dokumentation für Developer-Experience

❌ **DON'T:**
- Keine langen, blockierenden Operationen
- Keine Secrets in Tool-Responses
- Keine komplexen Objekte als Parameter (nur Primitives, DTOs)

## 🧪 Testing

```bash
# Mit curl testen
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "tool": "calculate_grade",
    "parameters": {
      "points": 85,
      "maxPoints": 100
    }
  }'
```

## 📖 Weitere Ressourcen

- [Model Context Protocol Specification](https://spec.modelcontextprotocol.io/)
- [ProductCatalog Hauptdokumentation](../README.md)
- [MCP .NET SDK Documentation](https://github.com/modelcontextprotocol/dotnet-sdk)

---

**Version:** 1.0 | **Framework:** .NET 8.0 | **MCP:** 0.5.0-preview.1
