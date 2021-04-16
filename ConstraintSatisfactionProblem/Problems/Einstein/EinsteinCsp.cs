using ConstraintSatisfactionProblem.CSP;
using ConstraintSatisfactionProblem.Utils.Maths;
using System.Collections.Generic;
using System.Linq;

namespace ConstraintSatisfactionProblem.Problems.Einstein
{
    public class EinsteinVariable : Variable<EinsteinValue, House>
    {
        public EinsteinVariable(EinsteinValue key, IList<House> domain, CspProblem<EinsteinValue, House> problem) :
            base(key,
                domain, problem)
        {
        }

        public EinsteinVariable(EinsteinValue key, CspProblem<EinsteinValue, House> problem) : base(key,
            new List<House>() { House.First, House.Second, House.Third, House.Fourth, House.Fifth }, problem)
        {
        }
    }

    public class EinsteinCsp : CspProblem<EinsteinValue, House>
    {
        public EinsteinCsp()
        {
            // setup variables
            var yellow = new EinsteinVariable(EinsteinValue.Yellow, this);
            var blue = new EinsteinVariable(EinsteinValue.Blue, this);
            var red = new EinsteinVariable(EinsteinValue.Red, this);
            var white = new EinsteinVariable(EinsteinValue.White, this);
            var green = new EinsteinVariable(EinsteinValue.Green, this);

            var norwegian = new EinsteinVariable(EinsteinValue.Norwegian, new List<House> { House.First }, this);
            var dane = new EinsteinVariable(EinsteinValue.Dane, this);
            var englishman = new EinsteinVariable(EinsteinValue.Englishman, this);
            var german = new EinsteinVariable(EinsteinValue.German, this);
            var swede = new EinsteinVariable(EinsteinValue.Swede, this);

            var water = new EinsteinVariable(EinsteinValue.Water, this);
            var tea = new EinsteinVariable(EinsteinValue.Tea, this);
            var milk = new EinsteinVariable(EinsteinValue.Milk, new List<House> { House.Third }, this);
            var beer = new EinsteinVariable(EinsteinValue.Beer, this);
            var coffee = new EinsteinVariable(EinsteinValue.Coffee, this);

            var cigar = new EinsteinVariable(EinsteinValue.Cigar, this);
            var lights = new EinsteinVariable(EinsteinValue.Lights, this);
            var noFilter = new EinsteinVariable(EinsteinValue.NoFilter, this);
            var pipe = new EinsteinVariable(EinsteinValue.Pipe, this);
            var menthol = new EinsteinVariable(EinsteinValue.Menthol, this);

            var cat = new EinsteinVariable(EinsteinValue.Cat, this);
            var horse = new EinsteinVariable(EinsteinValue.Horse, this);
            var bird = new EinsteinVariable(EinsteinValue.Bird, this);
            var dog = new EinsteinVariable(EinsteinValue.Dog, this);
            var fish = new EinsteinVariable(EinsteinValue.Fish, this);

            Variables = new List<Variable<EinsteinValue, House>>
            {
                norwegian,
                milk,
                blue,
                green,
                white,
                yellow,
                red,
                dane,
                englishman,
                german,
                swede,
                water,
                tea,
                beer,
                coffee,
                cigar,
                lights,
                noFilter,
                pipe,
                menthol,
                cat,
                horse,
                bird,
                dog,
                fish,
            };

            // setup riddle constraints
            Constraints = new List<BinaryConstraint<EinsteinValue, House>>
            {
                // riddle hints
                new InTheSameHouse(englishman, red), //2nd
                new InTheSameHouse(red, englishman),

                new HouseOnTheLeft(white, green), //3rd
                new HouseOnTheRight(green, white),

                new InTheSameHouse(dane, tea), //4th
                new InTheSameHouse(tea, dane),

                new HouseNextTo(lights, cat), //5th
                new HouseNextTo(cat, lights),

                new InTheSameHouse(yellow, cigar), //6th
                new InTheSameHouse(cigar, yellow),

                new InTheSameHouse(german, pipe), //7th
                new InTheSameHouse(pipe, german),

                new HouseNextTo(lights, water), //9th
                new HouseNextTo(water, lights),

                new InTheSameHouse(noFilter, bird), //10th
                new InTheSameHouse(bird, noFilter),

                new InTheSameHouse(swede, dog), //11th
                new InTheSameHouse(dog, swede),

                new HouseNextTo(norwegian, blue), //12th
                new HouseNextTo(blue, norwegian),

                new HouseNextTo(horse, yellow), //13th
                new HouseNextTo(yellow, horse),

                new InTheSameHouse(menthol, beer), //14th
                new InTheSameHouse(beer, menthol),

                new InTheSameHouse(green, coffee), //15th
                new InTheSameHouse(coffee, green)
            };

            // unique house constraints
            var c = Constraints as List<BinaryConstraint<EinsteinValue, House>>;

            // colors
            foreach (var (bc1, bc2) in MathCsp.Combinations(yellow, blue, red, green, white)
                .Select(pair =>
                    EinsteinBidirectionalConstraintFactory
                        .CreateBidirectionalConstraints(pair.Item1, pair.Item2, EinsteinConstraintType.UniqueHouse)))
            {
                c?.Add(bc1);
                c?.Add(bc2);
            }

            // nationalities
            foreach (var (bc1, bc2) in MathCsp.Combinations(german, norwegian, englishman, swede, dane)
                .Select(pair =>
                    EinsteinBidirectionalConstraintFactory
                        .CreateBidirectionalConstraints(pair.Item1, pair.Item2, EinsteinConstraintType.UniqueHouse)))
            {
                c?.Add(bc1);
                c?.Add(bc2);
            }

            // drinks
            foreach (var (bc1, bc2) in MathCsp.Combinations(milk, water, beer, coffee, tea)
                .Select(pair =>
                    EinsteinBidirectionalConstraintFactory
                        .CreateBidirectionalConstraints(pair.Item1, pair.Item2, EinsteinConstraintType.UniqueHouse)))
            {
                c?.Add(bc1);
                c?.Add(bc2);
            }

            // pets
            foreach (var (bc1, bc2) in MathCsp.Combinations(cat, dog, horse, fish, bird)
                .Select(pair =>
                    EinsteinBidirectionalConstraintFactory
                        .CreateBidirectionalConstraints(pair.Item1, pair.Item2, EinsteinConstraintType.UniqueHouse)))
            {
                c?.Add(bc1);
                c?.Add(bc2);
            }

            // smokes
            foreach (var (bc1, bc2) in MathCsp.Combinations(noFilter, menthol, pipe, cigar, lights)
                .Select(pair =>
                    EinsteinBidirectionalConstraintFactory
                        .CreateBidirectionalConstraints(pair.Item1, pair.Item2, EinsteinConstraintType.UniqueHouse)))
            {
                c?.Add(bc1);
                c?.Add(bc2);
            }
        }
    }
}