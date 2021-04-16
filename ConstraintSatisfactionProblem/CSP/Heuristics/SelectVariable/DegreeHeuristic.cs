namespace ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable
{
    public class DegreeHeuristic<TK, TD> : ISelectVariableHeuristic<TK, TD>
    {
        public Variable<TK, TD> SelectVariable(CspProblem<TK, TD> problem)
        {
            var maxDegree = int.MinValue;
            Variable<TK, TD> maxDegreeVariable = null;

            foreach (var variable in problem.UnassignedVariables)
            {
                if (variable.Constraints.Count <= maxDegree) continue;
                maxDegree = variable.Constraints.Count;
                maxDegreeVariable = variable;
            }

            return maxDegreeVariable;
        }
    }
}