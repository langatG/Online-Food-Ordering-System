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
    class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader OrderHeader)
        {
            var OrderHeaderFromDb = _db.OrderHeader.FirstOrDefault(s => s.Id == OrderHeader.Id);
            _db.OrderHeader.Update(OrderHeaderFromDb);
            _db.SaveChanges();
        }
    }
}
