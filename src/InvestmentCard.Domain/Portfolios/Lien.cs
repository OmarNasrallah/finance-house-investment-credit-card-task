namespace InvestmentCard.Domain.Portfolios;

public enum LienStatus
{
    Active = 1,
    Released = 2
}

public class Lien
{
    public Guid Id { get; private set; }
    public Guid PortfolioId { get; private set; }
    public decimal Amount { get; private set; }
    public LienStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ReleasedAtUtc { get; private set; }

    public bool IsActive => Status == LienStatus.Active;

    private Lien() { }

    internal Lien(Guid portfolioId, decimal amount, DateTime nowUtc)
    {
        Id = Guid.NewGuid();
        PortfolioId = portfolioId;
        Amount = amount;
        Status = LienStatus.Active;
        CreatedAtUtc = nowUtc;
    }

    internal void Release(DateTime nowUtc)
    {
        if (!IsActive) return;
        Status = LienStatus.Released;
        ReleasedAtUtc = nowUtc;
    }
}
