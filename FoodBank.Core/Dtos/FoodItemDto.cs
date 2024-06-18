using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodBank.CORE.Dtos
{
    public class FoodItemDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [StringLength(200)]
        public string ImageUrl { get; set; }

        public bool IsAvailable { get; set; }

        public int Calories { get; set; }

        public double Fat { get; set; } // in grams

        public double Carbohydrates { get; set; } // in grams

        public double Protein { get; set; } // in grams

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
