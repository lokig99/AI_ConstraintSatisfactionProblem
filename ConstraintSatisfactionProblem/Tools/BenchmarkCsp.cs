using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConstraintSatisfactionProblem.CSP;

namespace ConstraintSatisfactionProblem.Tools
{
    public static class BenchmarkCsp
    {
        public readonly struct BenchmarkResult<TK, TD>
        {
            public BenchmarkResult(
                long firstSolutionElapsedMilliseconds,
                ulong firstSolutionNodesVisited,
                long totalElapsedMilliseconds,
                ulong totalNodesVisited,
                ulong solutionsFound,
                Dictionary<TK, TD> firstSolution)
            {
                FirstSolutionElapsedMilliseconds = firstSolutionElapsedMilliseconds;
                FirstSolutionNodesVisited = firstSolutionNodesVisited;
                TotalElapsedMilliseconds = totalElapsedMilliseconds;
                TotalNodesVisited = totalNodesVisited;
                SolutionsFound = solutionsFound;
                FirstSolution = firstSolution;
            }

            public long FirstSolutionElapsedMilliseconds { get; }
            public ulong FirstSolutionNodesVisited { get; }
            public long TotalElapsedMilliseconds { get; }
            public ulong TotalNodesVisited { get; }
            public ulong SolutionsFound { get; }
            public Dictionary<TK, TD> FirstSolution { get; }

            public void Report()
            {
                Console.WriteLine("First solution:");
                Console.WriteLine($"\tFirst solution found in: {FirstSolutionElapsedMilliseconds} ms");
                Console.WriteLine($"\tNodes visited to find first solution: {FirstSolutionNodesVisited}");
                Console.WriteLine("All solutions:");
                Console.WriteLine($"\tNodes visited: {TotalNodesVisited}");
                Console.WriteLine($"\tTime duration: {TotalElapsedMilliseconds} ms");
                Console.WriteLine($"\tFound: {SolutionsFound}");
            }
        }

        public static BenchmarkResult<TK, TD> Benchmark<TK, TD>(
            CspSolver<TK, TD> cspSolver, CspProblem<TK, TD> problem)
        {
            var stopWatch = new Stopwatch();

            // first solution
            problem.ClearAll();
            stopWatch.Start();
            var first = cspSolver.BacktrackingSearch(problem).First();
            stopWatch.Stop();

            var firstNodesVisited = cspSolver.NodesVisited;
            var firstSolutionTime = stopWatch.ElapsedMilliseconds;

            // all solutions
            problem.ClearAll();
            stopWatch.Restart();
            _ = cspSolver.BacktrackingSearch(problem).Last();
            stopWatch.Stop();

            return new BenchmarkResult<TK, TD>(
                firstSolutionTime,
                firstNodesVisited,
                stopWatch.ElapsedMilliseconds,
                cspSolver.NodesVisited,
                cspSolver.SolutionCount,
                first);
        }
    }
}