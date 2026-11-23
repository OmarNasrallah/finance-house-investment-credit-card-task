using InvestmentCard.Application.Cards.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentCard.Api.Controllers;

public sealed record IssueCardRequest(Guid CustomerId, Guid PortfolioId, decimal LienAmount);

[ApiController]
[Route("api/cards")]
public sealed class CardsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Issue(IssueCardRequest body, CancellationToken ct)
    {
        var id = await sender.Send(new IssueCreditCardCommand(body.CustomerId, body.PortfolioId, body.LienAmount), ct);
        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    [HttpPost("{cardId:guid}/charge")]
    public async Task<IActionResult> Charge(Guid cardId, AmountRequest body, CancellationToken ct)
    {
        await sender.Send(new ChargeCardCommand(cardId, body.Amount), ct);
        return NoContent();
    }

    [HttpPost("{cardId:guid}/repay")]
    public async Task<IActionResult> Repay(Guid cardId, AmountRequest body, CancellationToken ct)
    {
        await sender.Send(new RepayCardCommand(cardId, body.Amount), ct);
        return NoContent();
    }

    [HttpPost("{cardId:guid}/close")]
    public async Task<IActionResult> Close(Guid cardId, CancellationToken ct)
    {
        await sender.Send(new CloseCardCommand(cardId), ct);
        return NoContent();
    }
}
