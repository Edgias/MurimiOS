using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Projections;
using GrantedTraceLinkProjection = global::EdgIQ.MurimiOS.Application.Common.Abstractions.GrantedTraceLinkProjection;
internal static class GrantedTraceLinkProjectionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var grantedTraceLinks = modelBuilder.Entity<GrantedTraceLinkProjection>();

        grantedTraceLinks.HasNoKey().ToView("granted_trace_link_projection", "supply");

    }
}
