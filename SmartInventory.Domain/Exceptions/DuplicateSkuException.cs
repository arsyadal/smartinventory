namespace SmartInventory.Domain.Exceptions;

public class DuplicateSkuException : DomainException
{
    public DuplicateSkuException(string sku)
        : base($"Product with SKU '{sku}' already exists.", "DUPLICATE_SKU")
    {
    }
}
