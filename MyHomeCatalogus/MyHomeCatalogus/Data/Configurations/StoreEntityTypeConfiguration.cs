using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
    public class StoreEntityTypeConfiguration:IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            //Table
            builder.ToTable("stores");

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
                .HasDatabaseName("UQ_stores_name")
                .IsUnique();

        }
    }
}
