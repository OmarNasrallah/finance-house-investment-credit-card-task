using InvestmentCard.Application.Common;
using InvestmentCard.Domain.Audit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentCard.Application.Cards.Commands;

public sealed record CloseCardCommand(Guid CardId) : IRequest;

public sealed class CloseCardCommandHandler(IAppDbContext db, TimeProvider clock)
    : IRequestHandler<CloseCardCommand>
{
    public async Task Handle(CloseCardCommand request, CancellationToken ct)
    {
        var card = await db.CreditCards.FindAsync([request.CardId], ct)
            ?? throw new NotFoundException("Card", request.CardId);

        var portfolio = await db.Portfolios
            .Include(p => p.Liens)
            .SingleAsync(p => p.Id == card.PortfolioId, ct);

        var now = clock.GetUtcNow().UtcDateTime;

        card.Close();
        portfolio.ReleaseLien(card.LienId, now);

        db.AuditEntries.Add(new AuditEntry(card.CustomerId, now, "Card", card.Id, "CardClosed", null, "Card closed"));
        db.AuditEntries.Add(new AuditEntry(card.CustomerId, now, "Lien", card.LienId, "LienReleased", null,
            $"Available value now {portfolio.AvailableValue:N2}"));

        await db.SaveChangesAsync(ct);
    }
}
