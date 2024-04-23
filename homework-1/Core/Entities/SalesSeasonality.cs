namespace Core.Entities;

public record SalesSeasonality
{
    public SalesSeasonality(long id, int month, double coef)
    {
        if (coef is < 0 or > 3)
        {
            throw new ArgumentException($"{coef} should be between 0 and 3");
        }

        if (id < 0)
        {
            throw new ArgumentException($"{id} should be positive");
        }

        if (month is < 0 or > 12)
        {
            throw new ArgumentException($"{month} should be positive");
        }
        
        Id = id;
        Month = month;
        Coef = coef;
    }

    public long Id { get; }
    public int Month { get; }
    public double Coef { get; }
}