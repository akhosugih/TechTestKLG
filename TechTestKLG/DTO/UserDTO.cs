using System.ComponentModel.DataAnnotations;

namespace TechTestKLG.DTO
{
    public class UserDTO
    {
        [MinLength(5)]
        [Required(ErrorMessage = "Username must not be empty")]
        public string Username { get; set; }

        [MinLength(8)]
        [Required(ErrorMessage = "Password must not be empty")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one number.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Name must not be empty")]
        public string Name { get; set; }
    }
}
