using System.Collections.Generic;
using System.Linq;
using ConstraintSatisfactionProblem.CSP;

namespace ConstraintSatisfactionProblem.Problems.Einstein
{
    public class UniqueHouse : BinaryConstraint<EinsteinValue, House>
    {
        protected override bool Test()
        {
            return VariableOne.Value != VariableTwo.Value;
        }

        public UniqueHouse(Variable<EinsteinValue, House> var1, Variable<EinsteinValue, House> var2) :
            base(var1, var2)
        {
        }
    }

    public class InTheSameHouse : BinaryConstraint<EinsteinValue, House>
    {
        protected override bool Test()
        {
            return VariableOne.Value == VariableTwo.Value;
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

        protected override bool Test()
        {
            return VariableOne.Value.HouseOnItsLeft() == VariableTwo.Value;
        }
    }

    public class HouseNextTo : BinaryConstraint<EinsteinValue, House>
    {
        public HouseNextTo(Variable<EinsteinValue, House> var1, Variable<EinsteinValue, House> var2) :
            base(var1, var2)
        {
        }

        protected override bool Test()
        {
            return VariableOne.Value.NextTo().Contains(VariableTwo.Value);
        }
    }
}