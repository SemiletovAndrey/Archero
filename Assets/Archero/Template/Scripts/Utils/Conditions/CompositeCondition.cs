using System;
using System.Collections.Generic;

namespace Template.Scripts.Utils
{
    public class CompositeCondition : ICompositeCondition
    {
        private List<ICondition> _conditions = new List<ICondition>();
        private Func<bool, bool, bool> _standardLogicOperation;

        public CompositeCondition() : this(LogicOperation.And)
        {
        }

        public CompositeCondition(Func<bool, bool, bool> standardLogicOperation)
        {
            _standardLogicOperation = standardLogicOperation;
        }

        public bool Evaluate()
        {
            if (_conditions.Count == 0)
                return false;

            bool result = _conditions[0].Evaluate();

            for (int i = 1; i < _conditions.Count; i++)
            {
                ICondition condition = _conditions[i];

                result = _standardLogicOperation(result, condition.Evaluate());
            }

            return result;
        }

        public ICompositeCondition Add(ICondition condition)
        {
            _conditions.Add(condition);
            return this;
        }

        public ICompositeCondition Remove(ICondition condition)
        {
            _conditions.Remove(condition);
            return this;
        }
    }
}