using InvestmentCard.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentCard.Application.Cards.Queries;

public sealed record CardDto(
    Guid Id,
    Guid PortfolioId,
    string PortfolioType,
    string MaskedCardNumber,
    decimal CreditLimit,
    decimal UtilizedAmount,
    decimal AvailableCredit,
    string Status,
    bool IsAtRisk,
    DateTime? FullyUtilizedSinceUtc,
    DateTime IssuedAtUtc);

public sealed record GetCardsQuery(Guid CustomerId) : IRequest<IReadOnlyList<CardDto>>;

public sealed class GetCardsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetCardsQuery, IReadOnlyList<CardDto>>
{
    public async Task<IReadOnlyList<CardDto>> Handle(GetCardsQuery request, CancellationToken ct)
    {
        var cards = await db.CreditCards.AsNoTracking()
            .Where(c => c.CustomerId == request.CustomerId)
            .OrderByDescending(c => c.IssuedAtUtc)
            .ToListAsync(ct);

        var portfolioTypes = await db.Portfolios.AsNoTracking()
            .Where(p => p.CustomerId == request.CustomerId)
            .ToDictionaryAsync(p => p.Id, p => p.Type.ToString(), ct);

        return cards.Select(c => new CardDto(
                c.Id,
                c.PortfolioId,
                portfolioTypes.GetValueOrDefault(c.PortfolioId, "?"),
                "**** **** **** " + c.CardNumber[^4..],
                c.CreditLimit,
                c.UtilizedAmount,
                c.AvailableCredit,
                c.Status.ToString(),
                c.IsAtRisk,
                c.FullyUtilizedSinceUtc,
                c.IssuedAtUtc))
            .ToList();
    }
}
