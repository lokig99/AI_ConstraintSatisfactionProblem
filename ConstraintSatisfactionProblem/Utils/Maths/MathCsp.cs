using System.Collections.Generic;

namespace ConstraintSatisfactionProblem.Utils.Maths
{
    public static class MathCsp
    {
        public static IEnumerable<(T, T)> Combinations<T>(params T[] values)
        {
            var index = 1;
            foreach (var val in values)
            {
                for (var i = index; i < values.Length; i++)
                {
                    yield return new(val, values[i]);
                }

                index++;
            }
        }
    }
}