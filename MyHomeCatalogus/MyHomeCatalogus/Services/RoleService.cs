using Microsoft.AspNetCore.Identity;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Services
{
	public class RoleService(UserManager<ApplicationUser> userManager) : IRoleService
	{
		public async Task AssignRoleAsync(ApplicationUser user, string roleName)
		{
			if (!await userManager.IsInRoleAsync(user, roleName))
			{
				await userManager.AddToRoleAsync(user, roleName);
			}
		}

		public async Task RemoveRoleAsync(ApplicationUser user, string roleName)
		{
			if (await userManager.IsInRoleAsync(user, roleName))
			{
				await userManager.RemoveFromRoleAsync(user, roleName);
			}
		}

		public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
		{
			return await userManager.GetRolesAsync(user);
		}
	}
}
