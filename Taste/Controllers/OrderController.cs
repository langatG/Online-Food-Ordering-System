using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Authorize]
        public IActionResult Get(string status=null)
        {
            List<OrderDetailsVM> orderDetailsVM = new List<OrderDetailsVM>();
            IEnumerable<OrderHeader> orderHeaderlist;
            if (User.IsInRole(SD.CustomerRole))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaderlist = _unitOfWork.OrderHeader.GetAll(u => u.UserId == claim.Value, null, "ApplicationUser");
                //return Json(new { data = _unitOfWork.Category.GetAll() });
            }
            else
            {
                orderHeaderlist = _unitOfWork.OrderHeader.GetAll(null, null, "ApplicationUser");
            }
            if (status=="cancelled")
            {
                orderHeaderlist = orderHeaderlist.Where(o=>o.Status == SD.StatusCancelled || o.Status==SD.StatusRefunded || o.Status == SD.PaymentStatusRejected);
            }
            else
            {
                if (status == "completed")
                {
                    orderHeaderlist = orderHeaderlist.Where(o => o.Status == SD.StatusCompleted);
                }
                else
                {
                    orderHeaderlist = orderHeaderlist.Where(o => o.Status == SD.StatusReady || o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess || o.Status == SD.PaymentStatusPending);
                }
            }
            foreach (OrderHeader item in orderHeaderlist)
            {
                OrderDetailsVM individual = new OrderDetailsVM
                {
                    OrderHeader = item,
                    OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == item.Id).ToList()
                };
                orderDetailsVM.Add(individual);
            }
            return Json(new { data = orderDetailsVM });
        }
    }
}
