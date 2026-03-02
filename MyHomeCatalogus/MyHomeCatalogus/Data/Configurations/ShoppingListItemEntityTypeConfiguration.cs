using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class ShoppingListItemEntityTypeConfiguration : IEntityTypeConfiguration<ShoppingListItem>
	{
		public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
		{
			//Table
			builder.ToTable("shoppinglist_items", t =>
				t.HasCheckConstraint(
				"CK_shoppinglist_items_quantity_positive",
				"[quantity] > 0"));

			//Columns
			builder
				.Property(p => p.Id)
				.HasColumnName("id")
				.IsRequired();

			builder
				.Property(p => p.ShoppingListId)
				.HasColumnName("shoppinglist_id")
				.IsRequired();

			builder
				.Property(p => p.ProductId)
				.HasColumnName("product_id")
				.IsRequired();

			builder
				.Property(p => p.Quantity)
				.HasColumnName("quantity")
				.IsRequired();

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			builder
				.HasIndex(p => new { p.ShoppingListId, p.ProductId })
				.IsUnique()
				.HasDatabaseName("UQ_shoppinglist_items_shoppinglist_id_product_id");

			//Relations
			builder
				.HasOne(p => p.Product)
				.WithMany()
				.HasForeignKey(p => p.ProductId)
				.HasConstraintName("FK_products_shoppinglist_items")
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
}
