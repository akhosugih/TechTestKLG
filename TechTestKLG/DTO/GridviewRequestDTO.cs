using Newtonsoft.Json;

namespace TechTestKLG.DTO
{
    public class GridviewRequestDTO
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string Search { get; set; }
    }
}
