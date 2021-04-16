using System;
using System.Collections.Generic;

namespace ConstraintSatisfactionProblem.Problems.Einstein
{
    public enum EinsteinValue
    {
        Yellow,
        Blue,
        Red,
        White,
        Green,
        Norwegian,
        Dane,
        Englishman,
        German,
        Swede,
        Water,
        Tea,
        Milk,
        Beer,
        Coffee,
        Cigar,
        Lights,
        NoFilter,
        Pipe,
        Menthol,
        Cat,
        Horse,
        Bird,
        Dog,
        Fish
    }

    public enum EinsteinValueType
    {
        Color,
        Nationality,
        Drink,
        Smoke,
        Pet
    }

    public static class EinsteinValueExtension
    {
        public static EinsteinValueType GetValueType(this EinsteinValue einsteinValue)
        {
            return einsteinValue switch
            {
                EinsteinValue.Yellow or EinsteinValue.Blue or EinsteinValue.Yellow or EinsteinValue.White or
                    EinsteinValue.Green or
                    EinsteinValue.Red => EinsteinValueType.Color,
                EinsteinValue.Norwegian or EinsteinValue.Dane or EinsteinValue.Englishman or EinsteinValue.German or
                    EinsteinValue.Swede => EinsteinValueType.Nationality,
                EinsteinValue.Water or EinsteinValue.Tea or EinsteinValue.Milk or EinsteinValue.Beer or EinsteinValue
                        .Coffee =>
                    EinsteinValueType.Drink,
                EinsteinValue.Cigar or EinsteinValue.Lights or EinsteinValue.NoFilter or EinsteinValue.Pipe or
                    EinsteinValue.Menthol => EinsteinValueType.Smoke,
                _ => EinsteinValueType.Pet
            };
        }

        public static readonly IReadOnlyList<EinsteinValue> Colors = new[]
        {
            EinsteinValue.Yellow,
            EinsteinValue.Blue,
            EinsteinValue.Red,
            EinsteinValue.White,
            EinsteinValue.Green
        };

        public static readonly IReadOnlyList<EinsteinValue> Nationalities = new[]
        {
            EinsteinValue.Norwegian,
            EinsteinValue.Dane,
            EinsteinValue.Englishman,
            EinsteinValue.German,
            EinsteinValue.Swede
        };

        public static readonly IReadOnlyList<EinsteinValue> Drinks = new[]
        {
            EinsteinValue.Water,
            EinsteinValue.Tea,
            EinsteinValue.Milk,
            EinsteinValue.Beer,
            EinsteinValue.Coffee
        };

        public static readonly IReadOnlyList<EinsteinValue> Smokes = new[]
        {
            EinsteinValue.Cigar,
            EinsteinValue.Lights,
            EinsteinValue.NoFilter,
            EinsteinValue.Pipe,
            EinsteinValue.Menthol
        };

        public static readonly IReadOnlyList<EinsteinValue> Pets = new[]
        {
            EinsteinValue.Cat,
            EinsteinValue.Horse,
            EinsteinValue.Bird,
            EinsteinValue.Dog,
            EinsteinValue.Fish
        };

        public static readonly IReadOnlyList<IReadOnlyList<EinsteinValue>> AllValues = new[]
        {
            Colors,
            Nationalities,
            Drinks,
            Smokes,
            Pets
        };
    }

    public enum House
    {
        First,
        Second,
        Third,
        Fourth,
        Fifth
    }

    public static class HouseExtension
    {
        public static List<House> NextTo(this House house)
        {
            var near = new List<House>();
            var left = house - 1;
            var right = house + 1;

            if (Enum.IsDefined(typeof(House), left)) near.Add(left);
            if (Enum.IsDefined(typeof(House), right)) near.Add(right);
            return near;
        }

        public static House? HouseOnItsLeft(this House house)
        {
            var left = house - 1;
            return Enum.IsDefined(typeof(House), left) ? left : null;
        }

        public static House? HouseOnItsRight(this House house)
        {
            var right = house + 1;
            return Enum.IsDefined(typeof(House), right) ? right : null;
        }

        public static readonly IReadOnlyList<House> Houses = new[]
        {
            House.First, House.Second, House.Third, House.Fourth, House.Fifth
        };
    }
}