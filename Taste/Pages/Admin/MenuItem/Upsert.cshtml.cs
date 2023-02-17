using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Pages.Admin.MenuItem
{
    [BindProperties]
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public UpsertModel(IUnitOfWork UnitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = UnitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public MenuItemVM MenuItemobj { get; set; }
        public IActionResult OnGet(int? id)
        {
            MenuItemobj = new MenuItemVM 
            {
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                FoodTypeList = _unitOfWork.FoodType.GetCategoryListForDropDown(),
                MenuItem=new Models.MenuItem()
            };
            if (id != null)
            {
                MenuItemobj.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(u => u.id == id);
                if (MenuItemobj.MenuItem == null)
                {
                    return NotFound();
                }
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(MenuItemobj.MenuItem.id == 0)
            {
                //create
                string filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webRootPath, @"images\menuItems");
                var extension = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(uploads, filename+ extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                MenuItemobj.MenuItem.Image = @"\images\menuItems\" + filename + extension;
                _unitOfWork.MenuItem.Add(MenuItemobj.MenuItem);
            }
            else
            {
                //edit
                var objFromdb = _unitOfWork.MenuItem.Get(MenuItemobj.MenuItem.id);
                if (files.Count > 0)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\menuItems");
                    var extension = Path.GetExtension(files[0].FileName);
                    //delete img
                    var imgpath = Path.Combine(webRootPath, objFromdb.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imgpath))
                    {
                        System.IO.File.Delete(imgpath);
                    }
                    //new upload
                    using (var fileStream = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    MenuItemobj.MenuItem.Image = @"\images\menuItems\" + filename + extension;
                }
                else
                {
                    MenuItemobj.MenuItem.Image = objFromdb.Image;
                }
                _unitOfWork.MenuItem.Update(MenuItemobj.MenuItem);
                
            }
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}
