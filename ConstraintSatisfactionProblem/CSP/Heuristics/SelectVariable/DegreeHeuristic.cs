using System.Linq;

namespace ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable
{
    public class DegreeHeuristic<TK, TD> : ISelectVariableHeuristic<TK, TD>
    {
        public Variable<TK, TD> SelectVariable(CspProblem<TK, TD> problem)
        {
            var unassigned = problem.UnassignedVariables.ToArray();
            var maxDegree = int.MinValue;
            var maxDegreeIndex = -1;

            for (var i = 0; i < unassigned.Length; i++)
            {
                var variable = unassigned[i];
                if (variable.Constraints.Count <= maxDegree) continue;
                maxDegree = variable.Constraints.Count;
                maxDegreeIndex = i;
            }

            return unassigned[maxDegreeIndex];
        }
    }
}