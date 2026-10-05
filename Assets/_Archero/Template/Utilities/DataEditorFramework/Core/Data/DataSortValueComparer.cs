using System;

namespace DataEditorFramework.Core.Data
{
    public static class DataSortValueComparer
    {
        public static int Compare(object left, object right)
        {
            if (ReferenceEquals(left, right))
                return 0;
            if (left == null)
                return 1;
            if (right == null)
                return -1;

            if (left is string leftString && right is string rightString)
                return string.Compare(leftString, rightString, StringComparison.OrdinalIgnoreCase);

            if (left is Enum leftEnum && right is Enum rightEnum)
                return Convert.ToInt64(leftEnum).CompareTo(Convert.ToInt64(rightEnum));

            var leftType = left.GetType();
            var rightType = right.GetType();
            if (leftType == rightType && left is IComparable comparable)
                return comparable.CompareTo(right);

            return string.Compare(left.ToString(), right.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
