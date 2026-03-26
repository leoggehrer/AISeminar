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
        decimal monthlyRate = 0.045m / 12;
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

    [McpServerTool]
    [Description("Berechnet Zinsen und erstellt einen Tilgungsplan")]
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

    #region helpers
    private static string GetReason(bool approved, decimal ratio, decimal income, decimal amount)
    {
        if (approved) return "Alle Kriterien erfüllt";
        if (income < 1500m) return "Mindesteinkommen 1.500 EUR nicht erreicht";
        if (amount > 50000m) return "Maximalbetrag 50.000 EUR überschritten";
        if (ratio > 0.35m) return $"Schuldenquote {ratio:P1} übersteigt 35%-Grenze";
        return "Kriterien nicht erfüllt";
    }

    private static string BuildSchedule(
        decimal principal, decimal monthlyRate,
        decimal monthlyPayment, int months)
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
    #endregion helpers
}
