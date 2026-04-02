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
				new List<IRoleValidator<IdentityRole>>(),
				Mock.Of<ILookupNormalizer>(),
				Mock.Of<IdentityErrorDescriber>(),
				Mock.Of<ILogger<RoleManager<IdentityRole>>>());
		}

		private Mock<ILoggerFactory> CreateLoggerFactoryMock()
		{
			var loggerMock = new Mock<ILogger>();
			var factoryMock = new Mock<ILoggerFactory>();
			factoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
			return factoryMock;
		}

		[Fact]
		public async Task SeedRolesAsync_CreatesRoles_WhenTheyDoNotExist()
		{
			var roleManager = CreateRoleManagerMock();
			var loggerFactory = CreateLoggerFactoryMock();

			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.Admin)).ReturnsAsync(false);
			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.RegularUser)).ReturnsAsync(false);
			roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);
			serviceProvider.Setup(x => x.GetService(typeof(ILoggerFactory))).Returns(loggerFactory.Object);

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
			var loggerFactory = CreateLoggerFactoryMock();

			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.Admin)).ReturnsAsync(true);
			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.RegularUser)).ReturnsAsync(true);

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);
			serviceProvider.Setup(x => x.GetService(typeof(ILoggerFactory))).Returns(loggerFactory.Object);

			await RoleSeeder.SeedRolesAsync(serviceProvider.Object);

			roleManager.Verify(x => x.RoleExistsAsync(RoleConstants.Admin), Times.Once);
			roleManager.Verify(x => x.RoleExistsAsync(RoleConstants.RegularUser), Times.Once);
			roleManager.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
		}

		[Fact]
		public async Task SeedRolesAsync_CreatesOnlyMissingRoles()
		{
			var roleManager = CreateRoleManagerMock();
			var loggerFactory = CreateLoggerFactoryMock();

			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.Admin)).ReturnsAsync(true);
			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.RegularUser)).ReturnsAsync(false);
			roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);
			serviceProvider.Setup(x => x.GetService(typeof(ILoggerFactory))).Returns(loggerFactory.Object);

			await RoleSeeder.SeedRolesAsync(serviceProvider.Object);

			roleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.RegularUser)), Times.Once);
			roleManager.Verify(x => x.CreateAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.Admin)), Times.Never);
		}

		[Fact]
		public async Task SeedRolesAsync_Should_Throw_When_ServiceProvider_Is_Null()
		{
			IServiceProvider? nullProvider = null;

			await Assert.ThrowsAsync<ArgumentNullException>(() =>
				RoleSeeder.SeedRolesAsync(nullProvider!));
		}

		[Fact]
		public async Task SeedRolesAsync_Should_Log_When_Role_Created()
		{
			var roleManager = CreateRoleManagerMock();
			var loggerMock = new Mock<ILogger>();
			var loggerFactory = new Mock<ILoggerFactory>();
			loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.Admin)).ReturnsAsync(false);
			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.RegularUser)).ReturnsAsync(true);
			roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);
			serviceProvider.Setup(x => x.GetService(typeof(ILoggerFactory))).Returns(loggerFactory.Object);

			await RoleSeeder.SeedRolesAsync(serviceProvider.Object);

			loggerMock.Verify(
				x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"'{RoleConstants.Admin}' created successfully")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}

		[Fact]
		public async Task SeedRolesAsync_Should_Log_When_Role_Already_Exists()
		{
			var roleManager = CreateRoleManagerMock();
			var loggerMock = new Mock<ILogger>();
			var loggerFactory = new Mock<ILoggerFactory>();
			loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

			roleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);
			serviceProvider.Setup(x => x.GetService(typeof(ILoggerFactory))).Returns(loggerFactory.Object);

			await RoleSeeder.SeedRolesAsync(serviceProvider.Object);

			loggerMock.Verify(
				x => x.Log(
					LogLevel.Debug,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("already exists")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Exactly(2));
		}

		[Fact]
		public async Task SeedRolesAsync_Should_Log_Error_When_Role_Creation_Fails()
		{
			var roleManager = CreateRoleManagerMock();
			var loggerMock = new Mock<ILogger>();
			var loggerFactory = new Mock<ILoggerFactory>();
			loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.Admin)).ReturnsAsync(false);
			roleManager.Setup(x => x.RoleExistsAsync(RoleConstants.RegularUser)).ReturnsAsync(true);

			var error = new IdentityError { Code = "DuplicateRoleName", Description = "Role name already exists" };
			roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed(error));

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);
			serviceProvider.Setup(x => x.GetService(typeof(ILoggerFactory))).Returns(loggerFactory.Object);

			await RoleSeeder.SeedRolesAsync(serviceProvider.Object);

			loggerMock.Verify(
				x => x.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to create role")),
					It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}

		[Fact]
		public async Task SeedRolesAsync_Should_Throw_When_Exception_Occurs()
		{
			var roleManager = CreateRoleManagerMock();
			var loggerMock = new Mock<ILogger>();
			var loggerFactory = new Mock<ILoggerFactory>();
			loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

			roleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
				.ThrowsAsync(new InvalidOperationException("Test exception"));

			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider.Setup(x => x.GetService(typeof(RoleManager<IdentityRole>))).Returns(roleManager.Object);
			serviceProvider.Setup(x => x.GetService(typeof(ILoggerFactory))).Returns(loggerFactory.Object);

			await Assert.ThrowsAsync<InvalidOperationException>(() =>
				RoleSeeder.SeedRolesAsync(serviceProvider.Object));
		}
	}
}