using InvestmentCard.Application.Audit;
using InvestmentCard.Application.Cards.Queries;
using InvestmentCard.Application.Customers;
using InvestmentCard.Application.Portfolios.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentCard.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<CustomerDto>> GetAll(CancellationToken ct) =>
        await sender.Send(new GetCustomersQuery(), ct);

    [HttpGet("{customerId:guid}/portfolios")]
    public async Task<IReadOnlyList<PortfolioDto>> GetPortfolios(Guid customerId, CancellationToken ct) =>
        await sender.Send(new GetPortfoliosQuery(customerId), ct);

    [HttpGet("{customerId:guid}/cards")]
    public async Task<IReadOnlyList<CardDto>> GetCards(Guid customerId, CancellationToken ct) =>
        await sender.Send(new GetCardsQuery(customerId), ct);

    [HttpGet("{customerId:guid}/audit")]
    public async Task<IReadOnlyList<AuditEntryDto>> GetAudit(Guid customerId, [FromQuery] int take = 50,
        CancellationToken ct = default) =>
        await sender.Send(new GetAuditTrailQuery(customerId, take), ct);
}
