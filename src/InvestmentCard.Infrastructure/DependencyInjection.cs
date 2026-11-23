using InvestmentCard.Application.Common;
using InvestmentCard.Infrastructure.Jobs;
using InvestmentCard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentCard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("InvestmentCardDb"));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<DelinquencyJob>();
        return services;
    }
}
