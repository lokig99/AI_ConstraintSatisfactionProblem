using System;

namespace ConstraintSatisfactionProblem.Utils.Types
{
    public class Line
    {
        private readonly int _x1, _x2, _y1, _y2;
        public bool IsVertical => _x1 == _x2;
        public (double A, double B) Coefficients { get; set; }

        public Point StartPoint { get; private set; }
        public Point EndPoint { get; private set; }

        public Line(Point start, Point end)
        {
            (_x1, _x2) = start.X < end.X ? (start.X, end.X) : (end.X, start.X);
            (_y1, _y2) = start.Y < end.Y ? (start.Y, end.Y) : (end.Y, start.Y);
            Coefficients = CalculateCoefficients(start, end);
            StartPoint = start;
            EndPoint = end;
        }

        public bool Intersects(Line other)
        {
            const double epsilon = 0.00001;
            var (sa, sb) = Coefficients;
            var (oa, ob) = other.Coefficients;
            if (IsVertical)
            {
                if (other.IsVertical) return _x1 == other._x1 && !(other._y1 >= _y2 || other._y2 <= _y1);
                return _x1 > other._x1 && _x1 < other._x2;
            }

            if (other.IsVertical)
            {
                var y = Math.Round(sa * other._x1 + sb, 2);
                return (other._x1 > _x1 && other._x1 < _x2) && (y > other._y1 && y < other._y2);
            }

            if (Math.Abs(sa - oa) < epsilon) // are parallel
            {
                if (Math.Abs(sb - ob) > epsilon) return false;
                return !(other._x1 >= _x2 || other._x2 <= _x1);
            }

            var x = Math.Round((ob - sb) / (sa - oa), 2);
            return (x > other._x1 && x < other._x2) && (x > _x1 && x < _x2);
        }


        private (double A, double B) CalculateCoefficients(Point start, Point end)
        {
            if (IsVertical) return (0d, 0d);
            var a = (double)(end.Y - start.Y) / (end.X - start.X);
            return (a, start.Y - a * start.X);
        }
    }
}