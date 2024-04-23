namespace ProductService.Domain.Entities.Products;

public record Product
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }

    public double Weight { get; set; }

    public ProductType ProductType { get; set; } 
    public DateTime DateOfCreation { get; set; }
    public long WarehouseId { get; set; }
}