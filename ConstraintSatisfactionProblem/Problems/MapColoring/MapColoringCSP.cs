using System;
using System.Collections.Generic;
using System.Linq;
using ConstraintSatisfactionProblem.Utils.Interfaces;
using ConstraintSatisfactionProblem.Utils.Types;

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
        public RegionVariable(Point key, IList<int> domain, CspProblem<Point, int> problem) : base(key, domain, problem)
        {
        }

        public override IList<int> OrderDomainValues()
        {
            return Domain;
        }
    }

    public class RegionConstraint : IConstraint
    {
        public Variable<Point, int> RegionOne { get; }
        public Variable<Point, int> RegionTwo { get; }
        public bool Evaluate()
        {
            if (!RegionOne.Assigned || !RegionTwo.Assigned) return true;
            return RegionOne.Value != RegionTwo.Value;
        }

        public RegionConstraint(Variable<Point, int> regionOne, Variable<Point, int> regionTwo)
        {
            RegionOne = regionOne;
            RegionTwo = regionTwo;
        }

        public RegionConstraint(IList<Variable<Point, int>> subset)
        {
            RegionOne = subset[0];
            RegionTwo = subset[1];
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

        public override Variable<Point, int> NextUnassigned()
        {
            return Variables.First(v => !v.Assigned);
        }

        public void ResetRegions(int regionCount, ICollection<int> domain)
        {
            if (MapSize * MapSize < regionCount)
                throw new ArgumentOutOfRangeException(nameof(regionCount));

            var lines = new LinkedList<Line>();
            var (vars, constraints) = GenerateRegions();
            Variables = vars;
            Constraints = constraints;

            Point DrawPoint()
            {
                var x = RandomGenerator.Next(0, MapSize);
                var y = RandomGenerator.Next(0, MapSize);
                return new Point(x, y);
            }

            (List<Variable<Point, int>>, List<IConstraint>) GenerateRegions()
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

                var tmpConstraints = lines
                    .Select(line => regionVariables.Where(v => v.Key == line.StartPoint || v.Key == line.EndPoint))
                    .Select(subset => new RegionConstraint(subset.ToArray())).Cast<IConstraint>().ToList();

                RegionsToSerialize = tmpRegions;
                return (regionVariables, tmpConstraints);
            }
        }
    }
}