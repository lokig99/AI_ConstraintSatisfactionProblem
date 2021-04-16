using System.Linq;

namespace ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable
{
    public class FailFirstHeuristic<TK, TD> : ISelectVariableHeuristic<TK, TD>
    {
        public Variable<TK, TD> SelectVariable(CspProblem<TK, TD> problem)
        {
            var unassigned = problem.UnassignedVariables.ToArray();
            var minRemaining = int.MaxValue;
            var minRemainingIndex = -1;

            for (var i = 0; i < unassigned.Length; i++)
            {
                var variable = unassigned[i];
                var domain = variable.Domain.ToArray();
                if (domain.Length >= minRemaining) continue;
                if (domain.Length == 0) return unassigned[i];
                minRemaining = domain.Length;
                minRemainingIndex = i;
            }

            return unassigned[minRemainingIndex];
        }
    }
}