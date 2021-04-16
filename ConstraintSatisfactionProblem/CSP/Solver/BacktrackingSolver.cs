using System.Collections.Generic;
using System.Linq;
using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;

namespace ConstraintSatisfactionProblem.CSP.Solver
{
    public class BacktrackingSolver<TK, TD> : CspSolver<TK, TD>
    {
        public BacktrackingSolver(ISelectVariableHeuristic<TK, TD> selectVariableHeuristic,
            IOrderDomainHeuristic<TK, TD> orderDomainHeuristic) : base(selectVariableHeuristic, orderDomainHeuristic)
        {
        }

        public override IEnumerable<Dictionary<TK, TD>> FindSolutions(CspProblem<TK, TD> problem)
        {
            return BacktrackingSearch(problem);
        }

        private IEnumerable<Dictionary<TK, TD>> BacktrackingSearch(CspProblem<TK, TD> problem)
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
                    foreach (var value in variable.OrderDomainValues(OrderDomainHeuristic))
                    {
                        NodesVisited++;
                        variable.Value = value;
                        if (variable.Consistent)
                        {
                            // try finding solution
                            foreach (var solution in Search().Where(s => s is not null))
                            {
                                yield return solution;
                            }
                        }

                        variable.Assigned = false;
                    }

                    yield return null;
                }
            }
        }
    }
}