using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class StorageUnitEntityTypeConfiguration : IEntityTypeConfiguration<StorageUnit>
	{
		public void Configure(EntityTypeBuilder<StorageUnit> builder)
		{
			//Table
			builder.ToTable("storageunits");

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
				.Property(p => p.RoomId)
				.HasColumnName("room_id")
				.IsRequired();

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			builder
				.HasIndex(p => new { p.Name, p.RoomId })
				.HasDatabaseName("UQ_storageunits_name_room_id")
				.IsUnique();

			//Relations
			builder
				.HasMany(p => p.Shelves)
				.WithOne(shelf => shelf.StorageUnit)
				.HasForeignKey(shelf => shelf.StorageUnitId)
				.HasConstraintName("FK_shelves_storageunits")
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			//AutoInclude
			builder.Navigation(p => p.Shelves).AutoInclude();
			builder.Navigation(p => p.Room).AutoInclude();
		}
	}
}
