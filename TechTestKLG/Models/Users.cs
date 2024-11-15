using System.ComponentModel.DataAnnotations;

namespace TechTestKLG.Models
{
    public class Users
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
}
