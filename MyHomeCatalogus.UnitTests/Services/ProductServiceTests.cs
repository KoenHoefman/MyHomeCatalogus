using Microsoft.EntityFrameworkCore;
using Moq;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
	public class ProductServiceTests
	{
		[Fact]
		public void Ctor_Should_Throw_When_ContextFactory_Is_Null()
		{
			IDbContextFactory<AppDbContext> nullContext = null!;

			Assert.Throws<ArgumentNullException>(() => new ProductService(nullContext, Mock.Of<ILogger<ProductService>>()));
		}
    [Fact]
    public void Ctor_Should_Throw_When_Logger_Is_Null()
    {
        var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
        ILogger<ProductService> nullLogger = null!;

        Assert.Throws<ArgumentNullException>(() => new ProductService(mockDbFactory.Object, nullLogger));
    }



		[Fact]
		public void Ctor_Should_Initialize_When_All_Parameters_Are_Not_Null()
		{
			var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();

			var service = new ProductService(mockDbFactory.Object, Mock.Of<ILogger<ProductService>>());

			Assert.NotNull(service);
		}
	}
}
