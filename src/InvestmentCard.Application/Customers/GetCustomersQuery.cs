using InvestmentCard.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InvestmentCard.Application.Customers;

public sealed record CustomerDto(Guid Id, string FullName);

public sealed record GetCustomersQuery : IRequest<IReadOnlyList<CustomerDto>>;

public sealed class GetCustomersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetCustomersQuery, IReadOnlyList<CustomerDto>>
{
    public async Task<IReadOnlyList<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken ct) =>
        await db.Customers.AsNoTracking()
            .OrderBy(c => c.FullName)
            .Select(c => new CustomerDto(c.Id, c.FullName))
            .ToListAsync(ct);
}
