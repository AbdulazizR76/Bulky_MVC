using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Reopsitory.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Reopsitory
{
    internal class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Company Obj)
        {
            _db.Companies.Update(Obj);
        }
    }
}
