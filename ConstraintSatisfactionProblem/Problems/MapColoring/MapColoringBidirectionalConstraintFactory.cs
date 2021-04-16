using ConstraintSatisfactionProblem.CSP;
using ConstraintSatisfactionProblem.Utils.Types;

namespace ConstraintSatisfactionProblem.Problems.MapColoring
{
    public class MapColoringBidirectionalConstraintFactory
    {
        public static (BinaryConstraint<Point, int> bc1, BinaryConstraint<Point, int> bc2)
            CreateBidirectionalConstraints(
                Variable<Point, int> var1, Variable<Point, int> var2)
        {
            // constraint var1 -> var2
            var bc1 = new RegionConstraint(var1, var2);

            // constraint var2 -> var1
            var bc2 = new RegionConstraint(var2, var1);

            return (bc1, bc2);
        }
    }
}