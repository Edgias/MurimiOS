using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.SharedKernel;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal abstract class BaseEntityConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity: BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(be => be.Id);

            builder.Property(be => be.Id).ValueGeneratedOnAdd();

            builder.Ignore(be => be.Events);
        }
    }
}
