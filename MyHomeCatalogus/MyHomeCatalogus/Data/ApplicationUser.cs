using Microsoft.AspNetCore.Identity;

namespace MyHomeCatalogus.Data
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
		public bool IsApproved { get; set; } = false;
	}
}
