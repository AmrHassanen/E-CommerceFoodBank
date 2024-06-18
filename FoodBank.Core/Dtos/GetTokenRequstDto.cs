using System.ComponentModel.DataAnnotations;

namespace FoodBank.CORE.Dtos
{
    public class GetTokenRequstDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Passward { get; set; }
    }
}