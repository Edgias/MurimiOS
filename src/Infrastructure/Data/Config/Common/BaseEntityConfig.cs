namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;
using BaseEntity = global::EdgIQ.MurimiOS.Domain.Common.BaseEntity;


internal abstract class BaseEntityConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(be => be.Id);

        builder.Property(be => be.Id).ValueGeneratedOnAdd();

        builder.Ignore(be => be.Events);
    }
}


