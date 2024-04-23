using Core.Entities;

namespace DataAccess.Parsers;

public interface ISalesSeasonalityParser : IParser<SalesSeasonality>
{
    IReadOnlyCollection<SalesSeasonality> Parse(string filePath);
}