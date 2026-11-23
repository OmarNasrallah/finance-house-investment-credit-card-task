namespace InvestmentCard.Domain.Common;

public sealed class DomainException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

public static class ErrorCodes
{
    public const string InvalidAmount = "INVALID_AMOUNT";
    public const string PortfolioFrozen = "PORTFOLIO_FROZEN";
    public const string LienExceedsAvailable = "LIEN_EXCEEDS_AVAILABLE_VALUE";
    public const string LienNotFound = "LIEN_NOT_FOUND";
    public const string RedemptionExceedsAvailable = "REDEMPTION_EXCEEDS_AVAILABLE_VALUE";
    public const string CreditLimitExceedsLien = "CREDIT_LIMIT_EXCEEDS_LIEN";
    public const string CreditLimitExceeded = "CREDIT_LIMIT_EXCEEDED";
    public const string CardNotActive = "CARD_NOT_ACTIVE";
    public const string RepaymentExceedsBalance = "REPAYMENT_EXCEEDS_BALANCE";
    public const string CardHasBalance = "CARD_HAS_OUTSTANDING_BALANCE";
}
