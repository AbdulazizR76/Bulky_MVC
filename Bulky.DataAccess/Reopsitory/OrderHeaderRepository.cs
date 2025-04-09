﻿using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Reopsitory.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Reopsitory
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }



        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }
    }
}
