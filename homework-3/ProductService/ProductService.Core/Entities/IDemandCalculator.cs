using ProductService.Core.Models;

namespace ProductService.Core.Entities;

public interface IDemandCalculator
{ 
    Task<int> Calculate(ProductSaleInfo productSaleInfo);
}