using System;
using System.Collections.Generic;

namespace ConstraintSatisfactionProblem
{
    public static class CspSolver
    {
        public static List<Dictionary<TK, TD>> BacktrackingSearch<TK, TD>(CspProblem<TK, TD> problem)
        {
            var solutions = new List<Dictionary<TK, TD>>();
            Search();
            return solutions;

            bool Search()
            {
                if (problem.Complete) return true;

                var variable = problem.NextUnassigned();
                foreach (var value in variable.OrderDomainValues())
                {
                    variable.Value = value;
                    variable.Assigned = true;
                    if (!problem.Consistent)
                    {
                        variable.Assigned = false;
                        continue;
                    }

                    // try finding solution
                    if (Search())
                    {
                        solutions.Add(problem.Assignment);
                    }

                    variable.Assigned = false;
                }

                return false;
            }
        }
    }
}