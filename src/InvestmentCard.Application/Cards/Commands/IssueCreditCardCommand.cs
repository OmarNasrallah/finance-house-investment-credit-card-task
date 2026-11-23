using InvestmentCard.Application.Common;
using InvestmentCard.Domain.Audit;
using InvestmentCard.Domain.Cards;
using InvestmentCard.Domain.Portfolios;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentCard.Application.Cards.Commands;

public sealed record IssueCreditCardCommand(Guid CustomerId, Guid PortfolioId, decimal LienAmount) : IRequest<Guid>;

public sealed class IssueCreditCardCommandHandler(IAppDbContext db, TimeProvider clock)
    : IRequestHandler<IssueCreditCardCommand, Guid>
{
    public async Task<Guid> Handle(IssueCreditCardCommand request, CancellationToken ct)
    {
        var portfolio = await db.Portfolios
            .Include(p => p.Liens)
            .SingleOrDefaultAsync(p => p.Id == request.PortfolioId && p.CustomerId == request.CustomerId, ct)
            ?? throw new NotFoundException("Portfolio", request.PortfolioId);

        var now = clock.GetUtcNow().UtcDateTime;

        // lien first — if this throws we never create the card
        var lien = portfolio.PlaceLien(request.LienAmount, now);

        var creditLimit = Math.Round(lien.Amount * portfolio.Type.AdvanceRate(), 2, MidpointRounding.ToZero);
        var card = CreditCard.Issue(request.CustomerId, portfolio, lien, creditLimit, GenerateCardNumber(), now);

        db.CreditCards.Add(card);
        db.AuditEntries.Add(new AuditEntry(request.CustomerId, now, "Lien", lien.Id, "LienPlaced", lien.Amount,
            $"{portfolio.Type} portfolio. Available value now {portfolio.AvailableValue:N2}"));
        db.AuditEntries.Add(new AuditEntry(request.CustomerId, now, "Card", card.Id, "CardIssued", card.CreditLimit,
            $"Credit limit {card.CreditLimit:N2} ({portfolio.Type.AdvanceRate():P0} of lien)"));

        await db.SaveChangesAsync(ct);
        return card.Id;
    }

    // fake visa-ish number, good enough for the demo
    private static string GenerateCardNumber() =>
        "4" + string.Concat(Enumerable.Range(0, 15).Select(_ => Random.Shared.Next(0, 10)));
}
