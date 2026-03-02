using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class RoomEntityTypeConfiguration : IEntityTypeConfiguration<Room>
	{
		public void Configure(EntityTypeBuilder<Room> builder)
		{
			//Table
			builder.ToTable("rooms");

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

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			builder
				.HasIndex(p => p.Name)
				.HasDatabaseName("UQ_rooms_name")
				.IsUnique();

			//Relations
			builder
				.HasMany(p => p.StorageUnits)
				.WithOne(storageUnit => storageUnit.Room)
				.HasForeignKey(storageUnit => storageUnit.RoomId)
				.HasConstraintName("FK_storageunits_rooms")
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			//AutoInclude
			builder.Navigation(p => p.StorageUnits).AutoInclude();
		}
	}
}
