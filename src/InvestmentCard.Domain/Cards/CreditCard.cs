using InvestmentCard.Domain.Common;
using InvestmentCard.Domain.Portfolios;

namespace InvestmentCard.Domain.Cards;

public enum CardStatus
{
    Active = 1,
    Blocked = 2,
    Delinquent = 3,
    Closed = 4
}

public class CreditCard
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid PortfolioId { get; private set; }
    public Guid LienId { get; private set; }
    public string CardNumber { get; private set; } = string.Empty;
    public decimal CreditLimit { get; private set; }
    public decimal UtilizedAmount { get; private set; }
    public CardStatus Status { get; private set; }
    public DateTime IssuedAtUtc { get; private set; }

    // set when we hit the limit; cleared on repay — used by the delinquency job
    public DateTime? FullyUtilizedSinceUtc { get; private set; }

    public decimal AvailableCredit => CreditLimit - UtilizedAmount;
    public bool IsAtRisk => UtilizedAmount == CreditLimit;

    private CreditCard() { }

    public static CreditCard Issue(Guid customerId, Portfolio portfolio, Lien lien, decimal creditLimit,
        string cardNumber, DateTime nowUtc)
    {
        if (lien.PortfolioId != portfolio.Id || !lien.IsActive)
            throw new DomainException(ErrorCodes.LienNotFound, "Need an active lien on this portfolio.");
        if (creditLimit <= 0)
            throw new DomainException(ErrorCodes.InvalidAmount, "Credit limit must be greater than zero.");
        if (creditLimit > lien.Amount)
            throw new DomainException(ErrorCodes.CreditLimitExceedsLien,
                $"Limit {creditLimit:N2} can't exceed lien {lien.Amount:N2}.");

        return new CreditCard
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            PortfolioId = portfolio.Id,
            LienId = lien.Id,
            CardNumber = cardNumber,
            CreditLimit = creditLimit,
            Status = CardStatus.Active,
            IssuedAtUtc = nowUtc
        };
    }

    public void Charge(decimal amount, DateTime nowUtc)
    {
        if (Status != CardStatus.Active)
            throw new DomainException(ErrorCodes.CardNotActive, $"Card is {Status}; transactions are blocked.");
        if (amount <= 0)
            throw new DomainException(ErrorCodes.InvalidAmount, "Amount must be greater than zero.");
        if (UtilizedAmount + amount > CreditLimit)
            throw new DomainException(ErrorCodes.CreditLimitExceeded,
                $"Charge {amount:N2} would go over the limit (available {AvailableCredit:N2}).");

        UtilizedAmount += amount;
        if (IsAtRisk)
            FullyUtilizedSinceUtc ??= nowUtc;
    }

    public void Repay(decimal amount)
    {
        if (Status == CardStatus.Closed)
            throw new DomainException(ErrorCodes.CardNotActive, "Card is closed.");
        if (amount <= 0)
            throw new DomainException(ErrorCodes.InvalidAmount, "Amount must be greater than zero.");
        if (amount > UtilizedAmount)
            throw new DomainException(ErrorCodes.RepaymentExceedsBalance,
                $"Repay {amount:N2} is more than the balance ({UtilizedAmount:N2}).");

        UtilizedAmount -= amount;
        FullyUtilizedSinceUtc = null;
    }

    public bool IsOverdue(DateTime nowUtc, TimeSpan threshold) =>
        Status == CardStatus.Active
        && IsAtRisk
        && FullyUtilizedSinceUtc is { } since
        && nowUtc - since >= threshold;

    public bool MarkDelinquent()
    {
        if (Status == CardStatus.Delinquent) return false;
        Status = CardStatus.Delinquent;
        return true;
    }

    public void Close()
    {
        if (Status is CardStatus.Closed or CardStatus.Delinquent)
            throw new DomainException(ErrorCodes.CardNotActive, $"Can't close a {Status} card.");
        if (UtilizedAmount > 0)
            throw new DomainException(ErrorCodes.CardHasBalance,
                $"Still owe {UtilizedAmount:N2} — repay before closing.");

        Status = CardStatus.Closed;
    }
}
