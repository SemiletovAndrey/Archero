namespace Template.Scripts.Utils
{
    public interface ICompositeCondition : ICondition
    {
        public ICompositeCondition Add(ICondition condition);
        public ICompositeCondition Remove(ICondition condition);
    }
}