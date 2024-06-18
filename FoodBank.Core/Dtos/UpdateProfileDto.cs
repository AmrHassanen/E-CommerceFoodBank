using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace FoodBank.CORE.Dtos
{
    public class UpdateProfileDto
    {
        public IFormFile ImageUrl { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
