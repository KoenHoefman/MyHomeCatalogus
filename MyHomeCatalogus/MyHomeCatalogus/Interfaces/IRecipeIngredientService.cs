using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Interfaces;

public interface IRecipeIngredientService
{
    Task<RecipeIngredient> GetRecipeIngredientById(int recipeIngredientId);
    Task<RecipeIngredient> AddRecipeIngredient(RecipeIngredient recipeIngredientId);
    Task<RecipeIngredient> UpdateRecipeIngredient(RecipeIngredient recipeIngredient);
    Task DeleteRecipeIngredient(int recipeIngredientId);
}