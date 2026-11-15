namespace Persistance.Features.EntityCore
{
    public interface IEntityRecipe : IEntityRecipeBase
    {
        public void Build(Entity entity);
    }
    
    public interface IEntityRecipe<TArgs> : IEntityRecipeBase
    {
        public void Build(Entity entity, TArgs args);
    }
}
