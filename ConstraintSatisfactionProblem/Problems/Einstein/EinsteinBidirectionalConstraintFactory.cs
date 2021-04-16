using System;
using ConstraintSatisfactionProblem.CSP;

namespace ConstraintSatisfactionProblem.Problems.Einstein
{
    public static class EinsteinBidirectionalConstraintFactory
    {
        public static (BinaryConstraint<EinsteinValue, House> bc1, BinaryConstraint<EinsteinValue, House> bc2)
            CreateBidirectionalConstraints(
                Variable<EinsteinValue, House> var1, Variable<EinsteinValue, House> var2,
                EinsteinConstraintType constraintType)
        {
            // constraint var1 -> var2
            var bc1 = constraintType switch
            {
                EinsteinConstraintType.UniqueHouse =>
                    new UniqueHouse(var1, var2) as BinaryConstraint<EinsteinValue, House>,
                EinsteinConstraintType.HouseNextTo =>
                    new HouseNextTo(var1, var2) as BinaryConstraint<EinsteinValue, House>,
                EinsteinConstraintType.HouseOnTheLeft =>
                    new HouseOnTheLeft(var1, var2) as BinaryConstraint<EinsteinValue, House>,
                EinsteinConstraintType.InTheSameHouse =>
                    new InTheSameHouse(var1, var2) as BinaryConstraint<EinsteinValue, House>,
                _ => throw new ArgumentOutOfRangeException(nameof(constraintType), constraintType, null)
            };

            // constraint var2 -> var1
            var bc2 = constraintType switch
            {
                EinsteinConstraintType.UniqueHouse =>
                    new UniqueHouse(var2, var1) as BinaryConstraint<EinsteinValue, House>,
                EinsteinConstraintType.HouseNextTo =>
                    new HouseNextTo(var2, var1) as BinaryConstraint<EinsteinValue, House>,
                EinsteinConstraintType.HouseOnTheLeft =>
                    new HouseOnTheLeft(var2, var1) as BinaryConstraint<EinsteinValue, House>,
                EinsteinConstraintType.InTheSameHouse =>
                    new InTheSameHouse(var2, var1) as BinaryConstraint<EinsteinValue, House>,
                _ => throw new ArgumentOutOfRangeException(nameof(constraintType), constraintType, null)
            };

            return (bc1, bc2);
        }
    }
}