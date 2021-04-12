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

        public EinsteinVariable(EinsteinValue key, CspProblem<EinsteinValue, House> problem) : base(key, null, problem)
        {
            Domain = new List<House>() { House.First, House.Second, House.Third, House.Fourth, House.Fifth };
        }

        public override IList<House> OrderDomainValues()
        {
            return Domain;
        }
    }

    public class EinsteinCsp : CspProblem<EinsteinValue, House>
    {
        public override Variable<EinsteinValue, House> NextUnassigned()
        {
            return Variables.First(v => !v.Assigned);
        }

        public EinsteinCsp()
        {
            // setup variables
            var yellow = new EinsteinVariable(EinsteinValue.Yellow, this);
            var blue = new EinsteinVariable(EinsteinValue.Blue, this);
            var red = new EinsteinVariable(EinsteinValue.Red, this);
            var white = new EinsteinVariable(EinsteinValue.White, this);
            var green = new EinsteinVariable(EinsteinValue.Green, this);

            var norwegian = new EinsteinVariable(EinsteinValue.Norwegian, this);
            var dane = new EinsteinVariable(EinsteinValue.Dane, this);
            var englishman = new EinsteinVariable(EinsteinValue.Englishman, this);
            var german = new EinsteinVariable(EinsteinValue.German, this);
            var swede = new EinsteinVariable(EinsteinValue.Swede, this);

            var water = new EinsteinVariable(EinsteinValue.Water, this);
            var tea = new EinsteinVariable(EinsteinValue.Tea, this);
            var milk = new EinsteinVariable(EinsteinValue.Milk, this);
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
                yellow,
                blue,
                red,
                white,
                green,
                norwegian,
                dane,
                englishman,
                german,
                swede,
                water,
                tea,
                milk,
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

            Constraints = new List<IConstraint>
            {
                new UniqueHouse(new List<Variable<EinsteinValue, House>> {yellow, blue, red, white, green}),
                new UniqueHouse(new List<Variable<EinsteinValue, House>> {norwegian, dane, englishman, swede, german}),
                new UniqueHouse(new List<Variable<EinsteinValue, House>> {water, tea, milk, beer, coffee}),
                new UniqueHouse(new List<Variable<EinsteinValue, House>> {cigar, lights, noFilter, pipe, menthol}),
                new UniqueHouse(new List<Variable<EinsteinValue, House>> {cat, horse, bird, dog, fish}),
                // riddle hints
                new ValueInHouse(norwegian, House.First), // 1st 
                new InTheSameHouse(englishman, red), //2nd
                new HouseOnTheLeft(white, green), //3rd
                new InTheSameHouse(dane, tea), //4th
                new HouseNear(lights, cat), //5th
                new InTheSameHouse(yellow, cigar), //6th
                new InTheSameHouse(german, pipe), //7th
                new ValueInHouse(milk, House.Third), //8th
                new HouseNear(lights, water), //9th
                new InTheSameHouse(noFilter, bird), //10th
                new InTheSameHouse(swede, dog), //11th
                new HouseNear(norwegian, blue), //12th
                new HouseNear(horse, yellow), //13th
                new InTheSameHouse(menthol, beer), //14th
                new InTheSameHouse(green, coffee) //15th
            };
        }
    }
}