using TechTestKLG.DTO;
using TechTestKLG.Models;

namespace TechTestKLG.Services.Interfaces
{
    public interface IActivityService
    {
        public Task<GridviewResponseDTO<Activities>> GetActivity(GridviewRequestDTO request);
        public Task<Activities> GetActivity(string id);
        public Task<bool> Insert(Activities activity);
        public Task<bool> Update(Activities activity);
        public Task<bool> Delete(string id);
    }
}
