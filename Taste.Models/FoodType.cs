﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taste.Models
{
    public class FoodType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Food Type")]
        public string Name { get; set; }
    }
}
