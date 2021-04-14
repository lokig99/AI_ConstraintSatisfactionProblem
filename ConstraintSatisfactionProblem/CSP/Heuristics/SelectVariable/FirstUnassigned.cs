using System.Linq;

namespace ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable
{
    public class FirstUnassigned<TK, TD> : ISelectVariableHeuristic<TK, TD>
    {
        public Variable<TK, TD> SelectVariable(CspProblem<TK, TD> problem)
        {
            return problem.Variables.First(v => !v.Assigned);
        }
    }
}