using Microsoft.AspNetCore.Identity;
using MyHomeCatalogus.Authorization.Roles;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Email;
using MyHomeCatalogus.Interfaces;

namespace MyHomeCatalogus.Services
{
	public class UserNotificationService : IUserNotificationService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IEmailService _emailService;
		private readonly ILogger<UserNotificationService> _logger;

		public UserNotificationService(
			UserManager<ApplicationUser> userManager,
			IEmailService emailService,
			ILogger<UserNotificationService> logger)
		{
			ArgumentNullException.ThrowIfNull(userManager);
			ArgumentNullException.ThrowIfNull(emailService);
			ArgumentNullException.ThrowIfNull(logger);

			_userManager = userManager;
			_emailService = emailService;
			_logger = logger;
		}

		public async Task NotifyAdminsNewUserConfirmedAsync(ApplicationUser newUser)
		{
			try
			{
				var admins = await _userManager.GetUsersInRoleAsync(RoleConstants.Admin);
				if (admins == null || !admins.Any())
				{
					_logger.LogInformation("No admins found to notify for new confirmed user {UserId}.", newUser.Id);
					return;
				}

				var subject = "New user registered and confirmed";
				var htmlMessage = $"User <strong>{newUser.UserName}</strong> ({newUser.Email}) completed registration and confirmed their email.";

				foreach (var admin in admins)
				{
					if (string.IsNullOrWhiteSpace(admin.Email))
					{
						continue;
					}

					await _emailService.SendEmailAsync(admin.Email, subject, htmlMessage);
					_logger.LogInformation("Sent admin notification for new confirmed user {UserId} to admin {AdminId}.", newUser.Id, admin.Id);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error sending admin notification for new confirmed user {UserId}.", newUser.Id);
			}
		}
	}
}
