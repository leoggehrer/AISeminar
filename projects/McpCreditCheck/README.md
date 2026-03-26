# MCP Credit Check

Ein ASP.NET Core Anwendung, die einen **Model Context Protocol (MCP)** Server mit Credit-Check-Funktionalität bereitstellt.

## Übersicht

Dieses Projekt implementiert einen MCP-Server, der AI-Agenten und anderen Clients ermöglicht, Kreditanfragen automatisiert zu prüfen. Der Server stellt ein Tool zur Verfügung, das anhand vordefinierter Geschäftsregeln entscheidet, ob ein Kredit genehmigt werden kann.

## Features

- **Credit Check Tool**: Automatisierte Kreditgenehmigungsprüfung
- **Schuldenquote-Berechnung**: Berechnet das Verhältnis zwischen monatlicher Rate und Einkommen
- **Regelbasierte Genehmigung**: Prüfung gegen konfigurierbare Geschäftsregeln
- **HTTP Transport**: MCP Server über HTTP erreichbar

## Geschäftsregeln für Kreditgenehmigung

Ein Kredit wird genehmigt, wenn **alle** folgenden Kriterien erfüllt sind:

| Kriterium        | Wert          | Beschreibung                                                  |
| ---------------- | ------------- | ------------------------------------------------------------- |
| Schuldenquote    | ≤ 35%         | Monatliche Rate darf max. 35% des Nettoeinkommens betragen    |
| Mindesteinkommen | ≥ 1.500 EUR   | Monatliches Nettoeinkommen muss mindestens 1.500 EUR betragen |
| Maximalbetrag    | ≤ 50.000 EUR  | Der angeforderte Kreditbetrag darf max. 50.000 EUR betragen   |
| Zinssatz         | 4,5% p.a.     | Verwendeter Jahreszins für Berechnung                        |

## Implementierung, Installation & Ausführung

### Voraussetzungen

- .NET 8.0 oder höher
- dotnet CLI

### Implementierung

**Projekt erstellen**

```bash
dotnet new web -n McpCreditCheck
cd McpCreditCheck
dotnet add package ModelContextProtocol.AspNetCore --prerelease
```

**Program.cs**

```csharp
using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<CreditCheckTools>();

var app = builder.Build();
app.MapMcp("/mcp");
app.Run();
```

**CreditCheckTools.cs**

```csharp
using ModelContextProtocol.Server;
using System.ComponentModel;

public class CreditCheckTools
{
    [McpServerTool]
    [Description("Prüft ob ein Kredit genehmigt werden kann")]
    public static string CheckCredit(
        [Description("Name des Kunden")] string customerName,
        [Description("Monatliches Nettoeinkommen in EUR")] decimal monthlyIncome,
        [Description("Gewünschter Kreditbetrag in EUR")] decimal loanAmount,
        [Description("Laufzeit in Monaten")] int durationMonths)
    {
        // Geschäftsregeln
        decimal monthlyRate = 0.045m / 12; // 4.5% Jahreszins
        decimal monthlyPayment = loanAmount * monthlyRate /
            (1 - (decimal)Math.Pow((double)(1 + monthlyRate), -durationMonths));

        decimal debtRatio = monthlyPayment / monthlyIncome;

        bool approved = debtRatio <= 0.35m && monthlyIncome >= 1500m && loanAmount <= 50000m;

        return $"""
            Kunde: {customerName}
            Kreditbetrag: {loanAmount:C}
            Monatliche Rate: {monthlyPayment:C}
            Schuldenquote: {debtRatio:P1}
            Entscheidung: {(approved ? "✅ GENEHMIGT" : "❌ ABGELEHNT")}
            Grund: {GetReason(approved, debtRatio, monthlyIncome, loanAmount)}
            """;
    }

    [McpServerTool, Description("Berechnet Zinsen und erstellt einen Tilgungsplan")]
    public static string CalculateInterest(
        [Description("Kreditbetrag in EUR")] decimal principal,
        [Description("Jährlicher Zinssatz in Prozent, z.B. 4.5")] decimal annualRatePercent,
        [Description("Laufzeit in Monaten")] int durationMonths,
        [Description("Tilgungsplan anzeigen? true/false")] bool showSchedule = false)
    {
        decimal monthlyRate = annualRatePercent / 100m / 12m;
        decimal monthlyPayment = principal * monthlyRate /
            (1 - (decimal)Math.Pow((double)(1 + monthlyRate), -durationMonths));

        decimal totalPayment = monthlyPayment * durationMonths;
        decimal totalInterest = totalPayment - principal;

        var result = $"""
            ── Zinsberechnung ──────────────────────
            Kreditbetrag:       {principal:C}
            Zinssatz:           {annualRatePercent:F2}% p.a.
            Laufzeit:           {durationMonths} Monate
            Monatliche Rate:    {monthlyPayment:C}
            Gesamtzahlung:      {totalPayment:C}
            Gesamtzinsen:       {totalInterest:C}
            Zinsanteil:         {totalInterest / totalPayment:P1}
            ────────────────────────────────────────
            """;

        if (showSchedule)
        {
            result += BuildSchedule(principal, monthlyRate, monthlyPayment, durationMonths);
        }

        return result;
    }

    private static string GetReason(bool approved, decimal ratio, decimal income, decimal amount)
    {
        if (approved) return "Alle Kriterien erfüllt";
        if (income < 1500m) return "Mindesteinkommen 1.500 EUR nicht erreicht";
        if (amount > 50000m) return "Maximalbetrag 50.000 EUR überschritten";
        if (ratio > 0.35m) return $"Schuldenquote {ratio:P1} übersteigt 35%-Grenze";
        return "Kriterien nicht erfüllt";
    }

    private static string BuildSchedule(decimal principal, decimal monthlyRate, decimal monthlyPayment, int months)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("\nTilgungsplan (erste 12 Monate):");
        sb.AppendLine($"{"Monat",-7} {"Rate",-12} {"Zinsen",-12} {"Tilgung",-12} {"Restschuld"}");
        sb.AppendLine(new string('─', 55));

        decimal balance = principal;
        int showMonths = Math.Min(months, 12);

        for (int i = 1; i <= showMonths; i++)
        {
            decimal interest = balance * monthlyRate;
            decimal repayment = monthlyPayment - interest;
            balance -= repayment;

            sb.AppendLine($"{i,-7} {monthlyPayment,8:C}    {interest,8:C}    {repayment,8:C}    {Math.Max(0, balance),10:C}");
        }

        if (months > 12)
            sb.AppendLine($"... (+ {months - 12} weitere Monate)");

        return sb.ToString();
    }

}
```

### Lokale Ausführung

```bash
cd McpCreditCheck
dotnet run
```

Der Server startet dann und ist standardmäßig unter `http://localhost:5250` erreichbar.

Der MCP-Endpoint ist unter `http://localhost:5250/mcp` zu finden.

#### Testen mit dem Postman

**initialize**:

```json
{
  "jsonrpc": "2.0",
  "id": "1",
  "method": "initialize",
  "params": {
    "protocolVersion": "2024-11-05",
    "clientInfo": { "name": "Postman", "version": "1.0" },
    "capabilities": {}
  }
}
```

> Hinweis: Im Response Header befindet sich die 'Mcp-Session-Id' 

**tools/list**:

```json
{
    "jsonrpc":"2.0",
    "id":1,
    "method":"tools/list"
}
```

> Hinweis: Im Header die 'Mcp-Session-Id' (Response Header initialize) eintragen

## Teil 2: n8n Workflow

### Aufbau
```
[Chat Trigger] → [AI Agent] → [MCP Client Tool Node]
                                      ↓
                              http://localhost:5000/mcp
```

### MCP Client Tool Node konfigurieren

| Feld | Wert |
|---|---|
| **Endpoint** | `http://localhost:5250/mcp` |
| **Authentication** | None |
| **Tools to Include** | All |

### AI Agent Node konfigurieren

- **Model:** Claude oder GPT-4
- **System Prompt:**

```text
Du bist ein Kreditberater. Nutze das CheckCredit-Tool um Kreditanfragen 
zu prüfen. Frage den Nutzer nach allen nötigen Informationen bevor du 
das Tool aufrufst.
```

## API - Credit Check Tool

### Eingabeparameter

```
CheckCredit(
  customerName: string,        # Name des Kunden
  monthlyIncome: decimal,      # Monatliches Nettoeinkommen in EUR
  loanAmount: decimal,         # Gewünschter Kreditbetrag in EUR
  durationMonths: int          # Laufzeit in Monaten
)
```

### Rückgabewert

Die Methode gibt einen formattierten Text zurück mit:

- Kundenname
- Kreditbetrag
- Berechnete monatliche Rate
- Schuldenquote
- Genehmigungsentscheidung (✅ GENEHMIGT / ❌ ABGELEHNT)
- Begründung der Entscheidung

### Beispiel

Nutzer schreibt:

„Ich möchte einen Kredit für Herrn Müller prüfen. Einkommen 2.800 EUR, Kredit 15.000 EUR, Laufzeit 36 Monate Der aktuelle Zinssatz beträgt 3.5 %."

**Input:**

```json
{
  "customerName": "Müller",
  "monthlyIncome": 2800,
  "loanAmount": 15000,
  "durationMonths": 36
}
```

**Output:**

```
Kunde: Müller
Kreditbetrag: €15.000
Monatliche Rate: €448,72
Schuldenquote: 16,0%
Entscheidung: ✅ GENEHMIGT
Grund: Alle Kriterien erfüllt
```

## Projektstruktur

```
McpCreditCheck/
├── Program.cs                 # MCP Server Setup
├── CreditCheckTools.cs       # Credit Check Tool Implementierung
├── McpCreditCheck.csproj     # Projektdatei
├── appsettings.json          # Allgemeine Konfiguration
├── appsettings.Development.json  # Development Konfiguration
└── README.md                 # Diese Datei
```

## Abhängigkeiten

- **ModelContextProtocol.AspNetCore**: MCP Server Framework für ASP.NET Core
