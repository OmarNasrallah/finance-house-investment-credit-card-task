using InvestmentCard.Application.Portfolios.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentCard.Api.Controllers;

public sealed record AmountRequest(decimal Amount);

[ApiController]
[Route("api/portfolios")]
public sealed class PortfoliosController(ISender sender) : ControllerBase
{
    [HttpPost("{portfolioId:guid}/redeem")]
    public async Task<IActionResult> Redeem(Guid portfolioId, AmountRequest body, CancellationToken ct)
    {
        await sender.Send(new RedeemPortfolioCommand(portfolioId, body.Amount), ct);
        return NoContent();
    }
}
