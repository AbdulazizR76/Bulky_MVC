using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Reopsitory.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }



        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
