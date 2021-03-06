using ConstraintSatisfactionProblem.CSP;
using ConstraintSatisfactionProblem.Utils.Interfaces;
using ConstraintSatisfactionProblem.Utils.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstraintSatisfactionProblem.Problems.MapColoring
{
    public class Region
    {
        public Point Point { get; set; }
        public LinkedList<Region> Neighbors { get; set; } = new();

        public Region(Point point)
        {
            Point = point;
        }
    }

    public class RegionVariable : Variable<Point, int>
    {
        public RegionVariable(Point key, ICollection<int> domain, CspProblem<Point, int> problem) : base(key, domain, problem)
        {
        }
    }

    public class RegionConstraint : BinaryConstraint<Point, int>
    {
        public override bool Test(int valueOfOne, int valueOfTwo)
        {
            return valueOfOne != valueOfTwo;
        }

        public RegionConstraint(Variable<Point, int> var1, Variable<Point, int> var2) : base(var1, var2)
        {
        }
    }

    public class MapColoringCsp : CspProblem<Point, int>, IRandom
    {
        public int MapSize { get; set; }
        public Random RandomGenerator { get; set; } = new();

        public MapColoringCsp(int mapSize)
        {
            MapSize = mapSize;
        }

        public List<Region> RegionsToSerialize { get; set; }

        public void ResetRegions(int regionCount, ICollection<int> domain)
        {
            if (MapSize * MapSize < regionCount)
                throw new ArgumentOutOfRangeException(nameof(regionCount));

            var lines = new LinkedList<Line>();
            GenerateRegions();

            Point DrawPoint()
            {
                var x = RandomGenerator.Next(0, MapSize);
                var y = RandomGenerator.Next(0, MapSize);
                return new Point(x, y);
            }


            void GenerateRegions()
            {
                var regions = new List<(Region region, LinkedList<Region> others)>(regionCount);
                var tmpRegions = new List<Region>(regionCount);

                // create regions
                var tmp = new HashSet<Point>();
                while (tmp.Count < regionCount)
                {
                    var point = DrawPoint();
                    while (tmp.Contains(point))
                    {
                        point = DrawPoint();
                    }

                    tmp.Add(point);
                    tmpRegions.Add(new Region(point));
                }

                regions.AddRange(from region in tmpRegions
                                 let tmpOthers = tmpRegions
                                     .Where(r => r != region)
                                     .OrderBy(r => Point.Distance(r.Point, region.Point))
                                 select (region, new LinkedList<Region>(tmpOthers)));


                // generate region constraints
                while (regions.Count > 0)
                {
                    var index = RandomGenerator.Next(0, regions.Count);
                    var (reg, nbrs) = regions[index];
                    var nbr = nbrs.First?.Value;
                    nbrs.RemoveFirst();

                    if (nbr is not null && !reg.Neighbors.Contains(nbr))
                    {
                        var line = new Line(reg.Point, nbr.Point);
                        if (lines.All(l => !line.Intersects(l)))
                        {
                            lines.AddLast(line);
                            reg.Neighbors.AddLast(nbr);
                            nbr.Neighbors.AddLast(reg);
                        }
                    }

                    if (nbrs.Count == 0)
                    {
                        regions.RemoveAt(index);
                    }
                }

                var regionVariables = new List<Variable<Point, int>>();

                foreach (var region in tmpRegions)
                {
                    var tmpDomain = new int[domain.Count];
                    domain.CopyTo(tmpDomain, 0);
                    regionVariables.Add(new RegionVariable(region.Point, tmpDomain, this));
                }

                _ = lines
                    .Select(line => new
                    {
                        first = regionVariables.First(v => v.Key == line.StartPoint),
                        second = regionVariables.First(v => v.Key == line.EndPoint)
                    })
                    .Select(pair =>
                        MapColoringBidirectionalConstraintFactory.CreateBidirectionalConstraints(pair.first,
                            pair.second)).ToArray();

                RegionsToSerialize = tmpRegions;

            }
        }
    }
}