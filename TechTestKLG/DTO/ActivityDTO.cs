using System.ComponentModel.DataAnnotations;

namespace TechTestKLG.DTO
{
    public class ActivityDTO
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Description must not be empty")]
        public string Description { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }
    }
}
