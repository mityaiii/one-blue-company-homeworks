using ProductService.Domain.Entities.Products;

namespace ProductService.Domain.Models;

public record ProductFilter(string? Name, 
    long? WarehouseId,
    DateTime? DateOfCreation,
    ProductType? ProductType,
    int? PageNumber,
    int? PageSize);