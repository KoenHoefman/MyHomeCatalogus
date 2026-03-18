using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MyHomeCatalogus.Authorization.Requirements;
using MyHomeCatalogus.Data;

namespace MyHomeCatalogus.Authorization.Handlers
{
	public class ApprovedUserHandler(UserManager<ApplicationUser> userManager) : AuthorizationHandler<ApprovedUserRequirement>
	{
		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApprovedUserRequirement requirement)
		{
			var user = await userManager.GetUserAsync(context.User);

			if (user != null && user.IsApproved)
			{
				context.Succeed(requirement);
			}
		}
	}
}
