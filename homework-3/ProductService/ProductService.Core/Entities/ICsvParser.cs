using ProductService.Core.Models;

namespace ProductService.Core.Entities;

public interface ICsvParser
{
    public ProductSaleInfo? GetNext();
    public void Close();
}