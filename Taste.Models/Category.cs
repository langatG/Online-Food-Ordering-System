using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taste.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="Category Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Display Orde")]
        public int DisplayOrder { get; set; }
    }
}
