using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Loan;
using LoanRequest = global::EdgIQ.MurimiOS.API.Endpoints.Loan.Shared.LoanRequest;


internal static class LoanEndpoints
{
    public static RouteGroupBuilder MapLoans(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/loans");
        group.WithTags("Loans");
        group.WithParameterValidation(typeof(LoanRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Loan.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Loan.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Loan.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Loan.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Loan.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

