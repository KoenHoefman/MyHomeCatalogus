using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
    public class StockItemAuditEntityTypeConfiguration : IEntityTypeConfiguration<StockItemAudit>
    {
        public void Configure(EntityTypeBuilder<StockItemAudit> builder)
        {
            //Table
            builder.ToTable("stockitem_audits", t =>
            {
                //Ensure mime type is provided when picture or barcode is present
                t.HasCheckConstraint(
                    "CK_stockitem_audits_quantity_old_zero_or_positive",
                    "[quantity_old] >= 0");

                t.HasCheckConstraint(
                    "CK_stockitem_audits_quantity_new_zero_or_positive",
                    "[quantity_new] >= 0");
            });

            //Columns
            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .IsRequired();

            builder
                .Property(p => p.StockItemId)
                .HasColumnName("stockitem_id")
                .IsRequired();

            builder
                .Property(p => p.AuditDate)
                .HasColumnName("audit_date")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder
                .Property(p => p.OldQuantity)
                .HasColumnName("quantity_old")
                .IsRequired();

            builder
                .Property(p => p.NewQuantity)
                .HasColumnName("quantity_new")
                .IsRequired();

            //PK
            builder.HasKey(p => p.Id);

            //Indexes
            builder
                .HasIndex(p => p.StockItemId)
                .HasDatabaseName("IX_stockitem_audits_stockitem_id");

            //Relations
            builder
                .HasOne(p => p.StockItem)
                .WithMany()
                .HasForeignKey(p => p.StockItemId)
                .HasConstraintName("FK_stockitem_audits_stockitems")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
