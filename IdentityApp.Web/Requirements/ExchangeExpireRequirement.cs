using Microsoft.AspNetCore.Authorization;

namespace IdentityApp.Web.Requirements
{
    public class ExchangeExpireRequirement:IAuthorizationRequirement
    {

    }

    public class ExchangeExpireRequirementHandler:AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "ExchangeExpireDate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var exchangeClaim = (context.User.FindFirst("ExchangeExpireDate"))!;
            var exchangeClaimValue = Convert.ToDateTime(exchangeClaim.Value);

            if (exchangeClaimValue < DateTime.Now)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
