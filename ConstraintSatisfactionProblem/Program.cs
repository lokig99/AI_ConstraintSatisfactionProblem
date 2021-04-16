using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;
using ConstraintSatisfactionProblem.CSP.Solver;
using ConstraintSatisfactionProblem.Problems.Einstein;
using ConstraintSatisfactionProblem.Problems.MapColoring;
using ConstraintSatisfactionProblem.Tools;
using ConstraintSatisfactionProblem.Utils.Types;
using System;
using System.Linq;

namespace ConstraintSatisfactionProblem
{
    internal class Programs
    {
        internal enum SolverType
        {
            Backtracking,
            ForwardChecking,
            Ac3
        }

        internal static void Main(string[] args)
        {
            const int regionCount = 25;
            const int seed = 42;
            const int mapSize = 64;
            var domain = new[] { 1, 2, 3, 4 };

            MapCsp(domain, mapSize, regionCount,
                new FailFirstHeuristic<Point, int>(),
                new OriginalOrderHeuristic<Point, int>(),
                SolverType.Ac3, seed);
            MapCsp(domain, mapSize, regionCount,
                new DegreeHeuristic<Point, int>(),
                new OriginalOrderHeuristic<Point, int>(),
                SolverType.Backtracking, seed);

            MapCsp(domain, mapSize, regionCount,
                new FailFirstHeuristic<Point, int>(),
                new OriginalOrderHeuristic<Point, int>(),
                SolverType.ForwardChecking, seed);

            Console.WriteLine("\n__________________________________________\n");
            EinsteinCsp(
                new FailFirstHeuristic<EinsteinValue, House>(),
                new OriginalOrderHeuristic<EinsteinValue, House>(),
                SolverType.Ac3, showResult: false);

            EinsteinCsp(
                new FirstUnassignedHeuristic<EinsteinValue, House>(),
                new OriginalOrderHeuristic<EinsteinValue, House>(),
                SolverType.Backtracking, showResult: false);

            EinsteinCsp(
                new FailFirstHeuristic<EinsteinValue, House>(),
                new OriginalOrderHeuristic<EinsteinValue, House>(),
                SolverType.ForwardChecking, showResult: false);
        }

        internal static void MapCsp(int[] domain, int mapSize, int regionCount,
            ISelectVariableHeuristic<Point, int> selectVariable,
            IOrderDomainHeuristic<Point, int> orderDomain,
            SolverType solverType,
            int? seed = null)
        {
            var colorMapCsp = new MapColoringCsp(mapSize)
            { RandomGenerator = seed is null ? new Random() : new Random((int)seed) };
            colorMapCsp.ResetRegions(regionCount, domain);

            var solver = solverType switch
            {
                SolverType.Ac3 => new ArcConsistencySolver<Point, int>(selectVariable, orderDomain),
                SolverType.Backtracking => new BacktrackingSolver<Point, int>(selectVariable, orderDomain),
                _ => (CspSolver<Point, int>)new ForwardCheckingSolver<Point, int>(selectVariable, orderDomain)
            };

            var benchResult = BenchmarkCsp.BenchmarkFirstOnly(solver, colorMapCsp);
            Console.WriteLine($"__________________________\n{solverType}");
            benchResult.Report();
            if (benchResult.FirstSolution is not null)
            {
                MapCspSerializer.SerializeResultsToJson(benchResult.FirstSolution, colorMapCsp);
                MapCspSerializer.GenerateImage();
            }

            benchResult = BenchmarkCsp.BenchmarkAll(solver, colorMapCsp);
            benchResult.Report();
        }

        internal static void EinsteinCsp(
            ISelectVariableHeuristic<EinsteinValue, House> selectVariable,
            IOrderDomainHeuristic<EinsteinValue, House> orderDomain,
            SolverType solverType, bool showResult = false)
        {
            var einstein = new EinsteinCsp();
            var solver = solverType switch
            {
                SolverType.Ac3 => new ArcConsistencySolver<EinsteinValue, House>(selectVariable, orderDomain),
                SolverType.Backtracking => new BacktrackingSolver<EinsteinValue, House>(selectVariable, orderDomain),
                _ => (CspSolver<EinsteinValue, House>)new ForwardCheckingSolver<EinsteinValue, House>(selectVariable,
                    orderDomain)
            };

            var benchResult = BenchmarkCsp.BenchmarkFirstOnly(solver, einstein);

            if (showResult)
            {
                foreach (var house in HouseExtension.Houses)
                {
                    Console.WriteLine("_____________________");
                    Console.WriteLine(house);
                    Console.WriteLine("_____________________");
                    foreach (var einsteinValue in benchResult.FirstSolution
                        .Where(r => r.Value == house)
                        .Select(r => r.Key))
                    {
                        Console.WriteLine(einsteinValue);
                    }

                    Console.WriteLine("\n\n");
                }
            }

            Console.WriteLine($"__________________________\n{solverType}");
            benchResult.Report();
            benchResult = BenchmarkCsp.BenchmarkAll(solver, einstein);
            benchResult.Report();
        }
    }
}