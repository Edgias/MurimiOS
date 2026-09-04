namespace Edgias.MurimiOS.API.Loans;

public static class LoanMapper
{
    public static LoanResponse AsApiResponse(this Loan entity)
    {
        LoanResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Duration = entity.Duration,
            CurrencyId = entity.CurrencyId,
            CurrencyName = entity.Currency?.Name!,
            EffectiveRate = entity.EffectiveRate,
            InterestRate = entity.InterestRate,
            PrincipalAmount = entity.PrincipalAmount,
        };

        return response;
    }

    public static Loan ToEntity(this LoanRequest request)
    {
        Loan entity = new(request.Name, request.Description, request.Duration,
            request.PrincipalAmount, request.InterestRate, request.EffectiveRate,
            request.CurrencyId);

        return entity;
    }

    public static void Update(this Loan entity, LoanRequest request)
    {
        entity.Update(request.Name, request.Description, request.Duration,
            request.PrincipalAmount, request.InterestRate, request.EffectiveRate);
    }
}

