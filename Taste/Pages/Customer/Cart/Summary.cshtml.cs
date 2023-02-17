using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Pages.Customer.Cart
{
    [BindProperties]
    public class SummaryModel : PageModel
    {

        private readonly IUnitOfWork _UnitOfWork;
        public SummaryModel(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public OrderDetailsCart DetailsCart { get; set; }
        public IActionResult OnGet()
        {
            DetailsCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };
            DetailsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<ShoppingCart> Cart = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value);
            if (Cart != null)
            {
                DetailsCart.listCart = Cart.ToList();
            }
            foreach (var Cartlist in DetailsCart.listCart)
            {
                Cartlist.MenuItem = _UnitOfWork.MenuItem.GetFirstOrDefault(m => m.id == Cartlist.MenuItemId);
                DetailsCart.OrderHeader.OrderTotal += (Cartlist.MenuItem.Price * Cartlist.Count);
            }
            ApplicationUser applicationUser = _UnitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);
            DetailsCart.OrderHeader.PickupName = applicationUser.FullName;
            DetailsCart.OrderHeader.PickupTime = DateTime.Now;
            DetailsCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            return Page();
        }
        public IActionResult OnPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            DetailsCart.listCart = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList();

            DetailsCart.OrderHeader.Paymentstatus = SD.PaymentStatusPending;
            DetailsCart.OrderHeader.OrderDate = DateTime.Now;
            DetailsCart.OrderHeader.UserId = claim.Value;
            DetailsCart.OrderHeader.Status = SD.PaymentStatusPending;
            DetailsCart.OrderHeader.PickupTime = Convert.ToDateTime(DetailsCart.OrderHeader.PickupDate.ToShortDateString() + " " +
                DetailsCart.OrderHeader.PickupTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _UnitOfWork.OrderHeader.Add(DetailsCart.OrderHeader);
            _UnitOfWork.Save();

            foreach (var item in DetailsCart.listCart)
            {
                item.MenuItem = _UnitOfWork.MenuItem.GetFirstOrDefault(m => m.id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = DetailsCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count,
                };

                DetailsCart.OrderHeader.OrderTotal += (item.MenuItem.Price * item.Count);
                _UnitOfWork.OrderDetails.Add(orderDetails);
            }
            DetailsCart.OrderHeader.OrderTotal = Convert.ToDouble(string.Format("{0:.##}", DetailsCart.OrderHeader.OrderTotal));
            _UnitOfWork.ShoppingCart.RemoveRange(DetailsCart.listCart);
            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
            _UnitOfWork.Save();
            if (stripeToken != null)
            {
                //StripeConfiguration.ApiKey = "sk_test_51KefkIAjTdbp5YHQdB2MbvinSHQpntnJIhclNnkxyYOruF7hbLPbRvI5bR7b5HMzZNvIaPAJTBT8U4t1wvbn3K4500VKhW5iZP";
                // `source` is obtained with Stripe.js; see https://stripe.com/docs/payments/accept-a-payment-charges#web-create-token
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(DetailsCart.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order ID:" + DetailsCart.OrderHeader.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);

                DetailsCart.OrderHeader.TransactionId = charge.Id;
                if (charge.Status.ToLower() == "succeeded")
                {
                    DetailsCart.OrderHeader.Paymentstatus = SD.PaymentStatusApproved;
                    DetailsCart.OrderHeader.Status = SD.StatusSubmitted;
                }
                else
                {
                    DetailsCart.OrderHeader.Paymentstatus = SD.PaymentStatusRejected;
                }
            }
            else
            {
                DetailsCart.OrderHeader.Paymentstatus = SD.PaymentStatusRejected;
            }
            _UnitOfWork.Save();
            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id=DetailsCart.OrderHeader.Id});
        }
    }
}
