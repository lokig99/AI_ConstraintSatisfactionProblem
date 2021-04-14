using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using ConstraintSatisfactionProblem.Problems.MapColoring;
using ConstraintSatisfactionProblem.Utils.Types;

namespace ConstraintSatisfactionProblem.Tools
{
    public static class MapCspSerializer
    {
        public static void SerializeResultsToJson(Dictionary<Point, int> result, MapColoringCsp mapColoringCsp)
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

        public static void GenerateImage()
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
    }
}