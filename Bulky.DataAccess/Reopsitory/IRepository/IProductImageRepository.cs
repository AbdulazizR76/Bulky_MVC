using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        void Update(ProductImage obj);

    }
}
