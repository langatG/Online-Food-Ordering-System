using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taste.Models.ViewModels
{
    public class OrderDetailsCart
    {
        public OrderHeader OrderHeader { get; set; }
        public List<ShoppingCart> listCart { get; set; }
    }
}
