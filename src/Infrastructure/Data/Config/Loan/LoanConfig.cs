using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Loan;
using Loan = global::EdgIQ.MurimiOS.Domain.Loan.Loan;


internal class LoanConfig : BaseEntityConfig<Loan>
{
    public override void Configure(EntityTypeBuilder<Loan> builder)
    {
        base.Configure(builder);

        builder.Property(l => l.Name)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(l => l.Description)
            .HasMaxLength(500);

        //builder.Property(l => l.PrincipalAmount)
        //    .HasColumnType("decimal(18,2)");

        //builder.Property(l => l.InterestRate)
        //    .HasColumnType("decimal(18,2)");

        //builder.Property(l => l.EffectiveRate)
        //    .HasColumnType("decimal(18,2)");

    }
}


