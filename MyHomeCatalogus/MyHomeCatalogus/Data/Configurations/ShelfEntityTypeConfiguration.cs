using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
    public class ShelfEntityTypeConfiguration : IEntityTypeConfiguration<Shelf>
    {
        public void Configure(EntityTypeBuilder<Shelf> builder)
        {
            //Table
            builder.ToTable("shelves");

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
                .Property(p => p.StorageUnitId)
                .HasColumnName("storageunit_id")
                .IsRequired();

            //PK
            builder.HasKey(p => p.Id);

            //Indexes
            builder
                .HasIndex(p => new { p.Name, p.StorageUnitId })
                .HasDatabaseName("UQ_shelves_name_storageunit_id")
                .IsUnique();

            //AutoInclude
            builder.Navigation(p => p.StorageUnit).AutoInclude();

        }
    }
}
