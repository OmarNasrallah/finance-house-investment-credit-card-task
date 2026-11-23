using InvestmentCard.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentCard.Application.Audit;

public sealed record AuditEntryDto(
    DateTime TimestampUtc, string EntityType, Guid EntityId, string Action, decimal? Amount, string Details);

public sealed record GetAuditTrailQuery(Guid CustomerId, int Take = 50) : IRequest<IReadOnlyList<AuditEntryDto>>;

public sealed class GetAuditTrailQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAuditTrailQuery, IReadOnlyList<AuditEntryDto>>
{
    public async Task<IReadOnlyList<AuditEntryDto>> Handle(GetAuditTrailQuery request, CancellationToken ct) =>
        await db.AuditEntries.AsNoTracking()
            .Where(a => a.CustomerId == request.CustomerId)
            .OrderByDescending(a => a.TimestampUtc)
            .Take(Math.Clamp(request.Take, 1, 200))
            .Select(a => new AuditEntryDto(a.TimestampUtc, a.EntityType, a.EntityId, a.Action, a.Amount, a.Details))
            .ToListAsync(ct);
}
