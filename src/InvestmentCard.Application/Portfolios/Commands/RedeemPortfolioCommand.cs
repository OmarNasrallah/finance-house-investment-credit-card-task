using InvestmentCard.Application.Common;
using InvestmentCard.Domain.Audit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentCard.Application.Portfolios.Commands;

public sealed record RedeemPortfolioCommand(Guid PortfolioId, decimal Amount) : IRequest;

public sealed class RedeemPortfolioCommandHandler(IAppDbContext db, TimeProvider clock)
    : IRequestHandler<RedeemPortfolioCommand>
{
    public async Task Handle(RedeemPortfolioCommand request, CancellationToken ct)
    {
        var portfolio = await db.Portfolios
            .Include(p => p.Liens)
            .SingleOrDefaultAsync(p => p.Id == request.PortfolioId, ct)
            ?? throw new NotFoundException("Portfolio", request.PortfolioId);

        portfolio.Redeem(request.Amount);

        db.AuditEntries.Add(new AuditEntry(portfolio.CustomerId, clock.GetUtcNow().UtcDateTime,
            "Portfolio", portfolio.Id, "Redeemed", request.Amount,
            $"New total value {portfolio.TotalValue:N2}"));

        await db.SaveChangesAsync(ct);
    }
}
