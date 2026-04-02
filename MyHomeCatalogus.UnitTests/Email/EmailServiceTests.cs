using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyHomeCatalogus.Email;
using MyHomeCatalogus.Settings;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Email
{
	public class EmailServiceTests
	{
		private Mock<IOptions<EmailSettings>> CreateEmailSettingsMock()
		{
			var settings = new EmailSettings
			{
				SenderName = "Test Sender",
				SenderEmail = "test@example.com",
				SmtpServer = "smtp.example.com",
				SmtpPort = 587,
				Username = "testuser",
				AppPassword = "testpass"
			};

			var optionsMock = new Mock<IOptions<EmailSettings>>();
			optionsMock.Setup(x => x.Value).Returns(settings);
			return optionsMock;
		}

		[Fact]
		public void Ctor_Should_Throw_When_Settings_Is_Null()
		{
			IOptions<EmailSettings> nullSettings = null!;
			var logger = Mock.Of<ILogger<EmailService>>();

			Assert.Throws<ArgumentNullException>(() => new EmailService(nullSettings, logger));
		}

		[Fact]
		public void Ctor_Should_Throw_When_Logger_Is_Null()
		{
			var settings = CreateEmailSettingsMock();
			ILogger<EmailService> nullLogger = null!;

			Assert.Throws<ArgumentNullException>(() => new EmailService(settings.Object, nullLogger));
		}

		[Fact]
		public void Ctor_Should_Initialize_When_All_Parameters_Are_Not_Null()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();

			var sender = new EmailService(settings.Object, logger);

			Assert.NotNull(sender);
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Email_Is_Null()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			string? email = null;

			await Assert.ThrowsAsync<ArgumentNullException>(() =>
				sender.SendEmailAsync(email!, "Subject", "<p>Message</p>"));
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Email_Is_Empty()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			await Assert.ThrowsAsync<ArgumentException>(() =>
				sender.SendEmailAsync("", "Subject", "<p>Message</p>"));
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Email_Is_Whitespace()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			await Assert.ThrowsAsync<ArgumentException>(() =>
				sender.SendEmailAsync("   ", "Subject", "<p>Message</p>"));
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Subject_Is_Null()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			string? subject = null;

			await Assert.ThrowsAsync<ArgumentNullException>(() =>
				sender.SendEmailAsync("test@example.com", subject!, "<p>Message</p>"));
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Subject_Is_Empty()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			await Assert.ThrowsAsync<ArgumentException>(() =>
				sender.SendEmailAsync("test@example.com", "", "<p>Message</p>"));
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Subject_Is_Whitespace()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			await Assert.ThrowsAsync<ArgumentException>(() =>
				sender.SendEmailAsync("test@example.com", "   ", "<p>Message</p>"));
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Message_Is_Null()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			string? message = null;

			await Assert.ThrowsAsync<ArgumentNullException>(() =>
				sender.SendEmailAsync("test@example.com", "Subject", message!));
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Message_Is_Empty()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			await Assert.ThrowsAsync<ArgumentException>(() =>
				sender.SendEmailAsync("test@example.com", "Subject", ""));
		}

		[Fact]
		public async Task SendEmailAsync_Should_Throw_When_Message_Is_Whitespace()
		{
			var settings = CreateEmailSettingsMock();
			var logger = Mock.Of<ILogger<EmailService>>();
			var sender = new EmailService(settings.Object, logger);

			await Assert.ThrowsAsync<ArgumentException>(() =>
				sender.SendEmailAsync("test@example.com", "Subject", "   "));
		}

	}
}
