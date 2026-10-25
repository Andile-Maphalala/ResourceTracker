using MediatR;

namespace ResourceTracker.Application.Features.Queries.RecipeQueries.GetRecipe
{
    public record GetRecipeQuery(int Id) : IRequest<GetRecipeResponse>;
}
