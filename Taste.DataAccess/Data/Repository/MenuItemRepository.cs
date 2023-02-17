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
    class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _db;
        public MenuItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(MenuItem menuItem)
        {
            var objFromDb = _db.menuItem.FirstOrDefault(s => s.id == menuItem.id);
            objFromDb.Name = menuItem.Name;
            objFromDb.CategoryId = menuItem.CategoryId;
            objFromDb.Description = menuItem.Description;
            objFromDb.FoodTypeId = menuItem.FoodTypeId;
            objFromDb.Price = menuItem.Price;
            if (menuItem.Image!=null)
            {
                objFromDb.Image = menuItem.Image;
            }
            _db.SaveChanges();
        }
    }
}
