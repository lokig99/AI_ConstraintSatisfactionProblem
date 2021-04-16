using System.Collections.Generic;
using System.Linq;
using ConstraintSatisfactionProblem.CSP;

namespace ConstraintSatisfactionProblem.Problems.Einstein
{
    public enum EinsteinConstraintType
    {
        UniqueHouse, InTheSameHouse, HouseOnTheLeft, HouseNextTo
    }

    public class UniqueHouse : BinaryConstraint<EinsteinValue, House>
    {
        public override bool Test(House valueOfOne, House valueOfTwo)
        {
            return valueOfOne != valueOfTwo;
        }

        public UniqueHouse(Variable<EinsteinValue, House> var1, Variable<EinsteinValue, House> var2) :
            base(var1, var2)
        {
        }
    }

    public class InTheSameHouse : BinaryConstraint<EinsteinValue, House>
    {
        public override bool Test(House valueOfOne, House valueOfTwo)
        {
            return valueOfOne == valueOfTwo;
        }

        public InTheSameHouse(Variable<EinsteinValue, House> var1, Variable<EinsteinValue, House> var2) :
            base(var1, var2)
        {
        }
    }

    public class HouseOnTheLeft : BinaryConstraint<EinsteinValue, House>
    {
        public HouseOnTheLeft(Variable<EinsteinValue, House> var, Variable<EinsteinValue, House> varOnLeft) :
            base(var, varOnLeft)
        {
        }

        public override bool Test(House valueOfOne, House valueOfTwo)
        {
            return valueOfOne.HouseOnItsLeft() == valueOfTwo;
        }
    }

    public class HouseOnTheRight : BinaryConstraint<EinsteinValue, House>
    {
        public HouseOnTheRight(Variable<EinsteinValue, House> var, Variable<EinsteinValue, House> varOnRight) :
            base(var, varOnRight)
        {
        }

        public override bool Test(House valueOfOne, House valueOfTwo)
        {
            return valueOfOne.HouseOnItsRight() == valueOfTwo;
        }
    }

    public class HouseNextTo : BinaryConstraint<EinsteinValue, House>
    {
        public HouseNextTo(Variable<EinsteinValue, House> var1, Variable<EinsteinValue, House> var2) :
            base(var1, var2)
        {
        }

        public override bool Test(House valueOfOne, House valueOfTwo)
        {
            return valueOfOne.NextTo().Contains(valueOfTwo);
        }
    }
}