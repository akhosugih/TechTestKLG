using System.ComponentModel.DataAnnotations;

namespace TechTestKLG.Models
{
    public class Activities
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        [MaxLength(5000)]
        public string Description { get; set; }
        [MaxLength(250)]
        public string Subject { get; set; }
        [MaxLength(1)]
        public int Status { get; set; }
    }
}
