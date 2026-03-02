using Microsoft.EntityFrameworkCore;
using MyHomeCatalogus.Data;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;

namespace MyHomeCatalogus.Services
{
	public class RecipeIngredientService : IRecipeIngredientService
	{
		private readonly IDbContextFactory<AppDbContext> _contextFactory;

		public RecipeIngredientService(IDbContextFactory<AppDbContext> contextFactory)
		{
			ArgumentNullException.ThrowIfNull(contextFactory);

			_contextFactory = contextFactory;
		}

		public async Task<RecipeIngredient> GetRecipeIngredientById(int recipeIngredientId)
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			return await context.RecipeIngredients.FindAsync(recipeIngredientId)
				   ?? throw new KeyNotFoundException($"RecipeIngredient with Id {recipeIngredientId} not found");
		}

		public async Task<RecipeIngredient> AddRecipeIngredient(RecipeIngredient recipeIngredient)
		{
			ArgumentNullException.ThrowIfNull(recipeIngredient);

			await using var context = await _contextFactory.CreateDbContextAsync();

			var addedEntity = context.RecipeIngredients.Add(recipeIngredient);

			await context.SaveChangesAsync();

			return addedEntity.Entity;
		}

		public async Task<RecipeIngredient> UpdateRecipeIngredient(RecipeIngredient recipeIngredient)
		{
			ArgumentNullException.ThrowIfNull(recipeIngredient);

			await using var context = await _contextFactory.CreateDbContextAsync();

			var foundEntity = await context.RecipeIngredients.FindAsync(recipeIngredient.Id);

			if (foundEntity is not null)
			{
				context.Entry(foundEntity).CurrentValues.SetValues(recipeIngredient);

				await context.SaveChangesAsync();
			}

			return foundEntity ?? throw new KeyNotFoundException($"RecipeIngredient with Id {recipeIngredient.Id} not found");
		}

		public async Task DeleteRecipeIngredient(int recipeIngredientId)
		{
			await using var context = await _contextFactory.CreateDbContextAsync();

			var foundEntity = await context.RecipeIngredients.FirstOrDefaultAsync(p => p.Id == recipeIngredientId);

			if (foundEntity == null)
			{
				return;
			}

			context.RecipeIngredients.Remove(foundEntity);

			await context.SaveChangesAsync();
		}

	}
}
