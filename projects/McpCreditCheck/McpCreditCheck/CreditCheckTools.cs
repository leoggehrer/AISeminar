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

    private static string GetReason(bool approved, decimal ratio, decimal income, decimal amount)
    {
        if (approved) return "Alle Kriterien erfüllt";
        if (income < 1500m) return "Mindesteinkommen 1.500 EUR nicht erreicht";
        if (amount > 50000m) return "Maximalbetrag 50.000 EUR überschritten";
        if (ratio > 0.35m) return $"Schuldenquote {ratio:P1} übersteigt 35%-Grenze";
        return "Kriterien nicht erfüllt";
    }
}