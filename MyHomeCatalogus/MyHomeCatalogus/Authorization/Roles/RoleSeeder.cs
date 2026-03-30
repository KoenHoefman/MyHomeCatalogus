using Microsoft.AspNetCore.Identity;

namespace MyHomeCatalogus.Authorization.Roles
{
	public static class RoleSeeder
	{
		public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var roles = new[] { RoleConstants.Admin, RoleConstants.RegularUser };

			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}
		}
	}
}
