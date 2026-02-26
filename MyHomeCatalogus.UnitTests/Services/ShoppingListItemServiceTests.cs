using Microsoft.EntityFrameworkCore;
using Moq;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Services
{
    public class ShoppingListItemServiceTests
    {
        [Fact]
        public void Ctor_Should_Throw_When_ContextFactory_Is_Null()
        {
            IDbContextFactory<AppDbContext> nullContext = null!;

            Assert.Throws<ArgumentNullException>(() => new ShoppingListItemService(nullContext));
        }

        [Fact]
        public void Ctor_Should_Initialize_When_ContextFactory_Is_Not_Null()
        {
            var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();

            var service = new ShoppingListItemService(mockDbFactory.Object);

            Assert.NotNull(service);
        }
    }
}
