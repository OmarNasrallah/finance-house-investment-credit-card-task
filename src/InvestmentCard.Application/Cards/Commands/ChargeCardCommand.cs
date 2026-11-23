using InvestmentCard.Application.Common;
using InvestmentCard.Domain.Audit;
using MediatR;

namespace InvestmentCard.Application.Cards.Commands;

public sealed record ChargeCardCommand(Guid CardId, decimal Amount) : IRequest;

public sealed class ChargeCardCommandHandler(IAppDbContext db, TimeProvider clock)
    : IRequestHandler<ChargeCardCommand>
{
    public async Task Handle(ChargeCardCommand request, CancellationToken ct)
    {
        var card = await db.CreditCards.FindAsync([request.CardId], ct)
            ?? throw new NotFoundException("Card", request.CardId);

        var now = clock.GetUtcNow().UtcDateTime;
        var before = card.UtilizedAmount;

        card.Charge(request.Amount, now);

        var details = $"Utilized {before:N2} → {card.UtilizedAmount:N2} of {card.CreditLimit:N2}";
        if (card.IsAtRisk) details += ". Limit fully utilized: card at risk";

        db.AuditEntries.Add(new AuditEntry(card.CustomerId, now, "Card", card.Id, "Charged", request.Amount, details));
        await db.SaveChangesAsync(ct);
    }
}
