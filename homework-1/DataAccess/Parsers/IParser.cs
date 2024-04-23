namespace DataAccess.Parsers;

public interface IParser<out TValue>
{
    IReadOnlyCollection<TValue> Parse(string filePath);
}