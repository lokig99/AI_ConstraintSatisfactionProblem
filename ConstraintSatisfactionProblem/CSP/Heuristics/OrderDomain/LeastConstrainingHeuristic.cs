using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain
{
    public class LeastConstrainingHeuristic<TK, TD> : IOrderDomainHeuristic<TK, TD>
    {
        public IEnumerable<TD> DomainOrdered(Variable<TK, TD> variable)
        {
            return variable.Domain
                .OrderByDescending(d =>
                    variable.Constraints
                        .Where(c => !c.VariableTwo.Assigned)
                        .Sum(c => c.VariableTwo.Domain
                            .Sum(d2 => c.Test(d, d2) ? 1 : 0)));
        }
    }
}