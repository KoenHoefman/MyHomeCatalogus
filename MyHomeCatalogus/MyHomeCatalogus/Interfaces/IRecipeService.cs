using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Interfaces;

public interface IRecipeService
{
    Task<IEnumerable<Recipe>> GetAllRecipes();
    Task<Recipe> GetRecipeById(int recipeId);
    Task<Recipe> AddRecipe(Recipe recipe);
    Task<Recipe> UpdateRecipe(Recipe recipe);
    Task DeleteRecipe(int recipeId);
}