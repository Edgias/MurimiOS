using Edgias.Agrik.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edgias.Agrik.Infrastructure.Data.ModelConfiguration
{
    internal abstract class BaseEntityConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity: BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(be => be.Id);

            builder.Property(be => be.Id).ValueGeneratedOnAdd();

            builder.Property(be => be.CreatedBy).IsRequired().HasMaxLength(50);

            builder.Property(be => be.LastModifiedBy).IsRequired().HasMaxLength(50);

            builder.Ignore(be => be.Events);
        }
    }
}
