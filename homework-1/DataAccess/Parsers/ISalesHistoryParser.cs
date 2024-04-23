using Core.Entities;

namespace DataAccess.Parsers;

public interface ISalesHistoryParser : IParser<SalesHistory>
{
    public IReadOnlyCollection<SalesHistory> Parse(string filePath);
}