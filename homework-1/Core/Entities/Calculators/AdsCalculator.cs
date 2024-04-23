namespace Core.Entities.Calculators;

public class AdsCalculator : IAdsCalculator
{
    public decimal Calculate(IReadOnlyCollection<SalesHistory> salesHistories)
    {
        SalesHistory tmpSalesHistory = salesHistories.First();
        int totalAmountOfDays = 0;
        int productsInStockAmount = 0;
        foreach (var salesHistory in salesHistories)
        {
            if (tmpSalesHistory.Stock == 0)
            {
                continue;
            }
            
            productsInStockAmount += salesHistory.Sales;
            TimeSpan difference = salesHistory.Date.Subtract(tmpSalesHistory.Date);
            totalAmountOfDays += difference.Days + 1;
            
            tmpSalesHistory = salesHistory;
        }
        
        
        if (productsInStockAmount == 0)
        {
            return 0;
        }

        return (decimal)totalAmountOfDays / productsInStockAmount;
    }
}