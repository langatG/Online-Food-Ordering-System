using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Pages.Customer.Cart
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _UnitOfWork;
        public IndexModel(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public OrderDetailsCart OrderDetailsCartVM { get; set; }
        public void OnGet()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader(),
                listCart = new List<ShoppingCart>()
            };
            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                IEnumerable<ShoppingCart> Cart = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value);
                if (Cart != null)
                {
                    OrderDetailsCartVM.listCart = Cart.ToList();
                }
                foreach (var Cartlist in OrderDetailsCartVM.listCart)
                {
                    Cartlist.MenuItem = _UnitOfWork.MenuItem.GetFirstOrDefault(m => m.id == Cartlist.MenuItemId);
                    OrderDetailsCartVM.OrderHeader.OrderTotal += (Cartlist.MenuItem.Price * Cartlist.Count);
                }
            }
        }
        public IActionResult OnPostPlus(int cartId)
        {
            var cart = _UnitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _UnitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _UnitOfWork.Save();
            return RedirectToPage("/Customer/Cart/Index");
        }
        public IActionResult OnPostMinus(int cartId)
        {
            var cart = _UnitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count == 1)
            {
                var count = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                _UnitOfWork.ShoppingCart.Remove(cart);
                _UnitOfWork.Save();
                HttpContext.Session.SetInt32(SD.ShoppingCart, count);
            }
            else
            {
                _UnitOfWork.ShoppingCart.DecrementCount(cart, 1);
                _UnitOfWork.Save();
            }
            return RedirectToPage("/Customer/Cart/Index");
        }
        public IActionResult OnPostRemove(int cartId)
        {
            var cart = _UnitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            var count = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;

            _UnitOfWork.ShoppingCart.Remove(cart);
            _UnitOfWork.Save();
            HttpContext.Session.SetInt32(SD.ShoppingCart, count);
            return RedirectToPage("/Customer/Cart/Index");
        }
    }
}
