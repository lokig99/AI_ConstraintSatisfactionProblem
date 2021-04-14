using System;
using System.Collections.Generic;

namespace ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain
{
    public class OriginalOrder<TK, TD> : IOrderDomainHeuristic<TK, TD>
    {
        public IEnumerable<TD> DomainOrdered(Variable<TK, TD> variable)
        {
            return variable.Domain;
        }
    }
}