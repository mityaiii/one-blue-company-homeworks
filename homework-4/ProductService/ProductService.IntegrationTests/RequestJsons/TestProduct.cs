using ProductService.Api;
using ProductService.Domain.Entities.Products;

namespace ProductService.IntegrationTests.RequestJsons;

public class TestProduct
{
    public long id { get; set; }
    public string name { get; set; }
    public double price { get; set; }
    public double weight { get; set; }
    public int productType { get; set; }
    public string dateOfCreation { get; set; }
    public long warehouseId { get; set; }
}