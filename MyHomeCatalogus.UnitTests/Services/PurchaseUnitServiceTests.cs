using Microsoft.EntityFrameworkCore;
using Moq;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
	public class PurchaseUnitServiceTests
	{
		[Fact]
		public void Ctor_Should_Throw_When_ContextFactory_Is_Null()
		{
			IDbContextFactory<AppDbContext> nullContext = null!;

			Assert.Throws<ArgumentNullException>(() => new PurchaseUnitService(nullContext, Mock.Of<ILogger<PurchaseUnitService>>()));
		}
    [Fact]
    public void Ctor_Should_Throw_When_Logger_Is_Null()
    {
        var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
        ILogger<PurchaseUnitService> nullLogger = null!;

        Assert.Throws<ArgumentNullException>(() => new PurchaseUnitService(mockDbFactory.Object, nullLogger));
    }



		[Fact]
		public void Ctor_Should_Initialize_When_All_Parameters_Are_Not_Null()
		{
			var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();

			var service = new PurchaseUnitService(mockDbFactory.Object, Mock.Of<ILogger<PurchaseUnitService>>());

			Assert.NotNull(service);
		}
	}
}
