using Microsoft.AspNetCore.Identity;

namespace MyHomeCatalogus.Authorization.Roles
{
	/// <summary>
	/// Seeds initial roles into the application.
	/// </summary>
	public static class RoleSeeder
	{
		/// <summary>
		/// Seeds roles asynchronously during application startup.
		/// </summary>
		/// <param name="serviceProvider">The service provider for accessing role manager and logger.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is null.</exception>
		public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
		{
			ArgumentNullException.ThrowIfNull(serviceProvider);

			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger("RoleSeeder");

			var roles = new[] { RoleConstants.Admin, RoleConstants.RegularUser };

			try
			{
				logger.LogInformation("Starting role seeding process. Total roles to process: {RoleCount}.", roles.Length);

				foreach (var role in roles)
				{
					try
					{
						if (!await roleManager.RoleExistsAsync(role))
						{
							var result = await roleManager.CreateAsync(new IdentityRole(role));

							if (result.Succeeded)
							{
								logger.LogInformation("Role '{RoleName}' created successfully.", role);
							}
							else
							{
								var errors = string.Join(", ", result.Errors.Select(e => e.Description));
								logger.LogError("Failed to create role '{RoleName}'. Errors: {Errors}", role, errors);
							}
						}
						else
						{
							logger.LogInformation("Role '{RoleName}' already exists. Skipping creation.", role);
						}
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "Error processing role '{RoleName}' during seeding.", role);
						throw;
					}
				}

				logger.LogInformation("Role seeding process completed successfully.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Role seeding process failed.");
				throw;
			}
		}
	}
}
