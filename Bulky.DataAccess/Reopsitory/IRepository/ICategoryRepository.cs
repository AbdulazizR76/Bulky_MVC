using Bulky.Models;

namespace Bulky.DataAccess.Reopsitory.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
        void Save();
    }
}
