
using Microsoft.AspNetCore.Identity;

namespace FoodBank.CORE.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string ImageUrl { get; set; }
    }
}
