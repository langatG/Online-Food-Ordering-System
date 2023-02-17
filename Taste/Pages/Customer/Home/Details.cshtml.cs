using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Utility;

namespace Taste.Pages.Customer.Home
{
    [Authorize]
    [BindProperties]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ShoppingCart ShoppingCartobj { get; set; }
        public void OnGet(int id)
        {
            ShoppingCartobj = new ShoppingCart()
            {
                MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(includeProperties: "Category,FoodType", filter: u => u.id == id),
                MenuItemId = id,
            };
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCartobj.ApplicationUserId = claim.Value;

                ShoppingCart CartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == ShoppingCartobj.ApplicationUserId &&
                    u.MenuItemId == ShoppingCartobj.MenuItemId);
                if (CartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(ShoppingCartobj);
                   // _unitOfWork.Save();
                    
                }
                else
                {
                    _unitOfWork.ShoppingCart.IncrementCount(CartFromDb, ShoppingCartobj.Count);
                }
                _unitOfWork.Save();
                
                var count= _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == ShoppingCartobj.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ShoppingCart, count);
                return RedirectToPage("Index");
            }
            else
            {
                ShoppingCartobj.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(includeProperties: "Category,FoodType", filter: u => u.id == ShoppingCartobj.MenuItemId);
                return Page();
            }
        }
    }
}
