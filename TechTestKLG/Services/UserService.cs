using Microsoft.EntityFrameworkCore;
using TechTestKLG.Data;
using TechTestKLG.DTO;
using TechTestKLG.Models;
using TechTestKLG.Services.Interfaces;

namespace TechTestKLG.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(DataContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Delete(string username)
        {
            try
            {
                var user = await _context.Users.FindAsync(username);
                if (user == null) return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Deleted user: {username}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user: {username}");
                return false;
            }
        }

        public async Task<Users> GetUser(string username)
        {
            try
            {
                return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching user: {username}");
                return null;
            }
        }

        public async Task<GridviewResponseDTO<Users>> GetUsers(GridviewRequestDTO request)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(request?.Search))
                {
                    var searchKey = request.Search.ToLower();
                    query = query.Where(u => u.Username.ToLower().Contains(searchKey) ||
                                              u.Name.ToLower().Contains(searchKey));
                }


                var totalUsers = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalUsers / (double)request.Length);

                var data = await query.Skip((request.Start - 1) * request.Length)
                                    .Take(request.Length)
                                    .ToListAsync();

                data.ForEach(f => f.Password = new string('*', 5));

                var result = new GridviewResponseDTO<Users>
                {
                    Data = data,
                    Draw = 1,
                    RecordsTotal = totalUsers,
                    RecordsFiltered = totalPages
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return new GridviewResponseDTO<Users>()
                { 
                    Data = new List<Users>(),
                    Draw = 1,
                    RecordsTotal = 0,
                    RecordsFiltered = 0
                };
            }
        }

        public async Task<bool> Insert(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                if (await _context.Users.AsNoTracking().AnyAsync(u => u.Username.ToLower() == user.Username.ToLower()))
                    throw new InvalidOperationException("Username already exists.");

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Inserted user: {user.Username}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error inserting user: {user.Username}");
                return false;
            }
        }

        public async Task<bool> Update(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                var obUpd = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

                if (obUpd == null)
                    throw new InvalidOperationException("Username not found.");

                obUpd.Password = user.Password;
                obUpd.Name = user.Name;
                _context.Users.Update(obUpd);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated user: {user.Username}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user: {user.Username}");
                return false;
            }
        }
    }
}
