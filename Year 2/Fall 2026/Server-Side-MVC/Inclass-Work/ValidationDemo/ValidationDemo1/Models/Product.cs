using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel; // Add This 
using System.ComponentModel.DataAnnotations; // and Add this to use the annotations

namespace ModelExample.Models
{
    public class Product
    {
        [Required(ErrorMessage = "No ProductId was given.")] // Right above the property, put whatever annotation you want
        public int ProductId { get; set; }

        [Required]      // Now the user needs to fill out both ProductId and ProductName
        [MinLength(5)]  // You can also stack annotations
        public string ProductName { get; set; }

        [Range(0, 9999)][Required] // Sets the Range
        public double Rate { get; set; }
    }
}
