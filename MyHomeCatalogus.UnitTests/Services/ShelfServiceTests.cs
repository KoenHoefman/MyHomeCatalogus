using Microsoft.EntityFrameworkCore;
using Moq;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
	public class ShelfServiceTests
	{
		[Fact]
		public void Ctor_Should_Throw_When_ContextFactory_Is_Null()
		{
			IDbContextFactory<AppDbContext> nullContext = null!;

			Assert.Throws<ArgumentNullException>(() => new ShelfService(nullContext, Mock.Of<ILogger<ShelfService>>()));
		}
    [Fact]
    public void Ctor_Should_Throw_When_Logger_Is_Null()
    {
        var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
        ILogger<ShelfService> nullLogger = null!;

        Assert.Throws<ArgumentNullException>(() => new ShelfService(mockDbFactory.Object, nullLogger));
    }



		[Fact]
		public void Ctor_Should_Initialize_When_All_Parameters_Are_Not_Null()
		{
			var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();

			var service = new ShelfService(mockDbFactory.Object, Mock.Of<ILogger<ShelfService>>());

			Assert.NotNull(service);
		}
	}
}
