using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;

namespace Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes
{
    public class TeleportatorEntityRecipe : IEntityRecipe
    {
        public EntityType Type => EntityType.Teleportator;

        private IColliderRegistryService _colliderRegistryService;

        public TeleportatorEntityRecipe(IColliderRegistryService colliderRegistryService)
        {
            _colliderRegistryService = colliderRegistryService;
        }

        public void Build(Entity entity)
        {
            //Teleport
            entity
                .Add(new TeleportRadius(5))
                .Add(new TeleportRequest())
                .Add(new TeleportEvent())
                .AddSystem(new RigidbodyTeleportSystem());

            CanTeleport canTeleport = new CanTeleport();
            canTeleport.Add(new FuncCondition(() => true));

            entity
                .Add(canTeleport);
        }
    }
}