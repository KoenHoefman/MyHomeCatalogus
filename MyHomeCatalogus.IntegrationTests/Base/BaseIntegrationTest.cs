using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Data.Interceptors;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.Base
{

	// Collection for sequential testing since SQLite in-memory must be handled carefully
	[CollectionDefinition("Sequential", DisableParallelization = true)]
	public class SequentialTestCollection { }


	[Collection("Sequential")]
	public abstract class BaseIntegrationTest : IDisposable
	{
		protected readonly DbContextOptions<AppDbContext> Options;
		protected readonly AppDbContext Context;

		protected BaseIntegrationTest()
		{
			var dbName = Guid.NewGuid().ToString();

			// Use :memory: SQLite for transactional constraint testing
			Options = new DbContextOptionsBuilder<AppDbContext>()
				.UseSqlite($"Data Source={dbName};Mode=Memory;Cache=Shared")
				//.UseSqlite("Filename=:memory:")
				.AddInterceptors(new StockItemAuditInterceptor())
				.Options;

			Context = new AppDbContext(Options);
			Context.Database.OpenConnection();
			Context.Database.EnsureCreated();
		}

		#region Seed methods

		protected async Task<Product> AddTestProduct()
		{

			var addedEntity = Context.Products.Add(new Product
			{
				Name = "Milk",
				ProductTypeId = (await AddTestProductType()).Id,
				StoreId = (await AddTestStore()).Id,
				PurchaseUnitId = (await AddTestPurchaseUnit()).Id,
				Picture = [0x01, 0x02, 0x03, 0x04],
				PictureMimeType = "image/jpeg",
				Barcode = [0x05, 0x06, 0x07, 0x08],
				BarcodeMimeType = "image/jpeg"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<PurchaseUnit> AddTestPurchaseUnit()
		{
			var addedEntity = Context.PurchaseUnits.Add(new PurchaseUnit
			{
				Name = "Pack"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<ProductType> AddTestProductType()
		{
			var addedEntity = Context.ProductTypes.Add(new ProductType
			{
				Name = "Food",
				Description = "Food related products"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<ProductThreshold> AddTestProductThreshold()
		{
			var addedEntity = Context.ProductThresholds.Add(new ProductThreshold
			{
				ProductId = (await AddTestProduct()).Id,
				PurchaseQuantity = 5,
				Threshold = 2
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<Store> AddTestStore()
		{
			var addedEntity = Context.Stores.Add(new Store
			{
				Name = "Store A",
				Description = "The cheapest store"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<Room> AddTestRoom()
		{
			var addedEntity = Context.Rooms.Add(new Room
			{
				Name = "Kitchen",
				Description = "The kitchen"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<StorageUnit> AddTestStorageUnit()
		{
			var addedEntity = Context.StorageUnits.Add(new StorageUnit()
			{
				Name = "Fridge",
				Description = "The fridge",
				RoomId = (await AddTestRoom()).Id
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<Shelf> AddTestShelf()
		{
			var addedEntity = Context.Shelves.Add(new Shelf()
			{
				Name = "Top shelf",
				Description = "The upper shelf",
				StorageUnitId = (await AddTestStorageUnit()).Id
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<StockUnit> AddTestStockUnit()
		{
			var addedEntity = Context.StockUnits.Add(new StockUnit
			{
				Name = "Bottle"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<ShoppingList> AddTestShoppingList()
		{
			var addedEntity = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = (await AddTestStore()).Id,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<ShoppingListItem> AddTestShoppingListItem()
		{
			var testProduct = await AddTestProduct();

			//AddTestShoppingList uses same AddTestStore which gives UQ index violation
			var addedList = Context.ShoppingLists.Add(new ShoppingList
			{
				StoreId = testProduct.StoreId,
				IsCompleted = false,
				DateCreated = DateOnly.FromDateTime(DateTime.Today)
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			var addedEntity = Context.ShoppingListItems.Add(new ShoppingListItem
			{
				ShoppingListId = addedList.Entity.Id,
				ProductId = testProduct.Id,
				Quantity = 5
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<RecipeIngredientMeasurement> AddTestRecipeIngredientMeasurement()
		{
			var addedEntity = Context.RecipeIngredientMeasurements.Add(new RecipeIngredientMeasurement
			{
				Name = "Teaspoon",
				Abbreviation = "tsp"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<Recipe> AddTestRecipe()
		{
			var addedEntity = Context.Recipes.Add(new Recipe
			{
				Name = "Spaghetti"
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<RecipePreparationStep> AddTestRecipePreparationSteps()
		{

			var addedEntity = Context.RecipePreparationSteps.Add(new RecipePreparationStep()
			{
				RecipeId = (await AddTestRecipe()).Id,
				StepNumber = 1,
				Instructions = "Do something."
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<RecipeIngredient> AddTestRecipeIngredient()
		{
			var addedEntity = Context.RecipeIngredients.Add(new RecipeIngredient()
			{
				ProductId = (await AddTestProduct()).Id,
				Quantity = 1,
				RecipeIngredientMeasurementId = (await AddTestRecipeIngredientMeasurement()).Id,
				RecipePreparationStepId = (await AddTestRecipePreparationSteps()).Id
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		protected async Task<StockItem> AddTestStockItem()
		{
			var addedEntity = Context.StockItems.Add(new StockItem()
			{
				ProductId = (await AddTestProduct()).Id,
				ShelfId = (await AddTestShelf()).Id,
				StockUnitId = (await AddTestStockUnit()).Id,
				Quantity = 50
			});

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
			Context.ChangeTracker.Clear();

			return addedEntity.Entity;
		}

		#endregion


		public void Dispose()
		{
			Context.Database.CloseConnection();
			Context.Dispose();
		}
	}
}
