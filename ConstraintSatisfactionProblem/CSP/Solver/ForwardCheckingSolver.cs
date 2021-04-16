using System.Collections.Generic;
using System.Linq;
using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;

namespace ConstraintSatisfactionProblem.CSP.Solver
{
    public class ForwardCheckingSolver<TK, TD> : CspSolver<TK, TD>
    {
        public ForwardCheckingSolver(ISelectVariableHeuristic<TK, TD> selectVariableHeuristic,
            IOrderDomainHeuristic<TK, TD> orderDomainHeuristic) : base(selectVariableHeuristic, orderDomainHeuristic)
        {
        }

        public override IEnumerable<Dictionary<TK, TD>> FindSolutions(CspProblem<TK, TD> problem)
        {
            return ForwardCheckingSearch(problem);
        }

        private IEnumerable<Dictionary<TK, TD>> ForwardCheckingSearch(CspProblem<TK, TD> problem)
        {
            NodesVisited = 0L;
            SolutionCount = 0L;
            foreach (var assignment in Search())
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
                    var unassignedConnectedVars =
                        variable.Constraints
                            .Where(vc => !vc.v.Assigned)
                            .Select(vc => vc.v)
                            .ToArray();

                    foreach (var value in variable.OrderDomainValues(OrderDomainHeuristic))
                    {
                        NodesVisited++;
                        variable.Value = value;

                        if (variable.Consistent)
                        {
                            // forward checking ->
                            // remove from Variables connected by constraints
                            // the values that are inconsistent with selected value
                            foreach (var v in unassignedConnectedVars)
                            {
                                v.RemoveFromDomain(v.Domain.Where(d => !v.CheckConsistency(d)).ToArray());
                            }

                            // try finding solution
                            foreach (var solution in Search().Where(s => s is not null))
                            {
                                yield return solution;
                            }

                            // restore domains to previous state
                            foreach (var v in unassignedConnectedVars)
                            {
                                v.RestorePreviousDomain();
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