using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConstraintSatisfactionProblem
{
    public static class CspSolver
    {
        public static List<Dictionary<TK, TD>> BacktrackingSearch<TK, TD>(CspProblem<TK, TD> problem,
            out ulong nodesVisited, out long timeDurationMilliseconds)
        {
            var stopWatch = new Stopwatch();
            ulong nodesCounter = 0L;
            var solutions = new List<Dictionary<TK, TD>>();
            stopWatch.Start();
            Search();
            stopWatch.Stop();
            nodesVisited = nodesCounter;
            timeDurationMilliseconds = stopWatch.ElapsedMilliseconds;
            return solutions;

            bool Search()
            {
                if (problem.Complete) return true;

                var variable = problem.NextUnassigned();
                foreach (var value in variable.OrderDomainValues())
                {
                    nodesCounter++;
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