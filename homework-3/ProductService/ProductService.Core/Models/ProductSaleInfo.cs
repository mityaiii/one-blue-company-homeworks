namespace ProductService.Core.Models;

public record ProductSaleInfo
{
    public int Id { get; }
    public int Prediction { get; }
    public int Stock { get; }

    public ProductSaleInfo(int id, int prediction, int stock)
    {
        Id = id;
        Prediction = prediction;
        Stock = stock;
    }

    public ProductSaleInfo()
    { }
}