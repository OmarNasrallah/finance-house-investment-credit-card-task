using Hangfire;
using InvestmentCard.Infrastructure.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentCard.Api.Controllers;

[ApiController]
[Route("api/jobs")]
public sealed class JobsController(IBackgroundJobClient jobs) : ControllerBase
{
    [HttpPost("delinquency-check")]
    public IActionResult TriggerDelinquencyCheck()
    {
        var jobId = jobs.Enqueue<DelinquencyJob>(j => j.RunAsync(CancellationToken.None));
        return Accepted(new { jobId });
    }
}
