using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        public void Update(ApplicationUser applicationUser);

    }
}
