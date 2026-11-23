using InvestmentCard.Domain.Audit;
using InvestmentCard.Domain.Cards;
using InvestmentCard.Domain.Customers;
using InvestmentCard.Domain.Portfolios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace InvestmentCard.Application.Common;

public interface IAppDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Portfolio> Portfolios { get; }
    DbSet<CreditCard> CreditCards { get; }
    DbSet<AuditEntry> AuditEntries { get; }

    ChangeTracker ChangeTracker { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
