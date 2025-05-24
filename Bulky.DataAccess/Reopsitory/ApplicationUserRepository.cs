using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Reopsitory.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUser applicationUser)
        {
            _db.ApplicationUsers.Update(applicationUser);
        }

    }
}
