using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.Helper;
using ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipe;
using ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipesForComponent;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;

namespace ResourceTracker.Application.Features.Queries.RecipeQueries
{
    public class RecipeQueriesHandler :
        IRequestHandler<GetRecipeQuery, GetRecipeResponse>,
        IRequestHandler<GetRecipesForComponentQuery, List<GetRecipeResponse>>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly string _baseUrl;

        public RecipeQueriesHandler(IResourceTrackerRepository repo, IOptions<ApplicationOptions> options)
        {
            _repo = repo;
            _baseUrl = options.Value.BaseUrl;
        }

        public async Task<GetRecipeResponse> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
        {
            var recipe = await Query().FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (recipe is null)
                throw new NotFoundException(nameof(Recipe), request.Id);

            return Map(recipe);
        }

        public async Task<List<GetRecipeResponse>> Handle(GetRecipesForComponentQuery request, CancellationToken cancellationToken)
        {
            var componentExists = await _repo.Components
                .AnyAsync(c => c.Id == request.ComponentId, cancellationToken);

            if (!componentExists)
                throw new NotFoundException(nameof(Component), request.ComponentId);

            var recipes = await Query()
                .Where(r => r.ComponentId == request.ComponentId)
                .OrderBy(r => r.Id)
                .ToListAsync(cancellationToken);

            return recipes.Select(Map).ToList();
        }

        private IQueryable<Recipe> Query()
        {
            // Loaded then mapped in memory rather than projected in SQL: the image URL helpers are
            // C# code, and the nested ingredient list makes the projection awkward to translate.
            return _repo.Recipes
                .Include(r => r.Component)
                    .ThenInclude(c => c.Picture)
                .Include(r => r.RequiredFacility)
                    .ThenInclude(f => f.Picture)
                .Include(r => r.RecipeComponents)
                    .ThenInclude(rc => rc.Component)
                    .ThenInclude(c => c.Picture);
        }

        private GetRecipeResponse Map(Recipe recipe) => new()
        {
            Id = recipe.Id,
            ComponentId = recipe.ComponentId,
            ComponentName = recipe.Component.Name,
            ComponentImageUrl = recipe.Component.Picture.GetFileUrl(_baseUrl),
            AmountMade = recipe.AmountMade,
            RequiredFacilityId = recipe.RequiredFacilityId,
            RequiredFacilityName = recipe.RequiredFacility?.Name,
            RequiredFacilityImageUrl = recipe.RequiredFacility?.Picture.GetFileUrl(_baseUrl),
            RequiredFacilityLevel = recipe.RequiredFacilityLevel,
            Ingredients = recipe.RecipeComponents
                .OrderBy(rc => rc.Component.Name)
                .Select(rc => new RecipeIngredientResponse
                {
                    ComponentId = rc.ComponentId,
                    ComponentName = rc.Component.Name,
                    ComponentType = rc.Component.Type,
                    ComponentTypeName = EnumHelper.GetEnumDescription((ComponentTypeEnum)rc.Component.Type),
                    AmountRequired = rc.AmountRequired,
                    ComponentImageUrl = rc.Component.Picture.GetFileUrl(_baseUrl)
                })
                .ToList()
        };
    }
}
