using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
	public class RecipeIngredientMeasurementIntegrationTests : BaseIntegrationTest
	{
		//UQ_recipe_ingredient_measurements_name
		[Fact]
		public async Task Measurement_Name_Cannot_Be_Duplicated()
		{
			var testMeasurement = await AddTestRecipeIngredientMeasurement();

			var duplicateMeasurement = new RecipeIngredientMeasurement
			{
				Name = testMeasurement.Name,
				Abbreviation = "tsp_dupl"
			};
			Context.RecipeIngredientMeasurements.Add(duplicateMeasurement);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}


		//UQ_recipe_ingredient_measurements_abbreviation
		[Fact]
		public async Task Measurement_Abbreviation_Cannot_Be_Duplicated()
		{
			var testMeasurement = await AddTestRecipeIngredientMeasurement();

			var duplicateMeasurement = new RecipeIngredientMeasurement
			{
				Name = "Teaspoon-Dup",
				Abbreviation = testMeasurement.Abbreviation
			};

			Context.RecipeIngredientMeasurements.Add(duplicateMeasurement);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}


		//FK_recipe_ingredient_measurements_recipe_ingredients
		[Fact]
		public async Task RecipeIngredientMeasurements_With_Linked_Ingredient_Cannot_Be_Deleted()
		{
			var testIngredient = await AddTestRecipeIngredient();

			var measurementUnitToDelete = await Context.RecipeIngredientMeasurements.FindAsync([testIngredient.RecipeIngredientMeasurementId], TestContext.Current.CancellationToken);

			if (measurementUnitToDelete != null)
			{
				Context.RecipeIngredientMeasurements.Remove(measurementUnitToDelete);
			}

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}


	}
}
