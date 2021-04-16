using System.Collections.Generic;
using System.Linq;
using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;

namespace ConstraintSatisfactionProblem.CSP.Solver
{
    public abstract class CspSolver<TK, TD>
    {
        public ISelectVariableHeuristic<TK, TD> SelectVariableHeuristic { get; set; }
        public IOrderDomainHeuristic<TK, TD> OrderDomainHeuristic { get; set; }
        public ulong NodesVisited { get; protected set; }
        public ulong SolutionCount { get; protected set; }

        protected CspSolver(ISelectVariableHeuristic<TK, TD> selectVariableHeuristic,
            IOrderDomainHeuristic<TK, TD> orderDomainHeuristic)
        {
            SelectVariableHeuristic = selectVariableHeuristic;
            OrderDomainHeuristic = orderDomainHeuristic;
        }

        public abstract IEnumerable<Dictionary<TK, TD>> FindSolutions(CspProblem<TK, TD> problem);
    }
}