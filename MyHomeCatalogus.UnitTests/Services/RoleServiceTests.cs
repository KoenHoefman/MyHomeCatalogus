using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Services;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
    public class RoleServiceTests
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
    public void Ctor_Should_Throw_When_Logger_Is_Null()
    {
        var mockUserManager = CreateUserManagerMock();
        ILogger<RoleService> nullLogger = null!;

        Assert.Throws<ArgumentNullException>(() => new RoleService(mockUserManager.Object, nullLogger));
    }



        [Fact]
        public void Ctor_Should_Throw_When_UserManager_Is_Null()
        {
            UserManager<ApplicationUser> nullUserManager = null!;

            Assert.Throws<ArgumentNullException>(() => new RoleService(nullUserManager, Mock.Of<ILogger<RoleService>>()));
        }

        [Fact]
        public async Task AssignRoleAsync_Should_AddRole_When_UserNotInRole()
        {
            var userManager = CreateUserManagerMock();
            var service = new RoleService(userManager.Object, Mock.Of<ILogger<RoleService>>());
            var user = new ApplicationUser { Id = "user1" };
            var roleName = "TestRole";

            userManager.Setup(x => x.IsInRoleAsync(user, roleName)).ReturnsAsync(false);
            userManager.Setup(x => x.AddToRoleAsync(user, roleName)).ReturnsAsync(IdentityResult.Success);

            await service.AssignRoleAsync(user, roleName);

            userManager.Verify(x => x.AddToRoleAsync(user, roleName), Times.Once);
        }

        [Fact]
        public async Task AssignRoleAsync_Should_NotAddRole_When_UserAlreadyInRole()
        {
            var userManager = CreateUserManagerMock();
            var service = new RoleService(userManager.Object, Mock.Of<ILogger<RoleService>>());
            var user = new ApplicationUser { Id = "user1" };
            var roleName = "TestRole";

            userManager.Setup(x => x.IsInRoleAsync(user, roleName)).ReturnsAsync(true);

            await service.AssignRoleAsync(user, roleName);

            userManager.Verify(x => x.AddToRoleAsync(user, roleName), Times.Never);
        }

        [Fact]
        public async Task RemoveRoleAsync_Should_RemoveRole_When_UserInRole()
        {
            var userManager = CreateUserManagerMock();
            var service = new RoleService(userManager.Object, Mock.Of<ILogger<RoleService>>());
            var user = new ApplicationUser { Id = "user1" };
            var roleName = "TestRole";

            userManager.Setup(x => x.IsInRoleAsync(user, roleName)).ReturnsAsync(true);
            userManager.Setup(x => x.RemoveFromRoleAsync(user, roleName)).ReturnsAsync(IdentityResult.Success);

            await service.RemoveRoleAsync(user, roleName);

            userManager.Verify(x => x.RemoveFromRoleAsync(user, roleName), Times.Once);
        }

        [Fact]
        public async Task RemoveRoleAsync_Should_NotRemoveRole_When_UserNotInRole()
        {
            var userManager = CreateUserManagerMock();
            var service = new RoleService(userManager.Object, Mock.Of<ILogger<RoleService>>());
            var user = new ApplicationUser { Id = "user1" };
            var roleName = "TestRole";

            userManager.Setup(x => x.IsInRoleAsync(user, roleName)).ReturnsAsync(false);

            await service.RemoveRoleAsync(user, roleName);

            userManager.Verify(x => x.RemoveFromRoleAsync(user, roleName), Times.Never);
        }

        [Fact]
        public async Task GetUserRolesAsync_Should_ReturnUserRoles()
        {
            var userManager = CreateUserManagerMock();
            var service = new RoleService(userManager.Object, Mock.Of<ILogger<RoleService>>());
            var user = new ApplicationUser { Id = "user1" };
            var expectedRoles = new List<string> { "Role1", "Role2" };

            userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(expectedRoles);

            var result = await service.GetUserRolesAsync(user);

            Assert.Equal(expectedRoles, result);
        }
    }
}