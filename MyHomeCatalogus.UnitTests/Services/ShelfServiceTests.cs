using Microsoft.EntityFrameworkCore;
using Moq;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Services;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
	public class ShelfServiceTests
	{
		[Fact]
		public void Ctor_Should_Throw_When_ContextFactory_Is_Null()
		{
			IDbContextFactory<AppDbContext> nullContext = null!;

			Assert.Throws<ArgumentNullException>(() => new ShelfService(nullContext));
		}

		[Fact]
		public void Ctor_Should_Initialize_When_ContextFactory_Is_Not_Null()
		{
			var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();

			var service = new ShelfService(mockDbFactory.Object);

			Assert.NotNull(service);
		}
	}
}
