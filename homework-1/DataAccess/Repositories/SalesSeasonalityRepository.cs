using Core.Entities;
using Core.Repositories;
using DataAccess.Parsers;

namespace DataAccess.Repositories;

public class SalesSeasonalityRepository : ISalesSeasonalityRepository
{
    private readonly IParser<SalesSeasonality> _parser;
    private readonly string _filePath;

    public SalesSeasonalityRepository(IParser<SalesSeasonality> parser, string filePath)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        ArgumentException.ThrowIfNullOrEmpty(filePath);
        _filePath = filePath;
    }
    public IReadOnlyCollection<SalesSeasonality> Get(long id)
    {
        IReadOnlyCollection<SalesSeasonality> salesSeasonalities = _parser.Parse(_filePath);
        
        return salesSeasonalities
            .Where(salesSeasonality => salesSeasonality.Id == id)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyCollection<SalesSeasonality> GetAll()
    {
        return _parser.Parse(_filePath);
    }
}