using ProductService.Core.Entities;
using ProductService.Core.Models;

namespace ProductService.DataAccess.Entities;

public class CsvProductParser : ICsvParser
{
    private readonly StreamReader _reader;
    
    public CsvProductParser(string filePath)
    {
        _reader = new StreamReader(filePath);
    }

    public ProductSaleInfo? GetNext()
    {
        var line = _reader.ReadLine();
        return line is null 
            ? null 
            : ParseProductInfo(line);
    }

    public void Close()
    {
        _reader.Close();
    }

    private ProductSaleInfo ParseProductInfo(string data)
    {
        string[] inputValues = data.Split(',');
        if (inputValues.Length != 3)
        {
            return new ProductSaleInfo();
        }

        int productId = int.Parse(inputValues[0].Trim());
        int prediction = int.Parse(inputValues[1].Trim());
        int stock = int.Parse(inputValues[2].Trim());

        return new ProductSaleInfo(productId, prediction, stock);
    }
}