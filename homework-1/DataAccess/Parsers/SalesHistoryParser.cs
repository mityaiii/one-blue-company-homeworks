using System.Globalization;
using Core.Entities;

namespace DataAccess.Parsers;

public class SalesHistoryParser : ISalesHistoryParser
{
    public IReadOnlyCollection<SalesHistory> Parse(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        var salesHistories = new List<SalesHistory>(lines.Length);
        
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            
            long id = long.Parse(parts[0].Trim());
            DateTime date = DateTime.ParseExact(parts[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var sales = int.Parse(parts[2].Trim());
            var stock = int.Parse(parts[3].Trim());
            
            salesHistories.Add(new SalesHistory(id, date, sales, stock));
        }

        return salesHistories.AsReadOnly();
    }
}