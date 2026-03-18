using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;

namespace MyHomeCatalogus.Components.Pages.Admin
{
	public partial class UserApproval(UserManager<ApplicationUser> userManager)
	{
		private List<ApplicationUser> users = new();

		protected override async Task OnInitializedAsync()
		{
			await LoadUsers();
		}

		private async Task LoadUsers()
		{
			users = await userManager.Users.ToListAsync();
		}

		private async Task ToggleApproval(ApplicationUser user)
		{
			user.IsApproved = !user.IsApproved;
			await userManager.UpdateAsync(user);
			await LoadUsers();
		}
	}
}
