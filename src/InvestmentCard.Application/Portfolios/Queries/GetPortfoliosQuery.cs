using InvestmentCard.Application.Common;
using InvestmentCard.Domain.Portfolios;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentCard.Application.Portfolios.Queries;

public sealed record PortfolioDto(
    Guid Id,
    string Type,
    decimal TotalValue,
    decimal LienAmount,
    decimal AvailableValue,
    bool IsFrozen,
    decimal AdvanceRate);

public sealed record GetPortfoliosQuery(Guid CustomerId) : IRequest<IReadOnlyList<PortfolioDto>>;

public sealed class GetPortfoliosQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPortfoliosQuery, IReadOnlyList<PortfolioDto>>
{
    public async Task<IReadOnlyList<PortfolioDto>> Handle(GetPortfoliosQuery request, CancellationToken ct)
    {
        var portfolios = await db.Portfolios.AsNoTracking()
            .Include(p => p.Liens)
            .Where(p => p.CustomerId == request.CustomerId)
            .OrderBy(p => p.Type)
            .ToListAsync(ct);

        return portfolios
            .Select(p => new PortfolioDto(
                p.Id, p.Type.ToString(), p.TotalValue, p.LienAmount, p.AvailableValue,
                p.IsFrozen, p.Type.AdvanceRate()))
            .ToList();
    }
}
