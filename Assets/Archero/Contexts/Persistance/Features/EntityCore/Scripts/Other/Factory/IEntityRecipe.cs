namespace Persistance.Features.EntityCore
{
    public interface IEntityRecipe
    {
        public EntityType Type { get; }
        
        public void Build(Entity entity);
    }
}
