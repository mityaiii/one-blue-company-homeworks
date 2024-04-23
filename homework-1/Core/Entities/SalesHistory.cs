namespace Core.Entities;

public record SalesHistory
{
    public SalesHistory(long id, DateTime date, int sales, int stock)
    {
        if (id < 0)
        {
            throw new ArgumentException($"{id} should be positive");
        }

        if (sales is < 0 or > 12)
        {
            throw new ArgumentException($"{sales} should be positive");
        }
        
        if (stock is < 0 or > 12)
        {
            throw new ArgumentException($"{stock} should be positive");
        }
        
        Id = id;
        Date = date;
        Sales = sales;
        Stock = stock;
    }
    
    public long Id { get; }
    public DateTime Date { get; }
    public int Sales { get; }
    public int Stock { get; }
}