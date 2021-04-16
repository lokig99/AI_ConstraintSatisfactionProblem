using System.Linq;

namespace ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable
{
    public class FailFirstHeuristic<TK, TD> : ISelectVariableHeuristic<TK, TD>
    {
        public Variable<TK, TD> SelectVariable(CspProblem<TK, TD> problem)
        {
            var minRemaining = int.MaxValue;
            Variable<TK, TD> failFirstVariable = null;

            foreach (var variable in problem.UnassignedVariables)
            {
                var domainSize = variable.Domain.Count();
                if (domainSize >= minRemaining) continue;
                if (domainSize == 0) return variable;
                minRemaining = domainSize;
                failFirstVariable = variable;
            }

            return failFirstVariable;
        }
    }
}