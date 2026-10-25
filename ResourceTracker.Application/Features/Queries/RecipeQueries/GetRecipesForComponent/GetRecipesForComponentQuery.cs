using MediatR;
using ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipe;

namespace ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipesForComponent
{
    /// <summary>
    /// Every recipe whose output is this component. Today that is at most one - the resolvers only
    /// ever use the first - but the endpoint is a list so that does not have to change shape later.
    /// </summary>
    public record GetRecipesForComponentQuery(int ComponentId) : IRequest<List<GetRecipeResponse>>;
}
