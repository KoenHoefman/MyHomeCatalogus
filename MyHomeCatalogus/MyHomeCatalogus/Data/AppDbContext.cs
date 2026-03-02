using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Shared.Domain;
using System.Reflection;

namespace MyHomeCatalogus.Data
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductType> ProductTypes { get; set; }
		public DbSet<ProductThreshold> ProductThresholds { get; set; }
		public DbSet<StockItem> StockItems { get; set; }
		public DbSet<PurchaseUnit> PurchaseUnits { get; set; }
		public DbSet<StockUnit> StockUnits { get; set; }
		public DbSet<Store> Stores { get; set; }
		public DbSet<ShoppingList> ShoppingLists { get; set; }
		public DbSet<ShoppingListItem> ShoppingListItems { get; set; }


		public DbSet<Room> Rooms { get; set; }
		public DbSet<StorageUnit> StorageUnits { get; set; }
		public DbSet<Shelf> Shelves { get; set; }


		public DbSet<Recipe> Recipes { get; set; }
		public DbSet<RecipePreparationStep> RecipePreparationSteps { get; set; }
		public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
		public DbSet<RecipeIngredientMeasurement> RecipeIngredientMeasurements { get; set; }

		public DbSet<StockItemAudit> StockItemAudits { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//Configurations
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			// CONDITIONAL FIX for SQLite Default Value
			// ----------------------------------------
			// For integration testing I use de SqlLite InMemory provider
			// HasDefaultValueSql("CONVERT(date, GETDATE())") (ShoppingListEntityTypeConfiguration)
			// gives an error due to not being a constant.
			// Overriding it here to use the working SqlLite syntax solves that issue.
			if (Database.IsSqlite())
			{
				// This targets the specific property that is causing the schema creation failure in SQLite
				modelBuilder.Entity<ShoppingList>()
					.Property(p => p.DateCreated)
					.HasDefaultValueSql("CURRENT_DATE");

				//Same for audit_date
				modelBuilder.Entity<StockItemAudit>()
					.Property(p => p.AuditDate)
					.HasDefaultValueSql("CURRENT_TIMESTAMP");
			}

		}
	}
}
