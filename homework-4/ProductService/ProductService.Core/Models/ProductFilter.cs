using ProductService.Domain.Entities.Products;

namespace ProductService.Domain.Models;

public record ProductFilter
{
    public string? Name { set; get; }
    public long? WarehouseId { set; get; }
    public DateTime? DateOfCreation { set; get; }
    public ProductType? ProductType { set; get; }
    public int? PageNumber { set; get; }

    public int? PageSize { set; get; }
}