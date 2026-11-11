using System;

namespace Template.Scripts.Utils
{
    public class FuncCondition : ICondition
    {
        private Func<bool> _condition;

        public FuncCondition(Func<bool> condition)
        {
            _condition = condition;
        }

        public bool Evaluate()
        {
            return _condition.Invoke();
        }
    }
}