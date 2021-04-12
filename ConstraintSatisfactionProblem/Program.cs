using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using ConstraintSatisfactionProblem.Problems.Einstein;
using ConstraintSatisfactionProblem.Problems.MapColoring;
using ConstraintSatisfactionProblem.Utils.Types;

namespace ConstraintSatisfactionProblem
{
    class Programs
    {
        static void Main(string[] args)
        {
            // MapCsp(null, new[] { 1, 2, 3 }, 16, 6);
            EinsteinCsp();
        }

        internal static void SerializeResultsToJson(Dictionary<Point, int> result, MapColoringCsp mapColoringCsp)
        {
            var res = result.Select(r => new { point = r.Key, color = r.Value }).ToArray();
            var regs = res.Select(r => r.point).ToList();
            var data = new
            {
                board = mapColoringCsp.MapSize,
                regions = res.Select(r => r.point.ToArray()),
                colors = res.Select(r => r.color),
                connections = res
                    .Select(
                        r =>
                            mapColoringCsp.RegionsToSerialize.First(x => x.Point == r.point)
                                .Neighbors.Select(n => regs.IndexOf(n.Point)).ToArray())
                    .ToArray()
            };

            var jsonData = JsonSerializer.Serialize(data);
            File.WriteAllText("out.json", jsonData);
        }

        internal static void GenerateImage()
        {
            var info = new ProcessStartInfo
            {
                FileName = "python",
                UseShellExecute = false,
                Arguments = "generator.py out.json"
            };

            var process = Process.Start(info);
            process?.WaitForExit(5000);
        }

        internal static void MapCsp(int? seed, int[] domain, int mapSize, int regionCount)
        {
            var colorMapCsp = new MapColoringCsp(mapSize) { RandomGenerator = seed is null ? new Random() : new Random(seed ?? 0) };
            colorMapCsp.ResetRegions(regionCount, domain);

            var results = CspSolver.BacktrackingSearch(colorMapCsp);

            Console.WriteLine($"Found: {results.Count} results!");
            foreach (var dictionary in results.Take(5))
            {
                foreach (var (key, value) in dictionary)
                {
                    Console.Write($"{key}={value},");
                }
                Console.WriteLine();
            }

            if (results.Count <= 0) return;
            SerializeResultsToJson(results.First(), colorMapCsp);
            GenerateImage();
        }

        internal static void EinsteinCsp()
        {
            var einstein = new EinsteinCsp();
            var result = CspSolver.BacktrackingSearch(einstein).First();

            foreach (var house in HouseExtension.Houses)
            {
                Console.WriteLine("_____________________");
                Console.WriteLine(house);
                Console.WriteLine("_____________________");
                foreach (var einsteinValue in result
                    .Where(r => r.Value == house)
                    .Select(r => r.Key))
                {
                    Console.WriteLine(einsteinValue);
                }
                Console.WriteLine("\n\n");
            }
        }
    }
}
