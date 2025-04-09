using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);

    }
}
