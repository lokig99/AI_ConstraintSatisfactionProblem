using System;
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
        public HashSet<TD> Domain { get; }
        public CspProblem<TK, TD> Problem { get; }

        public bool Assigned { get; private set; }

        public IEnumerable<TD> OrderDomainValues(IOrderDomainHeuristic<TK, TD> heuristic)
        {
            return heuristic.DomainOrdered(this);
        }

        protected Variable(TK key, IEnumerable<TD> domain, CspProblem<TK, TD> problem)
        {
            Key = key;
            Domain = domain.ToHashSet();
            Problem = problem;
        }

        public void Clear()
        {
            Assigned = false;
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
        }

        protected abstract bool Test();

        public sealed override bool Evaluate()
        {
            if (!VariableOne.Assigned || !VariableTwo.Assigned) return true;
            return Test();
        }
    }

    public abstract class CspProblem<TK, TD>
    {
        public IList<Variable<TK, TD>> Variables { get; set; }
        public IList<BinaryConstraint<TK, TD>> Constraints { get; set; }
        public Dictionary<TK, TD> Assignment => Variables.ToDictionary(v => v.Key, v => v.Value);
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