using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class ProductThresholdEntityTypeConfiguration : IEntityTypeConfiguration<ProductThreshold>
	{
		public void Configure(EntityTypeBuilder<ProductThreshold> builder)
		{
			//Table and constraints
			builder.ToTable("product_thresholds", t =>
			{
				t.HasCheckConstraint(
					"CK_productthresholds_purchasequantity_positive",
					"[purchase_quantity] > 0");

				t.HasCheckConstraint(
					"CK_productthresholds_threshold_zero_or_positive",
					"[threshold] >= 0");
			});

			//Columns
			builder
				.Property(p => p.Id)
				.HasColumnName("id")
				.IsRequired();

			builder
				.Property(p => p.ProductId)
				.HasColumnName("product_id")
				.IsRequired();

			builder
				.Property(p => p.Threshold)
				.HasColumnName("threshold")
				.IsRequired();

			builder
				.Property(p => p.PurchaseQuantity)
				.HasColumnName("purchase_quantity")
				.IsRequired();

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			//Maximum one treshold per product
			builder
				.HasIndex(p => p.ProductId)
				.HasDatabaseName("UQ_product_thresholds_product_id")
				.IsUnique();

			//Relations
			builder
				.HasOne(p => p.Product)
				.WithOne()
				.HasForeignKey<ProductThreshold>(p => p.ProductId)
				.HasConstraintName("FK_product_thresholds_products")
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade); //Deleting product will delete the threshold

			//AutoInclude
			builder.Navigation(p => p.Product).AutoInclude();
		}
	}
}
