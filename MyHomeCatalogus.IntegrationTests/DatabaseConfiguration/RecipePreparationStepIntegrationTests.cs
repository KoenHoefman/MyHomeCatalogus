using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.IntegrationTests.Base;
using MyHomeCatalogus.Shared.Domain;
using Xunit;

namespace MyHomeCatalogus.IntegrationTests.DatabaseConfiguration
{
    public class RecipePreparationStepIntegrationTests : BaseIntegrationTest
    {
        //UQ_recipe_preparationsteps_recipe_id_step_nr
        [Fact]
        public async Task Step_Cannot_Have_Same_Number_In_Same_Recipe()
        {
            var testPreparationStep = await AddTestRecipePreparationSteps();

            var duplicateStep = new RecipePreparationStep
            {
                RecipeId = testPreparationStep.RecipeId,
                StepNumber = testPreparationStep.StepNumber,
                Instructions = "Duplicate Step"
            };

            Context.RecipePreparationSteps.Add(duplicateStep);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }

        //CK_recipe_preparationsteps_step_nr_positive
        [Fact]
        public async Task Step_Nr_Must_Be_Positive()
        {
            var testRecipe = await AddTestRecipe();

            var invalidStep = new RecipePreparationStep
            {
                RecipeId = testRecipe.Id,
                StepNumber = 0, // Invalid value
                Instructions = "Invalid Step"
            };

            Context.RecipePreparationSteps.Add(invalidStep);

            await Assert.ThrowsAsync<DbUpdateException>(async () => await Context.SaveChangesAsync(TestContext.Current.CancellationToken));
        }



    }
}
