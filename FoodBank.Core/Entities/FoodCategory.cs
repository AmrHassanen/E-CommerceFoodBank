using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class FoodCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    [StringLength(200)]
    public string ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<FoodItem> FoodItems { get; set; }
}
