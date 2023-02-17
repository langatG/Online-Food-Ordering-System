using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taste.Models;

namespace Taste.DataAccess.Data.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        //IEnumerable<SelectListItem> GetCategoryListForDropDown();
        void Update(OrderHeader OrderHeader);
    }
}
