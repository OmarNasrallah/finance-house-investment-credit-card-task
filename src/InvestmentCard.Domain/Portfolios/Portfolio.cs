using InvestmentCard.Domain.Common;

namespace InvestmentCard.Domain.Portfolios;

public class Portfolio
{
    private readonly List<Lien> _liens = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public PortfolioType Type { get; private set; }
    public decimal TotalValue { get; private set; }
    public bool IsFrozen { get; private set; }

    public IReadOnlyCollection<Lien> Liens => _liens.AsReadOnly();

    public decimal LienAmount => _liens.Where(l => l.IsActive).Sum(l => l.Amount);
    public decimal AvailableValue => TotalValue - LienAmount;

    private Portfolio() { }

    public Portfolio(Guid id, Guid customerId, PortfolioType type, decimal totalValue)
    {
        if (totalValue < 0)
            throw new DomainException(ErrorCodes.InvalidAmount, "Portfolio value cannot be negative.");

        Id = id;
        CustomerId = customerId;
        Type = type;
        TotalValue = totalValue;
    }

    public Lien PlaceLien(decimal amount, DateTime nowUtc)
    {
        EnsureNotFrozen();
        EnsurePositive(amount);

        if (amount > AvailableValue)
            throw new DomainException(ErrorCodes.LienExceedsAvailable,
                $"Lien {amount:N2} is more than available value {AvailableValue:N2}.");

        var lien = new Lien(Id, amount, nowUtc);
        _liens.Add(lien);
        return lien;
    }

    public void ReleaseLien(Guid lienId, DateTime nowUtc)
    {
        var lien = _liens.SingleOrDefault(l => l.Id == lienId)
            ?? throw new DomainException(ErrorCodes.LienNotFound, $"Lien {lienId} not on this portfolio.");
        lien.Release(nowUtc);
    }

    public void Redeem(decimal amount)
    {
        EnsureNotFrozen();
        EnsurePositive(amount);

        if (amount > AvailableValue)
            throw new DomainException(ErrorCodes.RedemptionExceedsAvailable,
                $"Can't redeem {amount:N2}; only {AvailableValue:N2} free ({LienAmount:N2} under lien).");

        TotalValue -= amount;
    }

    public bool Freeze()
    {
        if (IsFrozen) return false;
        IsFrozen = true;
        return true;
    }

    private void EnsureNotFrozen()
    {
        if (IsFrozen)
            throw new DomainException(ErrorCodes.PortfolioFrozen,
                "Portfolio is frozen (delinquent card).");
    }

    private static void EnsurePositive(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException(ErrorCodes.InvalidAmount, "Amount must be greater than zero.");
    }
}
