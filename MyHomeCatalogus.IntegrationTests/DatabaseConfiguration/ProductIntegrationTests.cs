using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class ProductIntegrationTests : BaseIntegrationTest
    {
        //CK_products_picture_mime_type_required
        [Fact]
        public async Task Product_PictureMimeType_Required_If_Picture_Is_Present_Violation()
        {

            byte[] dummyPicture = [1, 2, 3];

            var invalidProduct = new Product
            {
                Name = "ProductWithPictureNoMime",
                ProductTypeId = (await AddTestProductType()).Id,
                StoreId = (await AddTestStore()).Id,
                PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
                Picture = dummyPicture,
                PictureMimeType = null,
                Barcode = null,
                BarcodeMimeType = null
            };

            Context.Products.Add(invalidProduct);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //CK_products_picture_mime_type_required
        [Fact]
        public async Task Product_PictureMimeType_Not_Allowed_If_Picture_Is_Null_Violation()
        {
            var invalidProduct = new Product
            {
                Name = "ProductWithoutPictureWithMime",
                ProductTypeId = (await AddTestProductType()).Id,
                StoreId = (await AddTestStore()).Id,
                PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
                Picture = null,
                PictureMimeType = "foo",
                Barcode = null,
                BarcodeMimeType = null
            };

             Context.Products.Add(invalidProduct);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //CK_products_picture_mime_type_required
        [Fact]
        public async Task Product_PictureMimeType_Allowed_Null_If_Picture_Is_Null_Success()
        {
            var validProduct = new Product
            {
                Name = "ProductWithNoPictureNoMime",
                ProductTypeId = (await AddTestProductType()).Id,
                StoreId = (await AddTestStore()).Id,
                PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
                Picture = null,
                PictureMimeType = null,
                Barcode = null,
                BarcodeMimeType = null
            };

             Context.Products.Add(validProduct);

            var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Equal(1, result);
        }

        //CK_products_barcode_mime_type_required
        [Fact]
        public async Task Product_BarcodeMimeType_Required_If_Barcode_Is_Present_Violation()
        {

            byte[] dummyBarcode = [1, 2, 3];

            var invalidProduct = new Product
            {
                Name = "ProductWithPictureNoMime",
                ProductTypeId = (await AddTestProductType()).Id,
                StoreId = (await AddTestStore()).Id,
                PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
                Picture = dummyBarcode,
                PictureMimeType = null,
                Barcode = null,
                BarcodeMimeType = null
            };

             Context.Products.Add(invalidProduct);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //CK_products_barcode_mime_type_required
        [Fact]
        public async Task Product_BarcodeMimeType_Not_Allowed_If_Barcode_Is_Null_Violation()
        {
            var invalidProduct = new Product
            {
                Name = "ProductWithoutPictureWithMime",
                ProductTypeId = (await AddTestProductType()).Id,
                StoreId = (await AddTestStore()).Id,
                PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
                Picture = null,
                PictureMimeType = null,
                Barcode = null,
                BarcodeMimeType = "foo"
            };

             Context.Products.Add(invalidProduct);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //CK_products_barcode_mime_type_required
        [Fact]
        public async Task Product_BarcodeMimeType_Allowed_Null_If_Picture_Is_Null_Success()
        {
            var validProduct = new Product
            {
                Name = "ProductWithNoPictureNoMime",
                ProductTypeId = (await AddTestProductType()).Id,
                StoreId = (await AddTestStore()).Id,
                PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
                Picture = null,
                PictureMimeType = null,
                Barcode = null,
                BarcodeMimeType = null
            };

             Context.Products.Add(validProduct);

            var result = await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Product_AutoInclude_NaviagationProperties()
        {
            var addedEntity = await AddTestProduct();

            var result = await Context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == addedEntity.Id, TestContext.Current.CancellationToken);

            Assert.NotNull(result);

            //AutoInclude
            Assert.NotNull(result.ProductType);
            Assert.NotNull(result.PurchaseUnit);
            Assert.NotNull(result.Store);

            //Not included
            Assert.NotNull(result.StockItems);
            Assert.Empty(result.StockItems);
        }

    }
}
