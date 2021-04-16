using System.Collections.Generic;

namespace ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain
{
    public interface IOrderDomainHeuristic<TK, TD>
    {
        IEnumerable<TD> DomainOrdered(Variable<TK, TD> variable);
    }
}