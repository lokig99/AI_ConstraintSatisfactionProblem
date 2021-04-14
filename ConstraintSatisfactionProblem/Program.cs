using System;
using System.Linq;
using ConstraintSatisfactionProblem.CSP;
using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;
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
            MapCsp(new[] { 1, 2, 3, 4 }, 32, 20);
            //EinsteinCsp();
        }

        internal static void MapCsp(int[] domain, int mapSize, int regionCount, int? seed = null)
        {
            var colorMapCsp = new MapColoringCsp(mapSize)
            { RandomGenerator = seed is null ? new Random() : new Random((int)seed) };
            colorMapCsp.ResetRegions(regionCount, domain);

            var solver = new CspSolver<Point, int>(
                new FirstUnassigned<Point, int>(),
                new OriginalOrder<Point, int>());

            var benchResult = BenchmarkCsp.Benchmark(solver, colorMapCsp);
            benchResult.Report();

            if (benchResult.FirstSolution is null) return;
            MapCspSerializer.SerializeResultsToJson(benchResult.FirstSolution, colorMapCsp);
            MapCspSerializer.GenerateImage();
        }

        internal static void EinsteinCsp()
        {
            var einstein = new EinsteinCsp();
            var solver = new CspSolver<EinsteinValue, House>(
                new FirstUnassigned<EinsteinValue, House>(),
                new OriginalOrder<EinsteinValue, House>());

            var benchResult = BenchmarkCsp.Benchmark(solver, einstein);

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