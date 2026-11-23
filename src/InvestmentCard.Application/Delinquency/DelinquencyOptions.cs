namespace InvestmentCard.Application.Delinquency;

public sealed class DelinquencyOptions
{
    public const string SectionName = "Delinquency";

    // kept short for the demo so you can actually see it fire
    public int ThresholdMinutes { get; set; } = 2;

    public string Cron { get; set; } = "0 * * * *";
}
