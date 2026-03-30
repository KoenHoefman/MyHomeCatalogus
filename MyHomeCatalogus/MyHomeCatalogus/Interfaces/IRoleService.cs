using MyHomeCatalogus.Data;

namespace MyHomeCatalogus.Interfaces
{
	public interface IRoleService
	{
		Task AssignRoleAsync(ApplicationUser user, string roleName);
		Task RemoveRoleAsync(ApplicationUser user, string roleName);
		Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
	}
}
