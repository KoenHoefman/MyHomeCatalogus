using Microsoft.AspNetCore.Identity;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Services
{
	public class RoleService(UserManager<ApplicationUser> userManager) : IRoleService
	{
		private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

		public async Task AssignRoleAsync(ApplicationUser user, string roleName)
		{
			if (!await _userManager.IsInRoleAsync(user, roleName))
			{
				await _userManager.AddToRoleAsync(user, roleName);
			}
		}

		public async Task RemoveRoleAsync(ApplicationUser user, string roleName)
		{
			if (await _userManager.IsInRoleAsync(user, roleName))
			{
				await _userManager.RemoveFromRoleAsync(user, roleName);
			}
		}

		public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
		{
			return await _userManager.GetRolesAsync(user);
		}
	}
}
