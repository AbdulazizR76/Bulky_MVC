using BulkyBook.DataAccess.Data;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
