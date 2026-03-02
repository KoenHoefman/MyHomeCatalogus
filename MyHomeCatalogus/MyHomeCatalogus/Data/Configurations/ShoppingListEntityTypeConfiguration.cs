using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class ShoppingListEntityTypeConfiguration : IEntityTypeConfiguration<ShoppingList>
	{
		public void Configure(EntityTypeBuilder<ShoppingList> builder)
		{
			//Table
			builder.ToTable("shoppinglists");

			//Columns
			builder
				.Property(p => p.Id)
				.HasColumnName("id")
				.IsRequired();

			builder
				.Property(p => p.StoreId)
				.HasColumnName("store_id")
				.IsRequired();

			builder
				.Property(p => p.DateCreated)
				.HasColumnName("date_created")
				.IsRequired()
				.HasDefaultValueSql("CONVERT(date, GETDATE())");

			builder
				.Property(p => p.IsCompleted)
				.HasColumnName("is_completed")
				.IsRequired();

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			//Unique filtered index to ensure only one list is open per store.
			builder
				.HasIndex(p => p.StoreId)
				.HasDatabaseName("UQ_shoppinglists_store_id_one_active_list")
				.IsUnique()
				.HasFilter("[is_completed] = 0");

			//Relations
			builder
				.HasMany(p => p.ShoppingListItems)
				.WithOne(shoppingListItem => shoppingListItem.ShoppingList)
				.HasForeignKey(shoppingListItem => shoppingListItem.ShoppingListId)
				.HasConstraintName("FK_shoppinglist_items_shoppinglists")
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(p => p.Store)
				.WithMany()
				.HasForeignKey(p => p.StoreId)
				.HasConstraintName("FK_stores_shoppinglists")
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
