using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConstraintSatisfactionProblem.CSP.Solver
{
    public class ArcConsistencySolver<TK, TD> : CspSolver<TK, TD>
    {
        public ArcConsistencySolver(ISelectVariableHeuristic<TK, TD> selectVariableHeuristic,
            IOrderDomainHeuristic<TK, TD> orderDomainHeuristic) : base(selectVariableHeuristic, orderDomainHeuristic)
        {
        }

        public override IEnumerable<Dictionary<TK, TD>> FindSolutions(CspProblem<TK, TD> problem)
        {
            return Ac3Search(problem);
        }


        private IEnumerable<Dictionary<TK, TD>> Ac3Search(CspProblem<TK, TD> problem)
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
                    var removals = Ac3(problem, out var isConsistent);

                    if (isConsistent)
                    {
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
                    }

                    foreach (var (v, removalCount) in removals)
                    {
                        v.RestorePreviousDomain(removalCount - 1);
                    }

                    yield return null;
                }
            }
        }

        private static Dictionary<Variable<TK, TD>, int> Ac3(CspProblem<TK, TD> problem, out bool isConsistent)
        {
            var queue = new LinkedList<BinaryConstraint<TK, TD>>(problem.Constraints);
            var removalsCounter = new Dictionary<Variable<TK, TD>, int>();
            while (queue.Any())
            {
                var con = queue.First?.Value;
                queue.RemoveFirst();

                if (!RemoveInconsistentValues(con, out var emptyFound)) continue;

                if (emptyFound)
                {
                    isConsistent = false;
                    return removalsCounter;
                }

                Debug.Assert(con?.VariableOne.Constraints != null, "con?.VariableOne.Constraints != null");
                foreach (var c in con.VariableOne.Constraints)
                {
                    queue.AddLast(c);
                }
            }

            isConsistent = true;
            return removalsCounter;

            bool RemoveInconsistentValues(BinaryConstraint<TK, TD> c, out bool emptyDomainFound)
            {
                var (v1, v2) = (c.VariableOne, c.VariableTwo);
                if (v1.Assigned)
                {
                    emptyDomainFound = false;
                    return false;
                }

                var removed = false;
                var toRemove = new List<TD>();

                foreach (var d in v1.Domain)
                {
                    if (v2.Domain.Any(d2 => c.Test(d, d2))) continue;
                    toRemove.Add(d);
                    removed = true;
                }

                if (!removed)
                {
                    emptyDomainFound = false;
                    return false;
                }

                v1.RemoveFromDomain(toRemove);
                emptyDomainFound = !v1.Domain.Any();
                removalsCounter[v1] = removalsCounter.GetValueOrDefault(v1, 0) + 1;
                return true;
            }
        }
    }
}