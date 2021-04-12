using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstraintSatisfactionProblem
{
    public abstract class Variable<TK, TD>
    {
        public TK Key { get; set; }
        public TD Value { get; set; }
        public IList<TD> Domain { get; set; }
        public CspProblem<TK, TD> Problem { get; private set; }
        public bool Assigned { get; set; }
        public abstract IList<TD> OrderDomainValues();

        protected Variable(TK key, IList<TD> domain, CspProblem<TK, TD> problem)
        {
            Key = key;
            Domain = domain;
            Problem = problem;
        }
    }

    public interface IConstraint
    {
        bool Evaluate();
    }

    public abstract class CspProblem<TK, TD>
    {
        public IList<Variable<TK, TD>> Variables { get; set; }
        public IList<IConstraint> Constraints { get; set; }
        public Dictionary<TK, TD> Assignment => Variables.ToDictionary(v => v.Key, v => v.Value);
        public bool Consistent => Constraints.All(c => c.Evaluate());
        public bool Complete => Variables.All(v => v.Assigned);
        public abstract Variable<TK, TD> NextUnassigned();
    }
}