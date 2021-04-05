using Murimi.ApplicationCore.Entities;
using Murimi.ApplicationCore.Exceptions;
using Murimi.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Murimi.Infrastructure.Data
{
    public class AgrikDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public AgrikDbContext(DbContextOptions<AgrikDbContext> options, 
            IDomainEventDispatcher domainEventDispatcher)
            :base(options)
        {
            _domainEventDispatcher = domainEventDispatcher;
        }

        public DbSet<Crop> Crops { get; set; }

        public DbSet<CropCategory> CropCategories { get; set; }

        public DbSet<CropUnit> CropUnits { get; set; }

        public DbSet<CropVariety> CropVarieties { get; set; }

        public DbSet<Field> Fields { get; set; }

        public DbSet<FieldMeasurement> FieldMeasurements { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<OwnershipType> OwnershipTypes { get; set; }

        public DbSet<Season> Seasons { get; set; }

        public DbSet<SeasonStatus> SeasonStatuses { get; set; }

        public DbSet<SoilType> SoilTypes { get; set; }

        public DbSet<WorkItem> WorkItems { get; set; }

        public DbSet<WorkItemCategory> WorkItemCategories { get; set; }

        public DbSet<WorkItemStatus> WorkItemStatuses { get; set; }

        public DbSet<WorkItemSubCategory> WorkItemSubCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                int result = await base.SaveChangesAsync(cancellationToken);

                // dispatch events only if save was successful
                var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                    .Select(e => e.Entity)
                    .Where(e => e.Events.Any())
                    .ToArray();

                foreach (var entity in entitiesWithEvents)
                {
                    var events = entity.Events.ToArray();

                    entity.Events.Clear();

                    foreach (var domainEvent in events)
                    {
                        _domainEventDispatcher.Dispatch(domainEvent);
                    }
                }

                return result;
            }

            catch(DbUpdateException e)
            {
                throw new DataStoreException(e.Message, e);
            }

        }
    }
}
