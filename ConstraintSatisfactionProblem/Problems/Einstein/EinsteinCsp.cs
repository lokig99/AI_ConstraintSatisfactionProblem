using System.Collections.Generic;
using System.Linq;
using ConstraintSatisfactionProblem.CSP;
using ConstraintSatisfactionProblem.Utils.Maths;

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
                new HouseOnTheLeft(white, green), //3rd
                new InTheSameHouse(dane, tea), //4th
                new HouseNextTo(lights, cat), //5th
                new InTheSameHouse(yellow, cigar), //6th
                new InTheSameHouse(german, pipe), //7th
                new HouseNextTo(lights, water), //9th
                new InTheSameHouse(noFilter, bird), //10th
                new InTheSameHouse(swede, dog), //11th
                new HouseNextTo(norwegian, blue), //12th
                new HouseNextTo(horse, yellow), //13th
                new InTheSameHouse(menthol, beer), //14th
                new InTheSameHouse(green, coffee) //15th
            };

            // unique house constraints
            var c = Constraints as List<BinaryConstraint<EinsteinValue, House>>;

            // colors
            c?.AddRange(MathCsp.Combinations(yellow, blue, red, green, white)
                .Select(pair => new UniqueHouse(pair.Item1, pair.Item2)));
            // nationalities
            c?.AddRange(MathCsp.Combinations(german, norwegian, englishman, swede, dane)
                .Select(pair => new UniqueHouse(pair.Item1, pair.Item2)));
            // drinks
            c?.AddRange(MathCsp.Combinations(milk, water, beer, coffee, tea)
                .Select(pair => new UniqueHouse(pair.Item1, pair.Item2)));
            // pets
            c?.AddRange(MathCsp.Combinations(cat, dog, horse, fish, bird)
                .Select(pair => new UniqueHouse(pair.Item1, pair.Item2)));
            // smokes
            c?.AddRange(MathCsp.Combinations(noFilter, menthol, pipe, cigar, lights)
                .Select(pair => new UniqueHouse(pair.Item1, pair.Item2)));
        }
    }
}