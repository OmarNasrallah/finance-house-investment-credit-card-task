namespace InvestmentCard.Domain.Audit;

public class AuditEntry
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime TimestampUtc { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public decimal? Amount { get; private set; }
    public string Details { get; private set; } = string.Empty;

    private AuditEntry() { }

    public AuditEntry(Guid customerId, DateTime timestampUtc, string entityType, Guid entityId,
        string action, decimal? amount, string details)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        TimestampUtc = timestampUtc;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Amount = amount;
        Details = details;
    }
}
