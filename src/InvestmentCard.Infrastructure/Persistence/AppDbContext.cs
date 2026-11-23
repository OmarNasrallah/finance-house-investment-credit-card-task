using InvestmentCard.Application.Common;
using InvestmentCard.Domain.Audit;
using InvestmentCard.Domain.Cards;
using InvestmentCard.Domain.Customers;
using InvestmentCard.Domain.Portfolios;
using Microsoft.EntityFrameworkCore;

namespace InvestmentCard.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Customer>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
        });

        b.Entity<Portfolio>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Ignore(x => x.LienAmount);
            e.Ignore(x => x.AvailableValue);
            e.HasMany(x => x.Liens).WithOne().HasForeignKey(l => l.PortfolioId);
            e.Navigation(x => x.Liens).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        b.Entity<Lien>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Ignore(x => x.IsActive);
        });

        b.Entity<CreditCard>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Ignore(x => x.AvailableCredit);
            e.Ignore(x => x.IsAtRisk);
        });

        b.Entity<AuditEntry>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
        });
    }
}
