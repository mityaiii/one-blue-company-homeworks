using Core.Exceptions;
using Core.Services;
using Core.Services.DemandServices;

namespace ConsolePresentation.Entities.Commands;

public class DemandCommand : ICommand
{
    private readonly IDemandService _demandService;
    private readonly long _productId;
    private readonly int _daysAmount;

    public DemandCommand(IDemandService demandService, long productId, int daysAmount)
    {
        _demandService = demandService ?? throw new NullReferenceException($"{nameof(demandService)}");
        _productId = productId;
        _daysAmount = daysAmount;
    }

    public void Execute()
    {
        try
        {
            int demand = _demandService.Demand(_productId, _daysAmount);
            Console.WriteLine(demand);
        }
        catch (ValueNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}