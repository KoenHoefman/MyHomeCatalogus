using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			//Table
			builder.ToTable("products", t =>
			{
				//Ensure mime type is provided when picture or barcode is present
				t.HasCheckConstraint(
					"CK_products_picture_mime_type_required",
					"(([picture] IS NULL AND [picture_mime_type] IS NULL) OR ([picture] IS NOT NULL AND [picture_mime_type] IS NOT NULL))");

				t.HasCheckConstraint(
					"CK_products_barcode_mime_type_required",
					"(([barcode] IS NULL AND [barcode_mime_type] IS NULL) OR ([barcode] IS NOT NULL AND [barcode_mime_type] IS NOT NULL))");
			});

			//Columns
			builder
				.Property(p => p.Id)
				.HasColumnName("id")
				.IsRequired();

			builder
				.Property(p => p.Name)
				.HasColumnName("name")
				.HasMaxLength(50)
				.IsRequired();

			builder
				.Property(p => p.Description)
				.HasColumnName("description")
				.HasMaxLength(500);

			builder
				.Property(p => p.ProductTypeId)
				.HasColumnName("product_type_id")
				.IsRequired();

			builder
				.Property(p => p.StoreId)
				.HasColumnName("store_id")
				.IsRequired();

			builder
				.Property(p => p.PurchaseUnitId)
				.HasColumnName("purchaseunit_id")
				.IsRequired();

			builder
				.Property(p => p.Picture)
				.HasColumnName("picture");

			builder
				.Property(p => p.PictureMimeType)
				.HasColumnName("picture_mime_type");

			builder
				.Property(p => p.Barcode)
				.HasColumnName("barcode");

			builder
				.Property(p => p.BarcodeMimeType)
				.HasColumnName("barcode_mime_type");

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			builder
				.HasIndex(p => p.ProductTypeId)
				.HasDatabaseName("IX_products_producttypeid");

			builder
				.HasIndex(p => new { p.Name, p.StoreId })
				.HasDatabaseName("UQ_products_name_store_id")
				.IsUnique();

			//Relations
			builder
				.HasOne(p => p.ProductType) // Navigation property on the dependent/many side
				.WithMany()                     // The principal/one side (ProductType) has no collection navigation property
				.HasForeignKey(p => p.ProductTypeId) // The foreign key property on the dependent side
				.HasConstraintName("FK_product_types_products")
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict); // Best practice for FKs: prevent accidental deletion of a Type

			builder
				.HasOne(p => p.Store)
				.WithMany()
				.HasForeignKey(p => p.StoreId)
				.HasConstraintName("FK_stores_products")
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(p => p.PurchaseUnit)
				.WithMany()
				.HasForeignKey(p => p.PurchaseUnitId)
				.HasConstraintName("FK_purchaseunits_products")
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			//AutoInclude
			builder.Navigation(p => p.ProductType).AutoInclude();
			builder.Navigation(p => p.PurchaseUnit).AutoInclude();
			builder.Navigation(p => p.Store).AutoInclude();
		}
	}
}
