using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class StockUnitEntityTypeConfiguration : IEntityTypeConfiguration<StockUnit>
	{
		public void Configure(EntityTypeBuilder<StockUnit> builder)
		{
			//Table
			builder.ToTable("stockunits");

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
				.HasDatabaseName("UQ_stockunits_name")
				.IsUnique();
		}
	}
}
