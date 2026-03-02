using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Data.Configurations
{
	public class RecipePreparationStepEntityTypeConfiguration : IEntityTypeConfiguration<RecipePreparationStep>
	{
		public void Configure(EntityTypeBuilder<RecipePreparationStep> builder)
		{
			//Table
			builder.ToTable("recipe_preparationsteps", t =>
				t.HasCheckConstraint(
					"CK_recipe_preparationsteps_step_nr_positive",
					"[step_nr] > 0"));

			//Columns
			builder
				.Property(p => p.Id)
				.HasColumnName("id")
				.IsRequired();

			builder
				.Property(p => p.RecipeId)
				.HasColumnName("recipe_id")
				.IsRequired();

			builder
				.Property(p => p.StepNumber)
				.HasColumnName("step_nr")
				.IsRequired();

			builder
				.Property(p => p.Instructions)
				.HasColumnName("instructions")
				.IsRequired();

			//PK
			builder.HasKey(p => p.Id);

			//Indexes
			builder
				.HasIndex(p => new { p.RecipeId, p.StepNumber })
				.IsUnique()
				.HasDatabaseName("UQ_recipe_preparationsteps_recipe_id_step_nr");

			//AutoInclude
			builder.Navigation(p => p.Ingredients).AutoInclude();
		}
	}
}
