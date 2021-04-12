using System.Collections.Generic;
using System.Linq;

namespace ConstraintSatisfactionProblem.Problems.Einstein
{
    public class UniqueHouse : IConstraint
    {
        private readonly Dictionary<House, int> _houseCounter;
        public IList<Variable<EinsteinValue, House>> Houses { get; }

        public UniqueHouse(IList<Variable<EinsteinValue, House>> subset)
        {
            Houses = subset;
            _houseCounter = new Dictionary<House, int>
            {
                {House.First, 0},
                {House.Second, 0},
                {House.Third, 0},
                {House.Fourth, 0},
                {House.Fifth, 0},
            };
        }

        private void ResetCounter()
        {
            foreach (var (house, count) in _houseCounter)
            {
                _houseCounter[house] = 0;
            }
        }

        public bool Evaluate()
        {
            ResetCounter();
            foreach (var variable in Houses.Where(v => v.Assigned))
            {
                _houseCounter[variable.Value] = _houseCounter.GetValueOrDefault(variable.Value, 0) + 1;
            }

            return _houseCounter.All(hc => hc.Value < 2);
        }
    }

    public class ValueInHouse : IConstraint
    {
        public Variable<EinsteinValue, House> Variable { get; }
        public House ExpectedHouse { get; }

        public ValueInHouse(Variable<EinsteinValue, House> variable, House expectedHouse)
        {
            Variable = variable;
            ExpectedHouse = expectedHouse;
        }

        public bool Evaluate()
        {
            if (!Variable.Assigned) return true;
            return Variable.Value == ExpectedHouse;
        }
    }

    public class InTheSameHouse : IConstraint
    {
        public Variable<EinsteinValue, House> VariableOne { get; }
        public Variable<EinsteinValue, House> VariableTwo { get; }

        public InTheSameHouse(Variable<EinsteinValue, House> variableOne, Variable<EinsteinValue, House> variableTwo)
        {
            VariableOne = variableOne;
            VariableTwo = variableTwo;
        }

        public bool Evaluate()
        {
            if (!VariableOne.Assigned || !VariableTwo.Assigned) return true;
            return VariableOne.Value == VariableTwo.Value;
        }
    }

    public class HouseOnTheLeft : IConstraint
    {
        public Variable<EinsteinValue, House> VariableOnLeft { get; }
        public Variable<EinsteinValue, House> Variable { get; }

        public HouseOnTheLeft(Variable<EinsteinValue, House> variable, Variable<EinsteinValue, House> variableOnLeft)
        {
            Variable = variable;
            VariableOnLeft = variableOnLeft;
        }

        public bool Evaluate()
        {
            if (!VariableOnLeft.Assigned || !Variable.Assigned) return true;
            return Variable.Value.HouseOnItsLeft() == VariableOnLeft.Value;
        }
    }

    public class HouseNear : IConstraint
    {
        public Variable<EinsteinValue, House> VariableOne { get; }
        public Variable<EinsteinValue, House> VariableTwo { get; }

        public HouseNear(Variable<EinsteinValue, House> variableOne, Variable<EinsteinValue, House> variableTwo)
        {
            VariableOne = variableOne;
            VariableTwo = variableTwo;
        }

        public bool Evaluate()
        {
            if (!VariableOne.Assigned || !VariableTwo.Assigned) return true;
            return VariableOne.Value.NearHouses().Contains(VariableTwo.Value);
        }
    }


}