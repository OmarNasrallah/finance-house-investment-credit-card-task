namespace InvestmentCard.Application.Common;

public sealed class NotFoundException(string entity, Guid id)
    : Exception($"{entity} '{id}' was not found.");
