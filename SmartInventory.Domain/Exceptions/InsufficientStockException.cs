namespace SmartInventory.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int requested, int available)
        : base(
            $"Insufficient stock for '{productName}'. Requested: {requested}, Available: {available}.",
            "INSUFFICIENT_STOCK")
    {
    }
}
