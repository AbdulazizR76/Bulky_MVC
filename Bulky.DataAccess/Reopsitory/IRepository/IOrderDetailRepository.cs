using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);

    }
}
