using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class RecipeIngredientEntityTypeConfiguration : IEntityTypeConfiguration<RecipeIngredient>
	{
		public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
		{
			//Table
			builder.ToTable("recipe_ingredients", t =>
				t.HasCheckConstraint(
				"CK_recipe_ingredients_quantity_positive",
				"[quantity] > 0"));

			//Columns
			builder.Property(p => p.Id)
				.HasColumnName("id")
				.IsRequired();

			builder.Property(p => p.RecipePreparationStepId)
				.HasColumnName("recipe_Preparationstep_Id")
				.IsRequired();

			builder.Property(p => p.ProductId)
				.HasColumnName("fooproduct_Id")
				.IsRequired();

			builder.Property(p => p.Quantity)
				.HasColumnName("quantity")
				.IsRequired();

			builder.Property(p => p.RecipeIngredientMeasurementId)
				.HasColumnName("measurement_id")
				.IsRequired();

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			builder
				.HasIndex(p => new { p.RecipePreparationStepId, p.ProductId })
				.IsUnique()
				.HasDatabaseName("UQ_recipe_ingredients_recipe_preparationstep_id_product_id");

			//Relations
			builder
				.HasOne(p => p.RecipePreparationStep)
				.WithMany(recipePreparationStep => recipePreparationStep.Ingredients)
				.HasForeignKey(p => p.RecipePreparationStepId)
				.HasConstraintName("FK_recipe_preparationsteps_recipe_ingredients")
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			builder
				.HasOne(p => p.Product)
				.WithMany()
				.HasForeignKey(p => p.ProductId)
				.HasConstraintName("FK_products_recipe_ingredients")
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(p => p.RecipeIngredientMeasurement)
				.WithMany()
				.HasForeignKey(p => p.RecipeIngredientMeasurementId)
				.HasConstraintName("FK_recipe_ingredient_measurements_recipe_ingredients")
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			//AutoInclude
			builder.Navigation(p => p.RecipeIngredientMeasurement).AutoInclude();
		}
	}
}
