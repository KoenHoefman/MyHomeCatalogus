using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Settings;

namespace MyHomeCatalogus.Email
{
	public class EmailSender(IOptions<EmailSettings> settings) : IEmailSender<ApplicationUser>, IEmailService
	{
		private readonly EmailSettings _settings = settings.Value;

		// Identity
		public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
			SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

		public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
			SendEmailAsync(email, "Reset your password", $"Reset your password by <a href='{resetLink}'>clicking here</a>.");

		public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
			SendEmailAsync(email, "Reset your password", $"Your reset code is: {resetCode}");

		//ToDo: Switch to Microsoft Graph API to send emails (using Azure)

		// Standard email
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
			message.To.Add(new MailboxAddress("", email));
			message.Subject = subject;

			message.Body = new TextPart("html") { Text = htmlMessage };

			using var client = new SmtpClient();
			try
			{
				await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(_settings.Username, _settings.AppPassword);
				await client.SendAsync(message);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				await client.DisconnectAsync(true);
			}
		}
	}
}
