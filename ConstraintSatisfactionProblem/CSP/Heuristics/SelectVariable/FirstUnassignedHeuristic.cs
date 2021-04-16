using System.Linq;

namespace ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable
{
    public class FirstUnassignedHeuristic<TK, TD> : ISelectVariableHeuristic<TK, TD>
    {
        public Variable<TK, TD> SelectVariable(CspProblem<TK, TD> problem)
        {
            return problem.UnassignedVariables.First();
        }
    }
}