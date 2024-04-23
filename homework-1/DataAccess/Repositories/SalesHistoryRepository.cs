using Core.Entities;
using Core.Repositories;
using DataAccess.Parsers;

namespace DataAccess.Repositories;

public class SalesHistoryRepository : ISalesHistoryRepository
{
    private readonly IParser<SalesHistory> _parser;
    private readonly string _filePath;

    public SalesHistoryRepository(IParser<SalesHistory> parser, string filePath)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        ArgumentException.ThrowIfNullOrEmpty(filePath);
        _filePath = filePath;
    }
    
    public IReadOnlyCollection<SalesHistory> Get(long id)
    {
        IReadOnlyCollection<SalesHistory> salesHistories = _parser.Parse(_filePath);
        
        return salesHistories
            .Where(salesHistory => salesHistory.Id == id)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyCollection<SalesHistory> GetAll()
    {
        return _parser.Parse(_filePath);
    }
}