using Microsoft.EntityFrameworkCore;
using Moq;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
	public class RoomServiceTests
	{
		[Fact]
		public void Ctor_Should_Throw_When_ContextFactory_Is_Null()
		{
			IDbContextFactory<AppDbContext> nullContext = null!;

			Assert.Throws<ArgumentNullException>(() => new RoomService(nullContext, Mock.Of<ILogger<RoomService>>()));
		}
    [Fact]
    public void Ctor_Should_Throw_When_Logger_Is_Null()
    {
        var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
        ILogger<RoomService> nullLogger = null!;

        Assert.Throws<ArgumentNullException>(() => new RoomService(mockDbFactory.Object, nullLogger));
    }



		[Fact]
		public void Ctor_Should_Initialize_When_All_Parameters_Are_Not_Null()
		{
			var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();

			var service = new RoomService(mockDbFactory.Object, Mock.Of<ILogger<RoomService>>());

			Assert.NotNull(service);
		}
	}
}
