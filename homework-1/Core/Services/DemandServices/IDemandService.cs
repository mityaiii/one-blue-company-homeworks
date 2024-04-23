namespace Core.Services.DemandServices;

public interface IDemandService
{
    int Demand(long productId, int daysAmount);
}