using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class PurchaseUnitEntityTypeConfiguration : IEntityTypeConfiguration<PurchaseUnit>
	{
		public void Configure(EntityTypeBuilder<PurchaseUnit> builder)
		{
			//Table
			builder.ToTable("purchaseunits");

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

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			builder
				.HasIndex(p => p.Name)
				.HasDatabaseName("UQ_purchaseunits_name")
				.IsUnique();
		}
	}
}
