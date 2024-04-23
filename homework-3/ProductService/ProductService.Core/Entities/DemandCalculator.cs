using ProductService.Core.Models;

namespace ProductService.Core.Entities;

public class DemandCalculator : IDemandCalculator
{
    public async Task<int> Calculate(ProductSaleInfo productSaleInfo)
    {
        // Imitation of hard task
        await Task.Delay(2000);
        
        return await Task.FromResult(Math.Max(productSaleInfo.Prediction - productSaleInfo.Stock, 0));
    }
}