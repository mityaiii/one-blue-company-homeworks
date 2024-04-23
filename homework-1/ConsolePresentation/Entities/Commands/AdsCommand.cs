using Core.Exceptions;
using Core.Services;

namespace ConsolePresentation.Entities.Commands;

public class AdsCommand : ICommand
{
    private readonly IAdsService _adsService;
    private readonly long _productId;
    
    public AdsCommand(IAdsService adsService, long productId)
    {
        _adsService = adsService ?? throw new NullReferenceException($"{nameof(adsService)}");
        _productId = productId;
    }

    public void Execute()
    {
        try
        {
            decimal ads = _adsService.CalculateAds(_productId);
            Console.WriteLine(ads);
        }
        catch (ValueNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
