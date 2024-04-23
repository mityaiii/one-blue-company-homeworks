using ProductService.Core.Models;

namespace ProductService.Core.Entities;

public interface ICsvFileWriter
{
    public void Write(ProductDemand productDemand);
    public void Close();
}