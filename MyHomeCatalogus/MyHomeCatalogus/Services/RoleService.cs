using Microsoft.AspNetCore.Identity;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Services
{
	public class RoleService(UserManager<ApplicationUser> userManager, ILogger<RoleService> logger) : IRoleService
	{
		private readonly UserManager<ApplicationUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
		private readonly ILogger<RoleService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

		public async Task AssignRoleAsync(ApplicationUser user, string roleName)
		{
			try
			{
				if (!await _userManager.IsInRoleAsync(user, roleName))
				{
					await _userManager.AddToRoleAsync(user, roleName);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error assigning role {roleName} to {userName}.", roleName, user.UserName);
				throw;
			}
		}

		public async Task RemoveRoleAsync(ApplicationUser user, string roleName)
		{
			try
			{
				if (await _userManager.IsInRoleAsync(user, roleName))
				{
					await _userManager.RemoveFromRoleAsync(user, roleName);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error removing role {roleName} from {userName}.", roleName, user.UserName);
				throw;
			}
		}

		public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
		{
			try
			{
				return await _userManager.GetRolesAsync(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting roles for {userName}.", user.UserName);
				throw;
			}
		}
	}
}
