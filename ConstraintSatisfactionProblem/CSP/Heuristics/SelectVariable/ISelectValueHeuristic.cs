namespace ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable
{
    public interface ISelectVariableHeuristic<TK, TD>
    {
        Variable<TK, TD> SelectVariable(CspProblem<TK, TD> problem);
    }
}