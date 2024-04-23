using Core.Services;

namespace Core.Entities.Calculators;

public class PredictionCalculator : IPredictionCalculator
{
    private readonly IAdsService _adsService;
    public PredictionCalculator(IAdsService adsCalculator) 
    {
        _adsService = adsCalculator ?? throw new ArgumentNullException(nameof(adsCalculator));
    }

    public int Calculate(IReadOnlyCollection<SalesSeasonality> salesSeasonalities, long productId, int daysAmount)
    {
        decimal ads = _adsService.CalculateAds(productId);

        DateTime tmpDate = DateTime.Today;
        DateTime today = DateTime.Today;
        int predictProductsAmount = 0;
        while (daysAmount > 0)
        {
            int daysInMonth = DateTime.DaysInMonth(tmpDate.Year, tmpDate.Month);
            if (today.Equals(tmpDate))
            {
                daysInMonth -= today.Day;
            }

            var filteredSalesSeasonality = salesSeasonalities
                .First(salesSeasonality => salesSeasonality.Month == tmpDate.Month);

            predictProductsAmount += (int)Math.Ceiling(ads * daysAmount * (decimal)filteredSalesSeasonality.Coef);
            
            daysAmount -= daysInMonth;
            tmpDate = tmpDate.AddMonths(1);
        }
        
        return predictProductsAmount;
    }
}