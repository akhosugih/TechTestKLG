using Microsoft.EntityFrameworkCore;
using TechTestKLG.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TechTestKLG.Data
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Activities> Activities { get; set; }
        public DbSet<Users> Users { get; set; }

        public override int SaveChanges()
        {
            var addedEntities = ChangeTracker.Entries<Activities>()
                                              .Where(e => e.State == EntityState.Added)
                                              .ToList();

            foreach (var entity in addedEntities)
            {
                if (string.IsNullOrEmpty(entity.Entity.Id))
                {
                    var lastActivity = Activities.OrderByDescending(a => a.Id)
                                                  .FirstOrDefault();

                    var nextSequence = lastActivity != null
                        ? int.Parse(lastActivity.Id.Split('-')[1]) + 1
                        : 1;

                    entity.Entity.Id = $"AC-{nextSequence:000}";
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var addedEntities = ChangeTracker.Entries<Activities>()
                                              .Where(e => e.State == EntityState.Added)
                                              .ToList();

            foreach (var entity in addedEntities)
            {
                if (string.IsNullOrEmpty(entity.Entity.Id))
                {
                    var lastActivity = await Activities.OrderByDescending(a => a.Id)
                                                        .FirstOrDefaultAsync(cancellationToken);

                    var nextSequence = lastActivity != null
                        ? int.Parse(lastActivity.Id.Split('-')[1]) + 1
                        : 1;

                    entity.Entity.Id = $"AC-{nextSequence:000}";
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
