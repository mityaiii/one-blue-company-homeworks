using Core.Entities;
using Core.Repositories;

namespace DataAccess.Parsers;

public class SalesSeasonalityParser : ISalesSeasonalityParser
{
    public IReadOnlyCollection<SalesSeasonality> Parse(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        var salesSeasonalities = new List<SalesSeasonality>(lines.Length);
        
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            
            long id = long.Parse(parts[0].Trim());
            int month = int.Parse(parts[1].Trim());
            double coef = double.Parse(parts[2].Trim());
            
            salesSeasonalities.Add(new SalesSeasonality(id, month, coef));
        }

        return salesSeasonalities.AsReadOnly();
    }
}