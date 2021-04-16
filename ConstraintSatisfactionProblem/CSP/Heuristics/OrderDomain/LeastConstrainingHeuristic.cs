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
                    variable.SetValue(d).Constraints
                        .Where(cv => !cv.v.Assigned)
                        .Sum(cv => cv.v.Domain
                            .Sum(d2 => cv.v.CheckConsistency(d2) ? 1 : 0)));
        }
    }
}