using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Reopsitory.IRepository;

namespace BulkyBook.DataAccess.Reopsitory
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
        }


        public void Save()
        {
            _db.SaveChanges();
        }

        public string GenerateRandomISBN()
        {
            string prefix = "SOTJ";
            Random random = new Random();

            string randomNumbers = "";
            for (int i = 0; i < 10; i++)
            {
                randomNumbers += random.Next(0, 10);
            }

            string lastTwo = "";
            for (int i = 0; i < 2; i++)
            {
                lastTwo += random.Next(0, 10);
            }
            return $"{prefix}{randomNumbers}{lastTwo}";
        }


    }
}
