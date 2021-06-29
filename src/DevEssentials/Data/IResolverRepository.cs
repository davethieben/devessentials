using System.Threading.Tasks;

namespace Essentials.Data
{
    public interface IResolverRepository<T>
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
    }
}
