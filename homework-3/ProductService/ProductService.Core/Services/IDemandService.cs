using ProductService.Core.Entities;

namespace ProductService.Core.Services;

public interface IDemandService
{ 
    Task StartCalculateAsync(CancellationToken cancellationToken);
    Task StopCalculate();
}