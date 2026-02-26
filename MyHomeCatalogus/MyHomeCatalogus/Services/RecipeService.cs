using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public RecipeService(IDbContextFactory<AppDbContext> contextFactory)
        {
            ArgumentNullException.ThrowIfNull(contextFactory);

            _contextFactory = contextFactory;
        }

        public Task<IEnumerable<Recipe>> GetAllRecipes()
        {
            throw new NotImplementedException();
        }

        public Task<Recipe> GetRecipeById(int recipeId)
        {
            throw new NotImplementedException();
        }

        public Task<Recipe> AddRecipe(Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public Task<Recipe> UpdateRecipe(Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRecipe(int recipeId)
        {
            throw new NotImplementedException();
        }
    }
}