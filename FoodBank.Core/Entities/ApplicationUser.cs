
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;

namespace FoodBank.CORE.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string ImageUrl { get; set; }
        public ICollection<Order> Orders { get; set; }

    }
}
