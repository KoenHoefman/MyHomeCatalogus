using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
	public class RecipeIngredientIntegrationTests : BaseIntegrationTest
	{
		//CK_recipe_ingredients_quantity_positive
		[Fact]
		public async Task RecipeIngredient_Quantity_Must_Be_Positive()
		{

			var invalidIngredient = new RecipeIngredient
			{
				RecipePreparationStepId = 1,
				ProductId = 1,
				Quantity = 0, // Invalid value
				RecipeIngredientMeasurementId = (await AddTestRecipeIngredientMeasurement()).Id
			};

			Context.RecipeIngredients.Add(invalidIngredient);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

		//UQ_recipe_ingredients_recipe_preparationstep_id_product_id
		[Fact]
		public async Task Ingredient_Cannot_Have_Same_Product_In_Same_PreperationStep()
		{
			var testIngredient = await AddTestRecipeIngredient();

			var duplicateIngredient = new RecipeIngredient()
			{
				ProductId = testIngredient.ProductId,
				RecipePreparationStepId = testIngredient.RecipePreparationStepId,
				RecipeIngredientMeasurementId = testIngredient.RecipeIngredientMeasurementId,
				Quantity = 1
			};

			Context.RecipeIngredients.Add(duplicateIngredient);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}


		//FK_recipe_preparationsteps_recipe_ingredients
		[Fact]
		public async Task RecipePreparationStep_With_Linked_Ingredients_Will_Delete_Ingredients()
		{
			var testIngredient = await AddTestRecipeIngredient();

			var stepToDelete = await Context.RecipePreparationSteps.FindAsync([testIngredient.RecipePreparationStepId], TestContext.Current.CancellationToken);

			if (stepToDelete != null)
			{
				Context.RecipePreparationSteps.Remove(stepToDelete);
			}

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			//Check if step and ingredient are deleted

			var deletedStep = await Context.RecipePreparationSteps.FindAsync([testIngredient.RecipePreparationStepId], TestContext.Current.CancellationToken);
			var deletedIngredient = await Context.RecipeIngredients.FindAsync([testIngredient.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedStep);
			Assert.Null(deletedIngredient);
		}


		//FK_products_recipe_ingredients
		[Fact]
		public async Task Deleting_RecipeIngredient_Will_Not_Delete_Linked_Product()
		{
			var testIngredient = await AddTestRecipeIngredient();

			var ingredientToDelete = await Context.RecipeIngredients.FindAsync([testIngredient.Id], TestContext.Current.CancellationToken);

			if (ingredientToDelete != null)
			{
				Context.RecipeIngredients.Remove(ingredientToDelete);
			}

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			var linkedProduct = await Context.Products.FindAsync([testIngredient.Id], TestContext.Current.CancellationToken);

			Assert.NotNull(linkedProduct);
		}




	}
}
