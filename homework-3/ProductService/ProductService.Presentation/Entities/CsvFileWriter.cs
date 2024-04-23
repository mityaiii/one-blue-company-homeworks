using ProductService.Core.Entities;
using ProductService.Core.Models;

namespace Presentation.Entities;

public class CsvFileWriter : ICsvFileWriter
{
    private readonly StreamWriter _streamWriter;
    
    public CsvFileWriter(string filePath)
    {
        _streamWriter = new StreamWriter(filePath);
    }
    
    public void Write(ProductDemand productSaleInfo)
    {
        _streamWriter.WriteLine($"{productSaleInfo.Id}, {productSaleInfo.Demand}");
    }

    public void Close()
    {
        _streamWriter.Close();
    }
}