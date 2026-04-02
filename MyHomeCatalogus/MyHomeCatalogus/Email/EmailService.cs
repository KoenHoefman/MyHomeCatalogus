using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Settings;

namespace MyHomeCatalogus.Email
{
	public class EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger) : IEmailSender<ApplicationUser>, IEmailService
	{
		private readonly EmailSettings _settings = (settings ?? throw new ArgumentNullException(nameof(settings))).Value;
		private readonly ILogger<EmailService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

		/// <summary>
		/// Sends a confirmation email to the specified user.
		/// </summary>
		public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
		{
			return SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
		}

		/// <summary>
		/// Sends a password reset link to the specified email address.
		/// </summary>
		public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
		{
			return SendEmailAsync(email, "Reset your password", $"Reset your password by <a href='{resetLink}'>clicking here</a>.");
		}

		/// <summary>
		/// Sends a password reset code to the specified email address.
		/// </summary>
		public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
		{
			return SendEmailAsync(email, "Reset your password", $"Your reset code is: {resetCode}");
		}

		//ToDo: Switch to Microsoft Graph API to send emails (using Azure)

		/// <summary>
		/// Sends an email to the specified recipient.
		/// </summary>
		/// <remarks>
		/// If sending fails, the exception is logged but not re-thrown (fire-and-forget pattern).
		/// </remarks>
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{

			ArgumentException.ThrowIfNullOrWhiteSpace(email);
			ArgumentException.ThrowIfNullOrWhiteSpace(subject);
			ArgumentException.ThrowIfNullOrWhiteSpace(htmlMessage);

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
			message.To.Add(new MailboxAddress("", email));
			message.Subject = subject;

			message.Body = new TextPart("html") { Text = htmlMessage };

			using var client = new SmtpClient();
			try
			{
				_logger.LogInformation("Attempting to send email to {Email} with subject '{Subject}'.", email, subject);

				await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
				_logger.LogDebug("Connected to SMTP server {SmtpServer}:{SmtpPort}.", _settings.SmtpServer, _settings.SmtpPort);

				await client.AuthenticateAsync(_settings.Username, _settings.AppPassword);
				_logger.LogDebug("Authenticated to SMTP server.");

				await client.SendAsync(message);
				_logger.LogInformation("Email successfully sent to {Email}.", email);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error sending email to {Email}. Subject: '{Subject}'.", email, subject);
			}
			finally
			{
				try
				{
					await client.DisconnectAsync(true);
				}
				catch (Exception ex)
				{
					_logger.LogWarning(ex, "Error disconnecting from SMTP server.");
				}
			}
		}
	}
}
