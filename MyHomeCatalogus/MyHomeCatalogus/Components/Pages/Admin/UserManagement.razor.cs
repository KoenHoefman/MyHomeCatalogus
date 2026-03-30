using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Authorization.Roles;
using MyHomeCatalogus.Components.Forms;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Data;

namespace MyHomeCatalogus.Components.Pages.Admin
{
	public partial class UserManagement
	{
		[Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
		[Inject] private UserManager<ApplicationUser> UserManager { get; set; } = null!;
		[Inject] private ILogger<UserManagement> Logger { get; set; } = null!;
		[Inject] public required IToastService ToastService { get; set; }

		private List<UserListItem> _userItems = new();
		private string? _currentUserId;
		private string? _errorMessage;

		private ConfirmationModal<ApplicationUser> _deleteModal = null!;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				var authState = await AuthStateProvider.GetAuthenticationStateAsync();
				_currentUserId = UserManager.GetUserId(authState.User);

				if (string.IsNullOrEmpty(_currentUserId))
				{
					_errorMessage = "Unable to identify current user.";
					Logger.LogError("Current user ID is null in UserManagement component");
					return;
				}

				await LoadUsers();
			}
			catch (Exception ex)
			{
				_errorMessage = $"Error loading user management: {ex.Message}";
				Logger.LogError(ex, "Error in UserManagement OnInitializedAsync");
			}
		}


		private async Task LoadUsers()
		{
			try
			{
				var users = await UserManager.Users.ToListAsync();
				var items = new List<UserListItem>();

				foreach (var user in users)
				{
					items.Add(new UserListItem
					{
						User = user,
						IsAdmin = await UserManager.IsInRoleAsync(user, RoleConstants.Admin),
						IsRegularUser = await UserManager.IsInRoleAsync(user, RoleConstants.RegularUser)
					});
				}

				_userItems = items;
				Logger.LogInformation("Loaded {UserCount} user display item(s)", _userItems.Count);
			}
			catch (Exception ex)
			{
				_errorMessage = $"Error loading users: {ex.Message}";
				Logger.LogError(ex, "Error loading users");
			}
		}


		private async Task ToggleApproval(ApplicationUser user)
		{
			// Prevent revoking your own access
			if (user.Id == _currentUserId)
			{
				ToastService.ShowToast("You cannot change your own approval.");
				return;
			}

			user.IsApproved = !user.IsApproved;
			var result = await UserManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				_errorMessage = "Failed to update status.";
				Logger.LogError("Failed to toggle approval for user {UserId}: {Errors}",
					user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
			}
			else
			{
				ToastService.ShowToast($"Approval status updated for {user.Email}.", ToastLevel.Success);
			}

			await LoadUsers();
		}


		private async Task HandleRoleChange(ApplicationUser user, ChangeEventArgs e)
		{
			var selectedRole = e.Value?.ToString();

			if (string.IsNullOrEmpty(selectedRole))
			{
				return;
			}

			// Prevent changing your own role
			if (user.Id == _currentUserId)
			{
				ToastService.ShowToast("You cannot change your own approval.");
				return;
			}

			try
			{
				// Get current roles and remove them
				var currentRoles = await UserManager.GetRolesAsync(user);
				var removeResult = await UserManager.RemoveFromRolesAsync(user, currentRoles);

				if (!removeResult.Succeeded)
				{
					_errorMessage = "Failed to remove previous role.";
					Logger.LogError("Failed to remove roles from user {UserId}: {Errors}",
						user.Id, string.Join(", ", removeResult.Errors.Select(error => error.Description)));
					await LoadUsers();
					return;
				}

				// Add new role
				var addResult = await UserManager.AddToRoleAsync(user, selectedRole);

				if (!addResult.Succeeded)
				{
					_errorMessage = "Failed to assign new role.";
					Logger.LogError("Failed to add role {Role} to user {UserId}: {Errors}",
						selectedRole, user.Id, string.Join(", ", addResult.Errors.Select(error => error.Description)));
					await LoadUsers();
					return;
				}

				ToastService.ShowToast($"Role updated to {selectedRole} for {user.Email}.", ToastLevel.Success);
				Logger.LogInformation("User {UserId} role changed to {Role} by admin {AdminId}",
					user.Id, selectedRole, _currentUserId);

				await LoadUsers();
			}
			catch (Exception ex)
			{
				_errorMessage = "An error occurred while updating the role.";
				Logger.LogError(ex, "Error updating role for user {UserId}", user.Id);
				await LoadUsers();
			}
		}


		private async Task DeleteUser(ApplicationUser user)
		{
			// Don't delete yourself!
			if (user.Id == _currentUserId) return;

			var result = await UserManager.DeleteAsync(user);
			if (result.Succeeded)
			{
				ToastService.ShowToast($"User {user.Email} has been deleted.", ToastLevel.Success);
				await LoadUsers();
			}
			else
			{
				// ToDo: this can happen if there are linked records to the user (e.g. shoppinglists) 
				// This can be fixed by adding a cascading delete, which will delete all linked data,
				//or some kind of SoftDelete (but the latter might have impact on other things like authorization??)
				_errorMessage = $"Could not delete user. They might have linked data. Error: {result.Errors.FirstOrDefault()?.Description}";
				Logger.LogError("Failed to delete user {UserId}: {Errors}",
					user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
			}
		}
	}

	public class UserListItem
	{
		public ApplicationUser User { get; set; } = null!;
		public bool IsAdmin { get; set; }
		public bool IsRegularUser { get; set; }
	}
}
