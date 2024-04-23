using Core.Entities;

namespace Core.Repositories;

public interface IRepository<out TValue, in TId>
{
    public IReadOnlyCollection<TValue> Get(TId id);
    public IReadOnlyCollection<TValue> GetAll();
}