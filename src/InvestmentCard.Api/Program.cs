using Hangfire;
using InvestmentCard.Api.Middleware;
using InvestmentCard.Application;
using InvestmentCard.Application.Delinquency;
using InvestmentCard.Infrastructure;
using InvestmentCard.Infrastructure.Jobs;
using InvestmentCard.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure();

builder.Services.Configure<DelinquencyOptions>(builder.Configuration.GetSection(DelinquencyOptions.SectionName));

builder.Services.AddHangfire(cfg => cfg
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage());
builder.Services.AddHangfireServer();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    SeedData.Seed(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHangfireDashboard("/hangfire");

var cron = builder.Configuration[$"{DelinquencyOptions.SectionName}:Cron"] ?? Cron.Hourly();
app.Services.GetRequiredService<IRecurringJobManager>()
    .AddOrUpdate<DelinquencyJob>(DelinquencyJob.RecurringJobId, j => j.RunAsync(CancellationToken.None), cron);

app.MapControllers();

app.Run();
