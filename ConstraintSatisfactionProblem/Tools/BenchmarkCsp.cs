using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConstraintSatisfactionProblem.CSP;
using ConstraintSatisfactionProblem.CSP.Solver;

namespace ConstraintSatisfactionProblem.Tools
{
    public static class BenchmarkCsp
    {
        public readonly struct BenchmarkResult<TK, TD>
        {
            public BenchmarkResult(
                long? firstSolutionElapsedMilliseconds,
                ulong? firstSolutionNodesVisited,
                long? totalElapsedMilliseconds,
                ulong? totalNodesVisited,
                ulong? solutionsFound,
                Dictionary<TK, TD> firstSolution)
            {
                FirstSolutionElapsedMilliseconds = firstSolutionElapsedMilliseconds;
                FirstSolutionNodesVisited = firstSolutionNodesVisited;
                TotalElapsedMilliseconds = totalElapsedMilliseconds;
                TotalNodesVisited = totalNodesVisited;
                SolutionsFound = solutionsFound;
                FirstSolution = firstSolution;
            }

            public long? FirstSolutionElapsedMilliseconds { get; }
            public ulong? FirstSolutionNodesVisited { get; }
            public long? TotalElapsedMilliseconds { get; }
            public ulong? TotalNodesVisited { get; }
            public ulong? SolutionsFound { get; }
            public Dictionary<TK, TD> FirstSolution { get; }

            public void Report()
            {
                if (FirstSolutionElapsedMilliseconds is not null && FirstSolutionNodesVisited is not null)
                {
                    Console.WriteLine("First solution:");
                    Console.WriteLine($"\tFirst solution found in: {FirstSolutionElapsedMilliseconds} ms");
                    Console.WriteLine($"\tNodes visited to find first solution: {FirstSolutionNodesVisited}");
                }

                if (TotalElapsedMilliseconds is null || TotalNodesVisited is null || SolutionsFound is null) return;
                Console.WriteLine("All solutions:");
                Console.WriteLine($"\tNodes visited: {TotalNodesVisited}");
                Console.WriteLine($"\tTime duration: {TotalElapsedMilliseconds} ms");
                Console.WriteLine($"\tFound: {SolutionsFound}");
            }
        }

        public static BenchmarkResult<TK, TD> BenchmarkAll<TK, TD>(
            CspSolver<TK, TD> cspSolver, CspProblem<TK, TD> problem)
        {
            var stopWatch = new Stopwatch();
            problem.ClearAll();

            stopWatch.Restart();
            _ = cspSolver.FindSolutions(problem).Last();
            stopWatch.Stop();

            problem.ClearAll();

            return new BenchmarkResult<TK, TD>(
                null,
                null,
                stopWatch.ElapsedMilliseconds,
                cspSolver.NodesVisited,
                cspSolver.SolutionCount,
                null);
        }

        public static BenchmarkResult<TK, TD> BenchmarkFirstOnly<TK, TD>(
            CspSolver<TK, TD> cspSolver, CspProblem<TK, TD> problem)
        {
            var stopWatch = new Stopwatch();
            problem.ClearAll();

            stopWatch.Start();
            var first = cspSolver.FindSolutions(problem).First();
            stopWatch.Stop();

            var firstNodesVisited = cspSolver.NodesVisited;
            var firstSolutionTime = stopWatch.ElapsedMilliseconds;

            problem.ClearAll();

            return new BenchmarkResult<TK, TD>(
                firstSolutionTime,
                firstNodesVisited,
                null,
                null,
                null,
                first);
        }
    }
}