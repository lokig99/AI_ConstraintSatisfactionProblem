using System;
using System.Diagnostics;
using System.Linq;
using ConstraintSatisfactionProblem.CSP;
using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;
using ConstraintSatisfactionProblem.CSP.Solver;
using ConstraintSatisfactionProblem.Problems.Einstein;
using ConstraintSatisfactionProblem.Problems.MapColoring;
using ConstraintSatisfactionProblem.Tools;
using ConstraintSatisfactionProblem.Utils.Types;

namespace ConstraintSatisfactionProblem
{
    internal class Programs
    {
        internal static void Main(string[] args)
        {
            MapCsp(new[] { 1, 2, 3, 4 }, 32, 20, 42);
            MapCspForward(new[] { 1, 2, 3, 4 }, 32, 20, 42);
            //EinsteinCsp();
        }

        internal static void MapCsp(int[] domain, int mapSize, int regionCount, int? seed = null)
        {
            var colorMapCsp = new MapColoringCsp(mapSize)
            { RandomGenerator = seed is null ? new Random() : new Random((int)seed) };
            colorMapCsp.ResetRegions(regionCount, domain);

            var solver = new BacktrackingSolver<Point, int>(
                new FirstUnassignedHeuristic<Point, int>(),
                new OriginalOrderHeuristic<Point, int>());

            var benchResult = BenchmarkCsp.BenchmarkFirstOnly(solver, colorMapCsp);
            benchResult.Report();
            if (benchResult.FirstSolution is not null)
            {
                MapCspSerializer.SerializeResultsToJson(benchResult.FirstSolution, colorMapCsp);
                MapCspSerializer.GenerateImage();
            }

            benchResult = BenchmarkCsp.BenchmarkAll(solver, colorMapCsp);
            benchResult.Report();
        }

        internal static void MapCspForward(int[] domain, int mapSize, int regionCount, int? seed = null)
        {
            var colorMapCsp = new MapColoringCsp(mapSize)
            { RandomGenerator = seed is null ? new Random() : new Random((int)seed) };
            colorMapCsp.ResetRegions(regionCount, domain);

            var solver = new ForwardCheckingSolver<Point, int>(
                new FailFirstHeuristic<Point, int>(),
                new OriginalOrderHeuristic<Point, int>());

            var benchResult = BenchmarkCsp.BenchmarkFirstOnly(solver, colorMapCsp);
            benchResult.Report();
            if (benchResult.FirstSolution is not null)
            {
                MapCspSerializer.SerializeResultsToJson(benchResult.FirstSolution, colorMapCsp);
                MapCspSerializer.GenerateImage();
            }

            benchResult = BenchmarkCsp.BenchmarkAll(solver, colorMapCsp);
            benchResult.Report();
        }

        internal static void EinsteinCsp()
        {
            var einstein = new EinsteinCsp();
            var solver = new ForwardCheckingSolver<EinsteinValue, House>(
                new FailFirstHeuristic<EinsteinValue, House>(),
                new OriginalOrderHeuristic<EinsteinValue, House>());

            var benchResult = BenchmarkCsp.BenchmarkFirstOnly(solver, einstein);

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

            benchResult.Report();
        }
    }
}