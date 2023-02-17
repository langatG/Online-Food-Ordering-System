using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Utility;

namespace Taste.Pages.Admin.Category
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
        public Models.Category Categoryobj { get; set; }
        public IActionResult OnGet(int? id)
        {
            Categoryobj = new Models.Category();
            if (id != null)
            {
                Categoryobj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
                if (Categoryobj == null)
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
            if(Categoryobj.Id == 0)
            {
                _unitOfWork.Category.Add(Categoryobj);
            }
            else
            {
                _unitOfWork.Category.Update(Categoryobj);
            }
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}
