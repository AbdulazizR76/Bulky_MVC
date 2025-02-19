using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);

    }
}
