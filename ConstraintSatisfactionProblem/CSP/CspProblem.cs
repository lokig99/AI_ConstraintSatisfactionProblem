using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;

namespace ConstraintSatisfactionProblem.CSP
{
    public abstract class Variable<TK, TD>
    {
        public TK Key { get; }
        private TD _value;

        public TD Value
        {
            get => _value;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                if (!Domain.Contains(value)) throw new ArgumentOutOfRangeException(nameof(value));

                _value = value;
                Assigned = true;
            }
        }

        public BitArray DomainMask { get; private set; }
        private LinkedList<BitArray> DomainMaskHistory { get; }
        public CspProblem<TK, TD> Problem { get; }
        public IEnumerable<TD> Domain => Problem.GlobalDomain.Where((_, i) => DomainMask[i]);
        public bool Assigned { get; private set; }
        public IList<(BinaryConstraint<TK, TD> c, Variable<TK, TD> v)> Constraints { get; }
        public bool Consistent => Constraints.All(cv => cv.c.Evaluate());

        public bool CheckConsistency(TD testedValue)
        {
            bool result;
            if (Assigned)
            {
                var prev = Value;
                Value = testedValue;
                result = Constraints.All(cv => cv.c.Evaluate());
                Value = prev;
            }
            else
            {
                Value = testedValue;
                result = Constraints.All(cv => cv.c.Evaluate());
                Clear();
            }

            return result;
        }

        /** this method does not change the current value **/
        public int CountConsistent(TD testedValue)
        {
            int result;
            if (Assigned)
            {
                var prev = Value;
                Value = testedValue;
                result = Constraints.Sum(cv => cv.c.Evaluate() ? 1 : 0);
                Value = prev;
            }
            else
            {
                Value = testedValue;
                result = Constraints.Sum(cv => cv.c.Evaluate() ? 1 : 0);
                Clear();
            }

            return result;
        }

        public Variable<TK, TD> SetValue(TD value)
        {
            Value = value;
            return this;
        }

        public IEnumerable<TD> OrderDomainValues(IOrderDomainHeuristic<TK, TD> heuristic)
        {
            return heuristic.DomainOrdered(this);
        }

        public Variable<TK, TD> Clear()
        {
            Assigned = false;
            if (DomainMaskHistory.Count == 0) return this;

            DomainMask = DomainMaskHistory.First?.Value;
            DomainMaskHistory.Clear();
            return this;
        }

        public Variable<TK, TD> RemoveFromDomain(params TD[] values)
        {
            if (values.Length == 0)
            {
                DomainMaskHistory.AddLast(DomainMask);
                return this;
            }

            DomainMaskHistory.AddLast(DomainMask.Clone() as BitArray);
            var newMask = Problem.GlobalDomain
                .Take(DomainMask.Count)
                .Select(d => !values.Contains(d))
                .ToArray();

            DomainMask = DomainMask.And(new BitArray(newMask));
            return this;
        }

        public bool RestorePreviousDomain()
        {
            if (DomainMaskHistory.Count == 0) return false;
            DomainMask = DomainMaskHistory.Last?.Value;
            DomainMaskHistory.RemoveLast();
            return true;
        }

        protected Variable(TK key, ICollection<TD> domain, CspProblem<TK, TD> problem)
        {
            Key = key;
            Problem = problem;
            Constraints = new List<(BinaryConstraint<TK, TD> c, Variable<TK, TD> v)>();
            DomainMaskHistory = new LinkedList<BitArray>();

            foreach (var d in domain)
            {
                if (Problem.GlobalDomain.Contains(d)) continue;
                Problem.GlobalDomain.Add(d);
            }

            var mask = Problem.GlobalDomain.Select(domain.Contains).ToArray();
            DomainMask = new BitArray(mask);
        }
    }

    public abstract class Constraint<TK, TD>
    {
        public abstract bool Evaluate();
    }

    public abstract class BinaryConstraint<TK, TD> : Constraint<TK, TD>
    {
        public Variable<TK, TD> VariableOne { get; }
        public Variable<TK, TD> VariableTwo { get; }

        protected BinaryConstraint(Variable<TK, TD> var1, Variable<TK, TD> var2)
        {
            VariableOne = var1;
            VariableTwo = var2;

            VariableOne.Constraints.Add((this, VariableTwo));
            VariableTwo.Constraints.Add((this, VariableOne));
        }

        protected abstract bool Test();

        public sealed override bool Evaluate()
        {
            if (!VariableOne.Domain.Any() || !VariableTwo.Domain.Any()) return false;
            if (!VariableOne.Assigned || !VariableTwo.Assigned) return true;
            return Test();
        }
    }

    public abstract class CspProblem<TK, TD>
    {
        protected CspProblem()
        {
            GlobalDomain = new List<TD>();
        }

        public IList<Variable<TK, TD>> Variables { get; set; }
        public IEnumerable<Variable<TK, TD>> UnassignedVariables => Variables.Where(v => !v.Assigned);
        public IList<BinaryConstraint<TK, TD>> Constraints { get; set; }
        public Dictionary<TK, TD> Assignment => Variables.ToDictionary(v => v.Key, v => v.Value);
        public IList<TD> GlobalDomain { get; }
        public bool Consistent => Constraints.All(c => c.Evaluate());
        public bool Complete => Variables.All(v => v.Assigned);

        public Variable<TK, TD> NextUnassigned(ISelectVariableHeuristic<TK, TD> heuristic)
        {
            return heuristic.SelectVariable(this);
        }

        public void ClearAll()
        {
            foreach (var variable in Variables)
            {
                variable.Clear();
            }
        }
    }
}