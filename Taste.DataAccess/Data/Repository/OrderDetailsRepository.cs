using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;

namespace Taste.DataAccess.Data.Repository
{
    class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetails OrderDetails)
        {
            var OrderDetailsFromDb = _db.OrderDetails.FirstOrDefault(s => s.Id == OrderDetails.Id);
            _db.OrderDetails.Update(OrderDetailsFromDb);
            _db.SaveChanges();
        }
    }
}
