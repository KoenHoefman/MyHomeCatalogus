using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyHomeCatalogus.Authorization.Roles;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Authorization.Roles
{
    public class RoleSeederTests
    {
        private Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
        {
            return new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<IdentityRole>>(),
                new List<IRoleValidator<IdentityRole>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<RoleManager<IdentityRole>>>());
        }

        [Fact]
        public async Task SeedRolesAsync_CreatesRoles_WhenTheyDoNotExist()
        {
            var roleManager = CreateRoleManagerMock();

            roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.Admin)).ReturnsAsync(false);
            roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.RegularUser)).ReturnsAsync(false);
            roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);

            await RoleSeeder.SeedRolesAsync(serviceProvider.Object);

            roleManager.Verify(x => x.RoleExistsAsync(RoleConstants.Admin), Times.Once);
            roleManager.Verify(x => x.RoleExistsAsync(RoleConstants.RegularUser), Times.Once);
            roleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.Admin)), Times.Once);
            roleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.RegularUser)), Times.Once);
        }

        [Fact]
        public async Task SeedRolesAsync_SkipsRoles_WhenTheyAlreadyExist()
        {
            var roleManager = CreateRoleManagerMock();

            roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.Admin)).ReturnsAsync(true);
            roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.RegularUser)).ReturnsAsync(true);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);

            await RoleSeeder.SeedRolesAsync(serviceProvider.Object);

            roleManager.Verify(x => x.RoleExistsAsync(RoleConstants.Admin), Times.Once);
            roleManager.Verify(x => x.RoleExistsAsync(RoleConstants.RegularUser), Times.Once);
            roleManager.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        [Fact]
        public async Task SeedRolesAsync_CreatesOnlyMissingRoles()
        {
            var roleManager = CreateRoleManagerMock();

            roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.Admin)).ReturnsAsync(true);
            roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.RegularUser)).ReturnsAsync(false);
            roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);

            await RoleSeeder.SeedRolesAsync(serviceProvider.Object);

            roleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.RegularUser)), Times.Once);
            roleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.Admin)), Times.Never);
        }
    }
}