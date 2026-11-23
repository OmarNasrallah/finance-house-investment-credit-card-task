using InvestmentCard.Domain.Customers;
using InvestmentCard.Domain.Portfolios;

namespace InvestmentCard.Infrastructure.Persistence;

public static class SeedData
{
    public static void Seed(AppDbContext db)
    {
        if (db.Customers.Any()) return;

        var anas = new Customer(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Anas Bourini");
        var rami = new Customer(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Rami Abusweilim");
        var omar = new Customer(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Omar Ismail");

        db.Customers.AddRange(anas, rami, omar);
        db.Portfolios.AddRange(
            new Portfolio(Guid.NewGuid(), anas.Id, PortfolioType.Basic, 10_000m),
            new Portfolio(Guid.NewGuid(), anas.Id, PortfolioType.Premium, 250_000m),
            new Portfolio(Guid.NewGuid(), rami.Id, PortfolioType.Plus, 50_000m),
            new Portfolio(Guid.NewGuid(), omar.Id, PortfolioType.Basic, 5_000m),
            new Portfolio(Guid.NewGuid(), omar.Id, PortfolioType.Plus, 30_000m),
            new Portfolio(Guid.NewGuid(), omar.Id, PortfolioType.Premium, 120_000m));

        db.SaveChanges();
    }
}
