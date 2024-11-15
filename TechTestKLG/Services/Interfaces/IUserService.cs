using TechTestKLG.DTO;
using TechTestKLG.Models;

namespace TechTestKLG.Services.Interfaces
{
    public interface IUserService
    {
        public Task<GridviewResponseDTO<Users>> GetUsers(GridviewRequestDTO request);
        public Task<Users> GetUser(string id);
        public Task<bool> Insert(Users user);
        public Task<bool> Update(Users user);
        public Task<bool> Delete(string id);
    }
}
