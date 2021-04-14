using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;

namespace ConstraintSatisfactionProblem.CSP
{
    public class CspSolver<TK, TD>
    {
        public ISelectVariableHeuristic<TK, TD> SelectVariableHeuristic { get; set; }
        public IOrderDomainHeuristic<TK, TD> OrderDomainHeuristic { get; set; }
        public ulong NodesVisited { get; private set; }
        public ulong SolutionCount { get; private set; }

        public CspSolver(ISelectVariableHeuristic<TK, TD> selectVariableHeuristic,
            IOrderDomainHeuristic<TK, TD> orderDomainHeuristic)
        {
            SelectVariableHeuristic = selectVariableHeuristic;
            OrderDomainHeuristic = orderDomainHeuristic;
        }

        public IEnumerable<Dictionary<TK, TD>> BacktrackingSearch(CspProblem<TK, TD> problem)
        {
            NodesVisited = 0L;
            SolutionCount = 0L;
            foreach (var assignment in Search().SkipLast(1))
            {
                yield return assignment;
            }

            IEnumerable<Dictionary<TK, TD>> Search()
            {
                if (problem.Complete)
                {
                    SolutionCount++;
                    yield return problem.Assignment;
                }
                else
                {
                    var variable = problem.NextUnassigned(SelectVariableHeuristic);
                    foreach (var value in variable.OrderDomainValues(OrderDomainHeuristic))
                    {
                        NodesVisited++;
                        variable.Value = value;

                        if (problem.Consistent)
                        {
                            // try finding solution
                            foreach (var solution in Search().Where(s => s is not null))
                            {
                                yield return solution;
                            }
                        }

                        variable.Clear();
                    }

                    yield return null;
                }
            }
        }
    }
}