﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taste.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        [NotMapped]
        public virtual MenuItem MenuItem { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [NotMapped]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Range(1, 100, ErrorMessage = "Please select a count between 1 and 100")]
        public int Count { get; set; }
       
    }
}
