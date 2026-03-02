using Microsoft.EntityFrameworkCore;
using Moq;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Services;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
	public class ProductServiceTests
	{
		[Fact]
		public void Ctor_Should_Throw_When_ContextFactory_Is_Null()
		{
			IDbContextFactory<AppDbContext> nullContext = null!;

			Assert.Throws<ArgumentNullException>(() => new ProductService(nullContext));
		}

		[Fact]
		public void Ctor_Should_Initialize_When_ContextFactory_Is_Not_Null()
		{
			var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();

			var service = new ProductService(mockDbFactory.Object);

			Assert.NotNull(service);
		}
	}
}
