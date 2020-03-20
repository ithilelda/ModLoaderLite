using System;
using System.Collections.Generic;
using System.Linq;

namespace ModLoaderLite.Patterns
{
    public class Arrangement : IEquatable<Arrangement>
    {
        public string Product { get; set; }
        public int Priority { get; set; }
        public List<PatternNode> Reactants { get; set; } = new List<PatternNode>();

        public bool Equals(Arrangement other)
        {
            if (other == null || other.Reactants == null || Reactants == null) return false;
            else if (other.Reactants.Count != Reactants.Count) return false;
            else return Reactants.SequenceEqual(other.Reactants);
        }
        public override bool Equals(object obj)
        {
            if (obj is Arrangement a) return Equals(a);
            else return false;
        }

        public override int GetHashCode()
        {
            return Reactants.GetHashCode();
        }
    }
}
