using System;
using System.Collections.Generic;
using System.Linq;

namespace ModLoaderLite.Patterns
{
    public class Combination : IEquatable<Combination>
    {
        public string Product { get; set; }
        public int Priority { get; set; }
        public HashSet<PatternNode> Reactants { get; set; } = new HashSet<PatternNode>();

        public bool Equals(Combination other)
        {
            if (other == null || other.Reactants == null || Reactants == null) return true;
            else if (other.Reactants.Count != Reactants.Count) return false;
            else return Reactants.SequenceEqual(other.Reactants);
        }

        public override bool Equals(object obj)
        {
            if (obj is Combination c) return Equals(c);
            else return false;
        }

        public override int GetHashCode()
        {
            return Reactants.GetHashCode();
        }
    }
}
