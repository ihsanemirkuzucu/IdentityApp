using Microsoft.AspNetCore.Authorization;

namespace IdentityApp.Web.Requirements
{
    public class ViolanceRequirement:IAuthorizationRequirement
    {
        public int ThresholdAge  { get; set; }
    }

    public class ViolanceRequirementHandler:AuthorizationHandler<ViolanceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolanceRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "birthdate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var birthdateClaim = (context.User.FindFirst("birthdate"))!;
            var birthdateClaimValue = Convert.ToDateTime(birthdateClaim.Value);

            var today = DateTime.Now;
            var age = today.Year - birthdateClaimValue.Year;
            if (birthdateClaimValue > today.AddDays(-age)) age--;

            if (requirement.ThresholdAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
