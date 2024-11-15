using Newtonsoft.Json;

namespace TechTestKLG.DTO
{
    public class GridviewResponseDTO<T>
    {
        public List<T> Data { get; set; }

        public int RecordsTotal { get; set; }

        public int RecordsFiltered { get; set; }

        public int Draw { get; set; }
    }
}