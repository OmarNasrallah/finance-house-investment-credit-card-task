namespace InvestmentCard.Domain.Customers;

public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;

    private Customer() { }

    public Customer(Guid id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }
}
