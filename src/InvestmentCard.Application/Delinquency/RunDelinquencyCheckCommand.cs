using InvestmentCard.Application.Common;
using InvestmentCard.Domain.Audit;
using InvestmentCard.Domain.Cards;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvestmentCard.Application.Delinquency;

public sealed record RunDelinquencyCheckCommand : IRequest;

public sealed class RunDelinquencyCheckCommandHandler(
    IAppDbContext db,
    TimeProvider clock,
    IOptions<DelinquencyOptions> options,
    ILogger<RunDelinquencyCheckCommandHandler> logger)
    : IRequestHandler<RunDelinquencyCheckCommand>
{
    public async Task Handle(RunDelinquencyCheckCommand request, CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var threshold = TimeSpan.FromMinutes(options.Value.ThresholdMinutes);
        var cutoff = now - threshold;

        var candidateIds = await db.CreditCards.AsNoTracking()
            .Where(c => c.Status == CardStatus.Active
                        && c.FullyUtilizedSinceUtc != null
                        && c.FullyUtilizedSinceUtc <= cutoff)
            .Select(c => c.Id)
            .ToListAsync(ct);

        int marked = 0, failed = 0;

        foreach (var cardId in candidateIds)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var card = await db.CreditCards.SingleAsync(c => c.Id == cardId, ct);

                // might have been repaid between the query and now
                if (!card.IsOverdue(now, threshold)) continue;

                var portfolio = await db.Portfolios.SingleAsync(p => p.Id == card.PortfolioId, ct);

                card.MarkDelinquent();
                var newlyFrozen = portfolio.Freeze();

                db.AuditEntries.Add(new AuditEntry(card.CustomerId, now, "Card", card.Id, "MarkedDelinquent",
                    card.UtilizedAmount, $"Fully utilized since {card.FullyUtilizedSinceUtc:u}. Transactions blocked."));
                if (newlyFrozen)
                    db.AuditEntries.Add(new AuditEntry(card.CustomerId, now, "Portfolio", portfolio.Id,
                        "PortfolioFrozen", null, $"Frozen due to delinquent card {card.Id}"));

                // save per card so one bad row doesn't kill the whole run
                await db.SaveChangesAsync(ct);
                marked++;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                failed++;
                db.ChangeTracker.Clear();
                logger.LogError(ex, "Delinquency check failed for card {CardId}", cardId);
            }
        }

        logger.LogInformation(
            "Delinquency check finished: {Candidates} candidate(s), {Marked} marked delinquent, {Failed} failed",
            candidateIds.Count, marked, failed);
    }
}
