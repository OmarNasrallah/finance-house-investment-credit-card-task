using InvestmentCard.Application.Common;
using InvestmentCard.Domain.Audit;
using MediatR;

namespace InvestmentCard.Application.Cards.Commands;

public sealed record RepayCardCommand(Guid CardId, decimal Amount) : IRequest;

public sealed class RepayCardCommandHandler(IAppDbContext db, TimeProvider clock)
    : IRequestHandler<RepayCardCommand>
{
    public async Task Handle(RepayCardCommand request, CancellationToken ct)
    {
        var card = await db.CreditCards.FindAsync([request.CardId], ct)
            ?? throw new NotFoundException("Card", request.CardId);

        var before = card.UtilizedAmount;
        card.Repay(request.Amount);

        db.AuditEntries.Add(new AuditEntry(card.CustomerId, clock.GetUtcNow().UtcDateTime, "Card", card.Id,
            "Repaid", request.Amount, $"Utilized {before:N2} → {card.UtilizedAmount:N2} of {card.CreditLimit:N2}"));
        await db.SaveChangesAsync(ct);
    }
}
