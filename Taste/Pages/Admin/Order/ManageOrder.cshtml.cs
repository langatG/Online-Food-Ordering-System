using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Pages.Admin.Order
{
    [BindProperties]
    [Authorize]
    public class ManageOrderModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public ManageOrderModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<OrderDetailsVM> orderDetailsVM { get; set; }
        
        public void OnGet()
        {
            orderDetailsVM = new List<OrderDetailsVM>();
            List<OrderHeader> orderHeaderlist = _unitOfWork.OrderHeader
                .GetAll(o=>o.Status == SD.StatusSubmitted || o.Status==SD.StatusInProcess)
                .OrderByDescending(u=>u.PickupTime).ToList();

            foreach (OrderHeader item in orderHeaderlist)
            {
                OrderDetailsVM individual = new OrderDetailsVM
                {
                    OrderHeader = item,
                    OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == item.Id).ToList()
                };
                orderDetailsVM.Add(individual);
            }
        }
        public IActionResult OnPostOrderPrepare(int OrderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o=>o.Id == OrderId);
            orderHeader.Status = SD.StatusInProcess;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder");
        }
        public IActionResult OnPostOrderReady(int OrderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o => o.Id == OrderId);
            orderHeader.Status = SD.StatusReady;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder");
        }
        public IActionResult OnPostOrderCancel(int OrderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o => o.Id == OrderId);
            orderHeader.Status = SD.StatusCancelled;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder");
        }
        public IActionResult OnPostOrderRefund(int OrderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o => o.Id == OrderId);
            //AMOUNT TO REFUND
            //StripeConfiguration.ApiKey = "sk_test_your_key";

            var options = new RefundCreateOptions
            {
                Amount = Convert.ToInt32(orderHeader.OrderTotal*100),
                Reason = RefundReasons.RequestedByCustomer,
                Charge = orderHeader.TransactionId
            };
            var service = new RefundService();
            Refund refund = service.Create(options);


            orderHeader.Status = SD.StatusRefunded;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder");
        }
    }
}