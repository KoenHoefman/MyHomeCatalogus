using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
	public class RecipeIntegrationTests : BaseIntegrationTest
	{
		//UQ_recipes_name
		[Fact]
		public async Task? Recipes_Must_Have_A_Uinque_Name()
		{
			var testRecipe = await AddTestRecipe();

			var duplicateRecipe = new Recipe()
			{
				Name = testRecipe.Name
			};

			Context.Recipes.Add(duplicateRecipe);

			await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
		}

		//FK_recipe_preparationsteps_recipes
		[Fact]
		public async Task Recipe_With_Linked_RecipePreparationStep_Will_Delete_PreparationSteps()
		{
			var testPreparationStep = await AddTestRecipePreparationSteps();

			var recipeToDelete = await Context.Recipes.FindAsync([testPreparationStep.RecipeId], TestContext.Current.CancellationToken);

			if (recipeToDelete != null)
			{
				Context.Recipes.Remove(recipeToDelete);
			}

			await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

			var deletedRecipe = await Context.Recipes.FindAsync([testPreparationStep.RecipeId], TestContext.Current.CancellationToken);
			var deletedStep = await Context.RecipePreparationSteps.FindAsync([testPreparationStep.Id], TestContext.Current.CancellationToken);

			Assert.Null(deletedRecipe);
			Assert.Null(deletedStep);
		}

	}
}
