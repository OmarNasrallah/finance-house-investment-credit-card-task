namespace InvestmentCard.Domain.Portfolios;

public enum PortfolioType
{
    Basic = 1,
    Plus = 2,
    Premium = 3
}

public static class PortfolioTypeExtensions
{
    public static decimal AdvanceRate(this PortfolioType type) => type switch
    {
        PortfolioType.Basic => 0.50m,
        PortfolioType.Plus => 0.70m,
        PortfolioType.Premium => 0.90m,
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}
