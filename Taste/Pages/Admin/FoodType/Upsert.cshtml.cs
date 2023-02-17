using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Utility;

namespace Taste.Pages.Admin.FoodType
{
    [BindProperties]
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Models.FoodType FoodTypeobj { get; set; }
        public IActionResult OnGet(int? id)
        {
            FoodTypeobj = new Models.FoodType();
            if (id != null)
            {
                FoodTypeobj = _unitOfWork.FoodType.GetFirstOrDefault(u => u.Id == id);
                if (FoodTypeobj == null)
                {
                    return NotFound();
                }
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (FoodTypeobj.Id == 0)
            {
                _unitOfWork.FoodType.Add(FoodTypeobj);
            }
            else
            {
                _unitOfWork.FoodType.Update(FoodTypeobj);
            }
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}
