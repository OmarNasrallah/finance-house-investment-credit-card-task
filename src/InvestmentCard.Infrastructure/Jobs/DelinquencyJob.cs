using Hangfire;
using InvestmentCard.Application.Delinquency;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InvestmentCard.Infrastructure.Jobs;

public sealed class DelinquencyJob(ISender sender, ILogger<DelinquencyJob> logger)
{
    public const string RecurringJobId = "delinquency-check";

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    [AutomaticRetry(Attempts = 3)]
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new RunDelinquencyCheckCommand(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delinquency job failed");
            throw;
        }
    }
}
