using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyHomeCatalogus.Authorization.Roles;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Email;
using MyHomeCatalogus.Services;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
    public class UserNotificationServiceTests
    {
        private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new List<IUserValidator<ApplicationUser>>(),
                new List<IPasswordValidator<ApplicationUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());
        }

        [Fact]
        public async Task NotifyAdminsNewUserConfirmedAsync_SendsEmailToAllAdmins()
        {
            var userManager = CreateUserManagerMock();

            var admin1 = new ApplicationUser { Id = "admin1", UserName = "admin1", Email = "admin1@example.com" };
            var admin2 = new ApplicationUser { Id = "admin2", UserName = "admin2", Email = "admin2@example.com" };

            userManager
                .Setup(x => x.GetUsersInRoleAsync(RoleConstants.Admin))
                .ReturnsAsync(new List<ApplicationUser> { admin1, admin2 });

            var emailService = new Mock<IEmailService>();
            var logger = new Mock<ILogger<UserNotificationService>>();
            var service = new UserNotificationService(userManager.Object, emailService.Object, logger.Object);

            var newUser = new ApplicationUser { Id = "newuser", UserName = "newuser", Email = "newuser@example.com" };

            await service.NotifyAdminsNewUserConfirmedAsync(newUser);

            emailService.Verify(x => x.SendEmailAsync("admin1@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            emailService.Verify(x => x.SendEmailAsync("admin2@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task NotifyAdminsNewUserConfirmedAsync_SkipsAdminsWithoutEmail()
        {
            var userManager = CreateUserManagerMock();

            var admin1 = new ApplicationUser { Id = "admin1", UserName = "admin1", Email = "" };
            var admin2 = new ApplicationUser { Id = "admin2", UserName = "admin2", Email = "admin2@example.com" };

            userManager
                .Setup(x => x.GetUsersInRoleAsync(RoleConstants.Admin))
                .ReturnsAsync(new List<ApplicationUser> { admin1, admin2 });

            var emailService = new Mock<IEmailService>();
            var logger = new Mock<ILogger<UserNotificationService>>();
            var service = new UserNotificationService(userManager.Object, emailService.Object, logger.Object);

            var newUser = new ApplicationUser { Id = "newuser", UserName = "newuser", Email = "newuser@example.com" };

            await service.NotifyAdminsNewUserConfirmedAsync(newUser);

            emailService.Verify(x => x.SendEmailAsync("admin2@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            emailService.Verify(x => x.SendEmailAsync("", It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task NotifyAdminsNewUserConfirmedAsync_NoAdmins_NoEmailsSent()
        {
            var userManager = CreateUserManagerMock();

            userManager
                .Setup(x => x.GetUsersInRoleAsync(RoleConstants.Admin))
                .ReturnsAsync(new List<ApplicationUser>());

            var emailService = new Mock<IEmailService>();
            var logger = new Mock<ILogger<UserNotificationService>>();
            var service = new UserNotificationService(userManager.Object, emailService.Object, logger.Object);

            var newUser = new ApplicationUser { Id = "newuser", UserName = "newuser", Email = "newuser@example.com" };

            await service.NotifyAdminsNewUserConfirmedAsync(newUser);

            emailService.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
