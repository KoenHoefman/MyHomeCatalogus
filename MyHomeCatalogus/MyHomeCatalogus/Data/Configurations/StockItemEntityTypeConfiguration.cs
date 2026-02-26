using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
    public class StockItemEntityTypeConfiguration : IEntityTypeConfiguration<StockItem>
    {
        public void Configure(EntityTypeBuilder<StockItem> builder)
        {
            //Table
            builder.ToTable("stockitems", t =>
                t.HasCheckConstraint(
                    "CK_stockitems_quantity_zero_or_positive",
                    "[quantity] >= 0"));

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
                .Property(p => p.ShelfId)
                .HasColumnName("shelf_id")
                .IsRequired();

            builder
                .Property(p => p.StockUnitId)
                .HasColumnName("stockunit_id")
                .IsRequired();

            builder.Property(p => p.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            //PK
            builder.HasKey(p => p.Id);

            //Indexes
            builder.HasIndex(p => p.ProductId)
                .HasDatabaseName("IX_products_stockitems");

            //It's possible to have same product stocked in different places, but not in the same place
            builder.HasIndex(p => new { p.ProductId, p.ShelfId })
                .HasDatabaseName("UQ_stockitems_productid_shelfid")
                .IsUnique();

            //Relations
            builder
                .HasOne(p => p.Product)
                .WithMany(product => product.StockItems)
                .HasForeignKey(p => p.ProductId)
                .HasConstraintName("FK_products_stockitems")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(p => p.Shelf)
                .WithMany(shelf => shelf.StockItems)
                .HasForeignKey(p => p.ShelfId)
                .HasConstraintName("FK_shelves_stockitems")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            builder
                .HasOne(p => p.StockUnit)
                .WithMany()
                .HasForeignKey(p => p.StockUnitId)
                .HasConstraintName("FK_stockunits_stockitems")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            //AutoInclude
            builder.Navigation(p => p.Product).AutoInclude();
            builder.Navigation(p => p.StockUnit).AutoInclude();
            builder.Navigation(p => p.Shelf).AutoInclude();

        }
    }
}
