using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Application.Common.CQRS;
using ResourceTracker.Application.Common.Exceptions;
using ResourceTracker.Application.Common.User;
using ResourceTracker.Application.Features.Commands.RecipeCommands.CreateRecipe;
using ResourceTracker.Application.Features.Commands.RecipeCommands.DeleteRecipe;
using ResourceTracker.Application.Features.Commands.RecipeCommands.UpdateRecipe;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Domain.Enums;
using Component = ResourceTracker.Domain.Entities.Component;

namespace ResourceTracker.Application.Features.Commands.RecipeCommands
{
    public class RecipeCommandHandler :
        ICommandHandler<CreateRecipeCommand, CreateRecipeResponse>,
        ICommandHandler<UpdateRecipeCommand>,
        ICommandHandler<DeleteRecipeCommand>
    {
        private readonly IResourceTrackerRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserInfo _userInfo;

        public RecipeCommandHandler(IResourceTrackerRepository repo, IUnitOfWork unitOfWork, IUserInfo userInfo)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
        }

        public async Task<CreateRecipeResponse> Handle(CreateRecipeCommand command, CancellationToken cancellationToken)
        {
            EnsureAdmin();

            await EnsureCraftable(command.ComponentId, cancellationToken);
            await EnsureStation(command.RequiredFacilityId, cancellationToken);
            await EnsureIngredients(command.ComponentId, command.Ingredients, cancellationToken);
            await EnsureNoOtherRecipe(command.ComponentId, excludingRecipeId: null, cancellationToken);

            var item = new Recipe
            {
                ComponentId = command.ComponentId,
                AmountMade = command.AmountMade,
                RequiredFacilityId = command.RequiredFacilityId,
                RequiredFacilityLevel = NormaliseLevel(command.RequiredFacilityId, command.RequiredFacilityLevel),
                RecipeComponents = command.Ingredients
                    .Select(i => new RecipeComponent
                    {
                        ComponentId = i.ComponentId,
                        AmountRequired = i.AmountRequired
                    })
                    .ToList()
            };

            await _repo.InsertAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return new CreateRecipeResponse(item.Id);
        }

        public async Task<Unit> Handle(UpdateRecipeCommand command, CancellationToken cancellationToken)
        {
            EnsureAdmin();

            var item = await _repo.Recipes.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
                throw new NotFoundException(nameof(Recipe), command.Id);

            await EnsureCraftable(command.ComponentId, cancellationToken);
            await EnsureStation(command.RequiredFacilityId, cancellationToken);
            await EnsureIngredients(command.ComponentId, command.Ingredients, cancellationToken);
            await EnsureNoOtherRecipe(command.ComponentId, command.Id, cancellationToken);

            item.ComponentId = command.ComponentId;
            item.AmountMade = command.AmountMade;
            item.RequiredFacilityId = command.RequiredFacilityId;
            item.RequiredFacilityLevel = NormaliseLevel(command.RequiredFacilityId, command.RequiredFacilityLevel);

            // Upsert rather than delete-and-reinsert: RecipeComponent is keyed by
            // (RecipeId, ComponentId), so removing a row and adding one back with the same key in
            // the same unit of work collides in the change tracker.
            var existing = await _repo.Set<RecipeComponent>()
                .Where(rc => rc.RecipeId == item.Id)
                .ToListAsync(cancellationToken);

            var requested = command.Ingredients
                .ToDictionary(i => i.ComponentId, i => i.AmountRequired);

            foreach (var recipeComponent in existing)
            {
                if (requested.Remove(recipeComponent.ComponentId, out var amount))
                {
                    recipeComponent.AmountRequired = amount;
                }
                else
                {
                    await _repo.DeleteAsync(recipeComponent, cancellationToken);
                }
            }

            foreach (var (componentId, amount) in requested)
            {
                await _repo.InsertAsync(new RecipeComponent
                {
                    RecipeId = item.Id,
                    ComponentId = componentId,
                    AmountRequired = amount
                }, cancellationToken);
            }

            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteRecipeCommand command, CancellationToken cancellationToken)
        {
            EnsureAdmin();

            var item = await _repo.Recipes.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (item == null)
                throw new NotFoundException(nameof(Recipe), command.Id);

            // RecipeComponents go with it: the relationship is configured as cascade.
            await _repo.DeleteAsync(item, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }

        private void EnsureAdmin()
        {
            if (!_userInfo.IsAdmin())
                throw new BadRequestException("Unauthorised action");
        }

        /// <summary>
        /// A recipe's output must exist. Resources are allowed here because the import path can
        /// attach recipes to any component; what matters is that the id resolves.
        /// </summary>
        private async Task EnsureCraftable(int componentId, CancellationToken cancellationToken)
        {
            var exists = await _repo.Components.AnyAsync(c => c.Id == componentId, cancellationToken);
            if (!exists)
                throw new NotFoundException(nameof(Component), componentId);
        }

        private async Task EnsureStation(int? facilityId, CancellationToken cancellationToken)
        {
            if (!facilityId.HasValue)
                return;

            var facility = await _repo.Components.FirstOrDefaultAsync(c => c.Id == facilityId.Value, cancellationToken);
            if (facility == null)
                throw new NotFoundException(nameof(Component), facilityId.Value);

            if (facility.Type != (int)ComponentTypeEnum.Facility)
            {
                throw new BadRequestException($"{facility.Name} is not a facility and cannot be used as a recipe station.");
            }
        }

        private async Task EnsureIngredients(int outputComponentId, List<RecipeIngredientDto>? ingredients, CancellationToken cancellationToken)
        {
            if (ingredients == null || ingredients.Count == 0)
                return;

            var ids = ingredients.Select(i => i.ComponentId).ToList();

            var duplicates = ids.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Count > 0)
            {
                throw new BadRequestException($"Ingredient component appears more than once: {string.Join(", ", duplicates)}.");
            }

            if (ids.Contains(outputComponentId))
            {
                throw new BadRequestException("A recipe cannot require its own output; the requirement resolver would recurse forever.");
            }

            var known = await _repo.Components
                .Where(c => ids.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var missing = ids.Except(known).ToList();
            if (missing.Count > 0)
            {
                throw new BadRequestException($"Unknown ingredient component ids: {string.Join(", ", missing)}.");
            }
        }

        /// <summary>
        /// Both requirement resolvers take only the first recipe per output, so a second recipe for
        /// the same component would be silently ignored. Refuse it rather than store dead data.
        /// </summary>
        private async Task EnsureNoOtherRecipe(int componentId, int? excludingRecipeId, CancellationToken cancellationToken)
        {
            var query = _repo.Recipes.Where(r => r.ComponentId == componentId);
            if (excludingRecipeId.HasValue)
            {
                query = query.Where(r => r.Id != excludingRecipeId.Value);
            }

            var exists = await query.AnyAsync(cancellationToken);

            if (exists)
            {
                throw new BadRequestException(
                    $"A recipe for component {componentId} already exists. Only one recipe per output is used, so a second would be ignored.");
            }
        }

        private static int NormaliseLevel(int? facilityId, int requiredLevel)
        {
            // Without a station the level is meaningless; keep the column at its default so it
            // never reads as a requirement that cannot be satisfied.
            return facilityId.HasValue ? requiredLevel : 1;
        }
    }
}
