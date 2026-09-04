using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Projections;
using GrantedHarvestLotProjection = global::EdgIQ.MurimiOS.Application.Common.Abstractions.GrantedHarvestLotProjection;
internal static class GrantedHarvestLotProjectionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var grantedHarvestLots = modelBuilder.Entity<GrantedHarvestLotProjection>();

        grantedHarvestLots.HasNoKey().ToView("granted_harvest_lot_projection", "supply");

    }
}
