using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
    public class RecipeIngredientMeasurementEntityTypeConfiguration: IEntityTypeConfiguration<RecipeIngredientMeasurement>
    {
        public void Configure(EntityTypeBuilder<RecipeIngredientMeasurement> builder)
        {
            //Table
            builder.ToTable("recipe_ingredient_measurements");

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
                .Property(p => p.Abbreviation)
                .HasColumnName("abbreviation")
                .HasMaxLength(10);

            //PK
            builder.HasKey(p => p.Id);

            //Indexes
            builder
                .HasIndex(p => p.Name)
                .HasDatabaseName("UQ_recipe_ingredient_measurements_name")
                .IsUnique();

            builder
                .HasIndex(p => p.Abbreviation)
                .HasDatabaseName("UQ_recipe_ingredient_measurements_abbreviation")
                .IsUnique();

        }
    }
}
