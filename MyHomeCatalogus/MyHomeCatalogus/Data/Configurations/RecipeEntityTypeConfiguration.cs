using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class RecipeEntityTypeConfiguration : IEntityTypeConfiguration<Recipe>
	{
		public void Configure(EntityTypeBuilder<Recipe> builder)
		{
			//Table
			builder.ToTable("recipes");

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
			builder.HasIndex(p => p.Name)
				.HasDatabaseName("UQ_recipes_name")
				.IsUnique();

			//Relations
			builder
				.HasMany(recipe => recipe.PreparationSteps)
				.WithOne(recipePreparationStep => recipePreparationStep.Recipe)
				.HasForeignKey(recipePreparationStep => recipePreparationStep.RecipeId)
				.HasConstraintName("FK_recipe_preparationsteps_recipes")
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);

			//AutoInclude
			builder.Navigation(p => p.PreparationSteps).AutoInclude();

		}
	}
}
