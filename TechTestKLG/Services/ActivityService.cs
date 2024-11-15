using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechTestKLG.Data;
using TechTestKLG.DTO;
using TechTestKLG.Enums;
using TechTestKLG.Models;
using TechTestKLG.Services.Interfaces;

namespace TechTestKLG.Services
{
    public class ActivityService : IActivityService
    {
        private readonly DataContext context;
        private readonly ILogger<ActivityService> logger;

        public ActivityService(DataContext _context, ILogger<ActivityService> _logger)
        {
            context = _context;
            logger = _logger;
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                var activity = await context.Activities.FindAsync(id);
                if (activity == null) return false;

                context.Remove(activity);
                await context.SaveChangesAsync();
                logger.LogInformation($"Deleted activity with id = {id}");
                return true;
            }
            catch (Exception err)
            {
                logger.LogError(err, $"Error deleting activity with id = {id}");
                return false;
            }
        }

        public async Task<GridviewResponseDTO<Activities>> GetActivity(GridviewRequestDTO request)
        {
            try
            {
                var query = context.Activities.AsQueryable();

                if (!string.IsNullOrEmpty(request?.Search))
                {
                    var searchKey = request.Search.ToLower();
                    query = query.Where(w => w.Id.ToLower().Contains(searchKey) ||
                                             w.Subject.ToLower().Contains(searchKey) ||
                                             w.Description.ToLower().Contains(searchKey));
                }


                var totalUsers = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalUsers / (double)request.Length);

                var data = await query.Skip((request.Start - 1) * request.Length)
                                    .Take(request.Length)
                                    .ToListAsync();

                var result = new GridviewResponseDTO<Activities>
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
                logger.LogError(ex, "Error fetching activities");
                return new GridviewResponseDTO<Activities>()
                {
                    Data = new List<Activities>(),
                    Draw = 1,
                    RecordsTotal = 0,
                    RecordsFiltered = 0
                };
            }
        }

        public async Task<Activities> GetActivity(string id)
        {
            try
            {
                return await context.Activities.AsNoTracking().FirstOrDefaultAsync(fd => fd.Id == id);
            }
            catch (Exception err)
            {
                logger.LogError(err, $"Error fetching activity with id = {id}");
                return default;
            }
        }

        public async Task<bool> Insert(Activities activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));

            try
            {
                context.Activities.Add(activity);
                await context.SaveChangesAsync();

                logger.LogInformation($"Inserted activity with id = {activity.Id}");
                return true;
            }
            catch (Exception err)
            {
                logger.LogError(err, $"Error inserting activity with id = {activity.Id}");
                return false;
            }
        }

        public async Task<bool> Update(Activities activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));

            try
            {
                var objUpd = await context.Activities.FirstOrDefaultAsync(fd => fd.Id == activity.Id);

                if (objUpd != null)
                {
                    var isUnmarked = activity.Status == (int)ActivityStatus.UNMARKED;

                    objUpd.Subject = isUnmarked ? activity.Subject : objUpd.Subject;
                    objUpd.Description = isUnmarked ? activity.Description : objUpd.Description;
                    objUpd.Status = activity.Status;
                    context.Entry(objUpd).State = EntityState.Modified;

                    logger.LogInformation($"Updated activity with id = {activity.Id}");
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception err)
            {
                logger.LogError(err, $"Error updating activity with id = {activity.Id}");
                return false;
            }
        }
    }
}
