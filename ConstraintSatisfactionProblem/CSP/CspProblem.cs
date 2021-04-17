using ConstraintSatisfactionProblem.CSP.Heuristics.OrderDomain;
using ConstraintSatisfactionProblem.CSP.Heuristics.SelectVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        public LinkedList<BitArray> DomainMaskHistory { get; }
        public CspProblem<TK, TD> Problem { get; }
        private HashSet<TD> _cachedDomain;
        public IEnumerable<TD> Domain => Assigned ? new[] { Value } : _cachedDomain;
        public bool Assigned { get; set; }

        public IList<BinaryConstraint<TK, TD>> Constraints { get; }
        public bool Consistent => Constraints.All(c => c.Evaluate());

        public bool CheckConsistency(TD testedValue)
        {
            var prev = _value;
            var prevAssigned = Assigned;

            _value = testedValue;
            Assigned = true;
            var result = Constraints.All(c => c.Evaluate());
            _value = prev;
            Assigned = prevAssigned;
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
            _cachedDomain = Problem.GlobalDomain.Where((_, i) => DomainMask[i]).ToHashSet();
            return this;
        }

        public Variable<TK, TD> RemoveFromDomain(ICollection<TD> values)
        {
            DomainMaskHistory.AddLast(DomainMask.Clone() as BitArray);

            for (var i = 0; i < DomainMask.Count; i++)
            {
                if (!values.Contains(Problem.GlobalDomain[i])) continue;
                DomainMask.Set(i, false);
                _cachedDomain.Remove(Problem.GlobalDomain[i]);
            }

            return this;
        }

        public bool RestorePreviousDomain()
        {
            if (DomainMaskHistory.Count == 0) return false;

            Debug.Assert(DomainMaskHistory.Last != null, "DomainMaskHistory.Last != null");
            var prevDomainDiff = DomainMaskHistory.Last.Value.Xor(DomainMask);
            DomainMaskHistory.RemoveLast();

            for (var i = 0; i < prevDomainDiff.Count; i++)
            {
                if (!prevDomainDiff[i]) continue;
                DomainMask.Set(i, true);
                _cachedDomain.Add(Problem.GlobalDomain[i]);
            }

            return true;
        }

        public bool RestorePreviousDomain(int offset)
        {
            if (offset < 0) return false;

            for (var i = 0; i < offset; i++)
            {
                DomainMaskHistory.RemoveLast();
            }

            return RestorePreviousDomain();
        }

        protected Variable(TK key, ICollection<TD> domain, CspProblem<TK, TD> problem)
        {
            Key = key;
            Problem = problem;
            Constraints = new List<BinaryConstraint<TK, TD>>();
            DomainMaskHistory = new LinkedList<BitArray>();

            foreach (var d in domain)
            {
                if (Problem.GlobalDomain.Contains(d)) continue;
                Problem.GlobalDomain.Add(d);
            }

            var mask = new bool[Problem.GlobalDomain.Count];
            _cachedDomain = new HashSet<TD>();
            for (var i = 0; i < Problem.GlobalDomain.Count; i++)
            {
                var d = Problem.GlobalDomain[i];
                mask[i] = domain.Contains(d);
                if (mask[i]) _cachedDomain.Add(d);
            }

            DomainMask = new BitArray(mask);
            Assigned = false;
            problem.Variables.Add(this);
        }

        public override string ToString()
        {
            return $"{Key} => {(Assigned ? Value : null)}";
        }
    }

    public abstract class Constraint
    {
        public abstract bool Evaluate();
    }

    public class BinaryConstraint<TK, TD> : Constraint
    {
        public Variable<TK, TD> VariableOne { get; }
        public Variable<TK, TD> VariableTwo { get; }

        // directed constraint: var1 -> var2
        protected BinaryConstraint(Variable<TK, TD> var1, Variable<TK, TD> var2)
        {
            VariableOne = var1;
            VariableTwo = var2;

            VariableOne.Constraints.Add(this);
        }

        public bool Test()
        {
            return Test(VariableOne.Value, VariableTwo.Value);
        }

        public virtual bool Test(TD valueOfOne, TD valueOfTwo)
        {
            throw new NotImplementedException(nameof(Test));
        }

        public sealed override bool Evaluate()
        {
            if (!VariableOne.Domain.Any() || !VariableTwo.Domain.Any()) return false;
            if (!VariableOne.Assigned || !VariableTwo.Assigned) return true;
            return Test();
        }

        public override string ToString()
        {
            return $"({VariableOne}) => ({VariableTwo})";
        }
    }

    public abstract class CspProblem<TK, TD>
    {
        protected CspProblem()
        {
            GlobalDomain = new List<TD>();
            Variables = new List<Variable<TK, TD>>();
        }

        public IList<Variable<TK, TD>> Variables { get; }
        public IEnumerable<Variable<TK, TD>> UnassignedVariables => Variables.Where(v => !v.Assigned);
        public IEnumerable<BinaryConstraint<TK, TD>> Constraints => Variables.SelectMany(v => v.Constraints);
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